namespace ProductAPI.Services
{
    public interface IRabbitMqService
    {
        void PublishOrderMessage(string stringJson);
    }
}
