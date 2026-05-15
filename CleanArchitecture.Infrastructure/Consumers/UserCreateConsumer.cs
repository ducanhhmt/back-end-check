using CleanArchitecture.Application.Features.Users;
using CleanArchitecture.Application.Repository_Interface;
using CleanArchitecture.Infrastructure.RabbitMQ;
using MassTransit;
using MediatR;
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
    //public class UserCreateConsumer : BackgroundService
    //{
    //    private readonly IRabbitMqConnectionFactory _connection;
    //    private readonly IRabbitMqQueueInitializer _queue;
    //    private readonly ILogger<UserCreateConsumer> _logger;
    //    private readonly IServiceScopeFactory _scopeFactory;

    //    private const string Exchange = "user_Created";
    //    private const string RoutingKey = "user.create.request";
    //    private const string QueueName = "user_create_queue";

    //    public UserCreateConsumer(IRabbitMqConnectionFactory connection, IRabbitMqQueueInitializer queue, 
    //        ILogger<UserCreateConsumer> logger, IServiceScopeFactory scopeFactory)
    //    {
    //        _connection = connection;
    //        _queue = queue;
    //        _logger = logger;
    //        _scopeFactory = scopeFactory;
    //    }

    //    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    //    {
    //        await using var connection = await _connection.CreateConnectionAsync(stoppingToken);
    //        await using var channel = await _queue.InitializeAsync(connection,Exchange,QueueName,RoutingKey,stoppingToken);
    //        var consumer = new AsyncEventingBasicConsumer(channel);

    //        consumer.ReceivedAsync += async (model, ea) =>
    //        {
    //            try
    //            {
    //                var body = ea.Body.ToArray();
    //                var message = Encoding.UTF8.GetString(body);

    //                _logger.LogInformation("Received message: {Message}", message);

    //                var request = JsonSerializer.Deserialize<AddUserCommand>(message, new JsonSerializerOptions
    //                {
    //                    PropertyNameCaseInsensitive = true
    //                });

    //                if (request == null || string.IsNullOrWhiteSpace(request.Account))
    //                {
    //                    _logger.LogWarning("Invalid request, skipping.");
    //                    return;
    //                }
    //                using var scope = _scopeFactory.CreateScope();
    //                var sender = scope.ServiceProvider.GetRequiredService<ISender>();
    //                //await _sender.Send(new AddUserCommand
    //                //{
    //                //    Account = request.Account,
    //                //    Password = request.Password ?? string.Empty
    //                //}, stoppingToken);
    //                await sender.Send(request);
    //                _logger.LogInformation("User created successfully: {Account}", request.Account);

    //                // Ack message (xóa khỏi queue)
    //                await channel.BasicAckAsync(ea.DeliveryTag, false, stoppingToken);
    //            }
    //            catch (Exception ex)
    //            {
    //                _logger.LogError(ex, "Error processing message");
    //                // Nack + requeue để thử lại sau
    //                await channel.BasicNackAsync(ea.DeliveryTag, false, true, stoppingToken);
    //            }
    //        };

    //        await channel.BasicConsumeAsync(
    //            queue: QueueName,
    //            autoAck: false, // Quan trọng: tự quản lý ack/nack
    //            consumer: consumer,
    //            cancellationToken: stoppingToken);

    //        _logger.LogInformation("UserCreateConsumer is running and listening on queue: {Queue}", QueueName);

    //        // Giữ consumer chạy mãi
    //        await Task.Delay(Timeout.Infinite, stoppingToken);
    //    }
    //}
}