using RabbitMQ.Client;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
// 1. This must point to your Application interfaces
using XYZUniversityAPI.Application.Interfaces; 

namespace XYZUniversityAPI.Infrastructure.Messaging
{
    // 2. DELETE the interface that was here. It's causing the mismatch.

    public class RabbitMqPublisher : IRabbitMqPublisher // 3. Now this refers to the Application one
    {
        private readonly ConnectionFactory _factory;

        // 4. Update constructor to use the Factory from DI (registered in Program.cs)
        public RabbitMqPublisher(ConnectionFactory factory)
        {
            _factory = factory;
        }

        public async Task PublishAsync<T>(string queueName, T message)
        {
            using var connection = await _factory.CreateConnectionAsync();
            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null
            );

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            var properties = new BasicProperties { Persistent = true };

            await channel.BasicPublishAsync(
                exchange: string.Empty,
                routingKey: queueName,
                mandatory: false,
                basicProperties: properties,
                body: body
            );
        }
    }
}
