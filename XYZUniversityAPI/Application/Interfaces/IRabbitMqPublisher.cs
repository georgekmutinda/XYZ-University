namespace XYZUniversityAPI.Application.Interfaces
{
    public interface IRabbitMqPublisher
    {
        Task PublishAsync<T>(string queueName, T message);
    }
}
