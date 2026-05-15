using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.RabbitMQ
{
    public interface IRabbitMqQueueInitializer
    {
        Task<IChannel> InitializeAsync(IConnection connection,string exchange,string queue,string routingKey, CancellationToken cancellationToken);
    }
    public class RabbitMqQueueInitializer : IRabbitMqQueueInitializer
    {
        public async Task<IChannel> InitializeAsync(
            IConnection connection,
            string exchange,
            string queue,
            string routingKey,
            CancellationToken cancellationToken)
        {
            var channel = await connection.CreateChannelAsync(cancellationToken:cancellationToken);
            await channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct, durable: true, cancellationToken: cancellationToken);
            await channel.QueueDeclareAsync(queue, durable: true, exclusive: false, autoDelete: false, cancellationToken: cancellationToken);
            await channel.QueueBindAsync(queue, exchange, routingKey, cancellationToken: cancellationToken);
            return channel;
        }
    }

}
