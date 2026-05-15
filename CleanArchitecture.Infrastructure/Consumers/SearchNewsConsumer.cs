using CleanArchitecture.Application.DTO;
using CleanArchitecture.Application.Features.News;
using CleanArchitecture.Application.Features.Users;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Infrastructure.HubRealTime;
using CleanArchitecture.Infrastructure.RabbitMQ;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using MongoDB.Bson.IO;
using MongoDB.Driver.Core.Connections;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace CleanArchitecture.Infrastructure.Consumers
{
    public class SearchNewsConsumer : BackgroundService
    {
        private readonly IRabbitMqConnectionFactory _connection;
        private readonly IRabbitMqQueueInitializer _queue;
        private readonly IServiceProvider _serviceProvider;

        private const string Exchange = "news_exchange";
        private const string RoutingKey = "news_exchange.request";
        private const string QueueName = "news_searching_queue";

        public SearchNewsConsumer(IRabbitMqConnectionFactory connection, IRabbitMqQueueInitializer queue, IServiceProvider serviceProvider )
        {
            _connection = connection;
            _queue = queue;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await using var connection = await _connection.CreateConnectionAsync(stoppingToken);
            await using var channel = await _queue.InitializeAsync(connection, Exchange, QueueName, RoutingKey, stoppingToken);
            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var request = JsonSerializer.Deserialize<SearchNewsMessage>(message, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (request == null || string.IsNullOrWhiteSpace(request.Keyword))
                    {
                        return;
                    }
                    // 👇 xử lý logic tại đây
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                        var hub = scope.ServiceProvider.GetRequiredService<IHubContext<NewsHub>>();

                        var result = await mediator.Send(new SearchingNewsQueries
                        {
                            Keyword = request.Keyword
                        });
                        // 👇 push về UI
                        await hub.Clients.Group(request.RequestId).SendAsync("ReceiveResult", result);
                    }

                    // Ack message (xóa khỏi queue)
                    await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
                }
                catch (Exception ex)
                {
                    // Nack + requeue để thử lại nếu có lỗi 
                    await channel.BasicNackAsync(ea.DeliveryTag, false, false, stoppingToken);
                }
            };

            await channel.BasicConsumeAsync(
                queue: QueueName,
                autoAck: false, // Quan trọng: tự quản lý ack/nack
                consumer: consumer,
                cancellationToken: stoppingToken);

            // Giữ consumer chạy mãi
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
    }
}