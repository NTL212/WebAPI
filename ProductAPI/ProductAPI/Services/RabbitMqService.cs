using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace ProductAPI.Services
{
    public class RabbitMqService
    {
        private readonly IConnection connection;
        private readonly IModel channel;

        public RabbitMqService()
        {
            var factory = new ConnectionFactory
            {
                UserName = "loimq",
                Password = "123456",
                HostName = "localhost",
                AutomaticRecoveryEnabled = true, // Tự động khôi phục kết nối
                NetworkRecoveryInterval = TimeSpan.FromSeconds(10) // Khoảng thời gian thử lại
            };
            connection = factory.CreateConnection();
            channel = connection.CreateModel();

            // Declare a queue
            channel.QueueDeclare(queue: "OrderQueue2",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

        }

        // Method to publish messages to the queue
        public void PublishOrderMessage(string stringJson)
        {
            var body = System.Text.Encoding.UTF8.GetBytes(stringJson);

            channel.BasicPublish(
                    exchange: "",
                    routingKey: "OrderQueue2",
                    basicProperties: null,
                    body: body
                );
        }

        // Method to consume messages from the queue
        public void ConsumeMessages()
        {
            channel.BasicConsume("OrderQueue2", autoAck: true, consumer: new EventingBasicConsumer(channel));
        }
    }
}
