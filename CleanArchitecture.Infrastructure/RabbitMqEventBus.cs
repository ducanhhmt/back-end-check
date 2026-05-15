//using Microsoft.Extensions.Configuration;
//using RabbitMQ.Client;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;

//namespace CleanArchitecture.Infrastructure
//{
//    public interface IEventBus
//    {
//        Task PublishAsync<T>(T @event);
//    }
//    public class RabbitMqEventBus : IEventBus
//    {
//        private readonly IConnection _connection;

//        public RabbitMqEventBus(/*IConfiguration config*/)
//        {
//            var factory = new ConnectionFactory
//            {
//                //HostName = config["RabbitMQ:Host"],
//                //UserName = config["RabbitMQ:Username"],
//                //Password = config["RabbitMQ:Password"]
//                HostName = "localhost",
//                UserName = "guest",
//                Password = "guest",
//            };

//            _connection = Task.Run(async () => await factory.CreateConnectionAsync()).Result;
//        }

//        // Cách tốt hơn: async factory method (nếu không dùng DI)
//        public static async Task<RabbitMqEventBus> CreateAsync()
//        {
//            var factory = new ConnectionFactory
//            {
//                HostName = "localhost",
//                UserName = "guest",
//                Password = "guest",
//            };

//            var connection = await factory.CreateConnectionAsync();
//            return new RabbitMqEventBus(connection);
//        }

//        // Private constructor để inject connection
//        private RabbitMqEventBus(IConnection connection)
//        {
//            _connection = connection;
//        }

//        public async Task PublishAsync<T>(T @event)
//        {
//            // Tạo channel mới cho mỗi publish (khuyến nghị để tránh sharing channel)
//            await using var channel = await _connection.CreateChannelAsync();

//            var exchange = "user.exchange";
//            await channel.ExchangeDeclareAsync(
//                exchange: exchange,
//                type: ExchangeType.Topic,
//                durable: true,
//                autoDelete: false,
//                arguments: null);

//            var routingKey = typeof(T).Name;

//            var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(@event));

//            await channel.BasicPublishAsync(
//                exchange: exchange,
//                routingKey: routingKey,
//                basicProperties: null,  // Có thể tạo IBasicProperties nếu cần headers
//                body: body,
//                mandatory: false);

//            // Không cần return gì thêm, publish là fire-and-forget
//        }

//        // Dispose connection khi không dùng nữa (nếu cần)
//        public void Dispose()
//        {
//            _connection?.Dispose();
//        }
//    }
//}
