using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CleanArchitecture.Application.Services_Interface;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RabbitMQ.Client;
using MassTransit.RabbitMqTransport.Topology;

namespace CleanArchitecture.Infrastructure.RabbitMQ
{
    public class RabbitMqService : IMessageBus
    {
        private readonly IRabbitMqConnectionFactory _factory;
        private readonly IRabbitMqQueueInitializer _initializer;
        private const string Exchange = "news_exchange";
        private const string RoutingKey = "news_exchange.request";
        //private const string QueueName = "news_searching_queue";
        public RabbitMqService(
            IRabbitMqConnectionFactory factory,
            IRabbitMqQueueInitializer initializer)
        {
            _factory = factory;
            _initializer = initializer;
        }

        public async Task PublishAsync<T>(T message, string queue)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "guest", Password = "guest" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();
            await channel.ExchangeDeclareAsync(Exchange, ExchangeType.Direct, durable: true, autoDelete: false);
            await channel.QueueDeclareAsync(queue: queue, durable: true, exclusive: false, autoDelete: false, arguments: null);
            await channel.QueueBindAsync(queue: queue, exchange: Exchange, routingKey: RoutingKey);
            var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message));
            await channel.BasicPublishAsync(
                exchange: Exchange,
                routingKey: RoutingKey,
                body: body
            );
        }
    }
}
