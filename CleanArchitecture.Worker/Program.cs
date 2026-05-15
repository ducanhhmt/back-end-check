using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace RabbitPuslish
{
    internal class Program
    {
        const string exchange = "user_Created";
        const string routing = "user.create.request";

        static async Task Main(string[] args)
        {
            var factory = new ConnectionFactory() { HostName = "localhost", Port = 5672, UserName = "guest", Password = "guest" };
            using var connection = await factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.ExchangeDeclareAsync(exchange, ExchangeType.Direct, durable: true,autoDelete:false)  ;
            await channel.QueueDeclareAsync(queue: "user_create_queue", durable: true, exclusive: false, autoDelete: false, arguments: null);
            await channel.QueueBindAsync(queue: "user_create_queue", exchange: exchange, routingKey: routing);

            while (true)
            {
                Console.WriteLine("Enter your Account:");
                string account = Console.ReadLine()!;

                if (account.ToLower() == "exit") break;

                Console.WriteLine("Enter your Password:");
                string password = Console.ReadLine()!;

                var data = new MessageRespone
                {
                    Account = account,
                    Password = password
                };
                var body = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
                await channel.BasicPublishAsync(exchange: exchange, routingKey: routing, body: body);

                Console.WriteLine($"[SENT] UserCreateRequested: {account}");
                Console.WriteLine(" [x] Sent {0}", body);
                Console.WriteLine("-----------------------------------");
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
    public class MessageRespone
    {
        public string Account { get; set; }

        public string Password { get; set; }
    }
}











