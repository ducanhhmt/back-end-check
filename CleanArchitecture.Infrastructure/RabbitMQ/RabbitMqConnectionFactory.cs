using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanArchitecture.Infrastructure.RabbitMQ
{
    public interface IRabbitMqConnectionFactory
    {
        Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken);
    }
    public class RabbitMqConnectionFactory : IRabbitMqConnectionFactory
    {
        private readonly ConnectionFactory _factory;

        public RabbitMqConnectionFactory(IConfiguration configuration)
        {
            _factory = new ConnectionFactory
            {
                HostName = configuration["RabbitMQ:Host"] ?? "localhost",
                Port = int.Parse(configuration["RabbitMQ:Port"] ?? "5672"),
                UserName = configuration["RabbitMQ:User"] ?? "guest",
                Password = configuration["RabbitMQ:Password"] ?? "guest",
                AutomaticRecoveryEnabled = true
            };
        }

        public async Task<IConnection> CreateConnectionAsync(CancellationToken cancellationToken)
        {
            return await _factory.CreateConnectionAsync(cancellationToken);
        }
    }

}
