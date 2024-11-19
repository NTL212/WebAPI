using Newtonsoft.Json;
using OrderService.Repositories;
using OrderService.Repositories.Interfaces;
using ProductDataAccess.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace OrderService
{
    public class OrderConsumer
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<OrderConsumer> _logger;

        public OrderConsumer(IOrderRepository orderRepository, ILogger<OrderConsumer> logger)
        {
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async 
        Task
StartConsuming()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };

            using (var connection = await factory.CreateConnectionAsync())
            using (var channel = await connection.CreateChannelAsync())
            {
                // Đảm bảo queue tồn tại
                await channel.QueueDeclareAsync(queue: "OrderQueue",
                                     durable: true,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new AsyncEventingBasicConsumer(channel);

                consumer.ReceivedAsync += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    // Deserialize the order message
                    var order = JsonConvert.DeserializeObject<Order>(message);

                    if (order != null)
                    {
                        try
                        {
                            // Gọi repository để lưu đơn hàng vào cơ sở dữ liệu
                            var orderCreated = await _orderRepository.AddAsync(order);

                            if (orderCreated != null)
                            {
                                _logger.LogInformation($"Order {order.OrderId} processed successfully.");
                            }
                            else
                            {
                                _logger.LogWarning($"Failed to create order {order.OrderId}.");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error processing order: {ex.Message}");
                        }
                    }
                };

                // Bắt đầu tiêu thụ tin nhắn từ queue
                await channel.BasicConsumeAsync(queue: "OrderQueue", autoAck: true, consumer: consumer);

                Console.WriteLine("Order consumer is running. Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}
