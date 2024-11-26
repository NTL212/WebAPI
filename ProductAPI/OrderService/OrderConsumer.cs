using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductDataAccess.Repositories;
using ProductDataAccess.Repositories.Interfaces;
using ProductDataAccess.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using ProductDataAccess.Models.Request;
using System.Threading.Channels;
using MassTransit.Transports;
using ProductDataAccess.ViewModels;
using System.Net;

namespace OrderService
{
    public class OrderConsumer
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IHttpClientFactory _httpClientFactory;
        private IConnection _connection;
        private IModel _channel;
        private readonly ILogger<OrderConsumer> _logger;
        private readonly string _apiUrl = "https://localhost:7016/";

        public OrderConsumer(
            IServiceScopeFactory serviceScopeFactory,
            IOrderRepository orderRepository,
            ILogger<OrderConsumer> logger,
            IProductRepository productRepository,
            IHttpClientFactory httpClientFactory)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;

        }



        public async Task StartConsuming()
        {


            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = "localhost",
                    UserName = "loimq",
                    Password = "123456",
                };

                // Tạo connection và channel ngoài khối using để không bị dispose quá sớm
                var connection = factory.CreateConnection();
                var channel = connection.CreateModel();

                // Đảm bảo queue tồn tại
                channel.QueueDeclare(queue: "OrderQueue2",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);

                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    _logger.LogInformation($"Received message: {message}");

                    try
                    {
                        // Deserialize RabbitMessage
                        var rabbitMessage = JsonConvert.DeserializeObject<RabbitMessage>(message);

                        if (rabbitMessage == null || rabbitMessage.Payload == null)
                        {
                            _logger.LogWarning("Invalid message format or null payload.");
                            channel.BasicAck(ea.DeliveryTag, false); // Acknowledge the message to remove it from the queue
                            return;
                        }

                        // Xử lý từng action type
                        switch (rabbitMessage.ActionType)
                        {
                            case "Create":
                                await HandleCreateAction(rabbitMessage);
                                break;

                            case "Cancel":
                                await HandleCancelAction(rabbitMessage);
                                break;

                            case "Confirm":
                                await HandleConfirmAction(rabbitMessage);
                                break;

                            default:
                                _logger.LogWarning($"Unknown action type: {rabbitMessage.ActionType}");
                                break;
                        }

                        // Xác nhận xử lý thành công
                        channel.BasicAck(ea.DeliveryTag, false);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Error processing message: {ex.Message}");
                        // Không acknowledge để message quay lại hàng đợi nếu gặp lỗi
                        channel.BasicNack(ea.DeliveryTag, false, true); // Nack the message and requeue it
                    }
                };

                // Bắt đầu tiêu thụ tin nhắn từ queue
                channel.BasicConsume(queue: "OrderQueue2", autoAck: false, consumer: consumer);
                _logger.LogInformation("Order consumer is running.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error starting consumer: {ex.Message}");
            }

        }



        private async Task HandleCreateAction(RabbitMessage rabbitMessage)
        {
            var order = JsonConvert.DeserializeObject<Order>(rabbitMessage.Payload.ToString());
            if (order == null)
            {
                _logger.LogWarning("Invalid order data.");
                return;
            }

            // Gọi API để tạo đơn hàng
            var handler = new HttpClientHandler
            {
                UseCookies = true,
                CookieContainer = new CookieContainer(),
            };
            var client = new HttpClient(handler);
            client.DefaultRequestHeaders.Add("Origin", "https://client-origin.com");

            var resultJson = JsonConvert.SerializeObject(order);
            await client.PostAsync($"{_apiUrl}Cart/OrderResult", new StringContent(resultJson, Encoding.UTF8, "application/json"));
            try
            {
                var response = await client.PostAsync($"{_apiUrl}api/order", new StringContent(resultJson, Encoding.UTF8, "application/json"));
               if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    await client.PostAsync($"{_apiUrl}Cart/OrderResult", new StringContent(content, Encoding.UTF8, "application/json"));
                    _logger.LogInformation($"Order {order.OrderId} created successfully via API.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning($"Failed to create order {order.OrderId}. Response: {response.StatusCode}, Error: {errorContent}");
                    _logger.LogWarning($"Failed to create order {order.OrderId}. Status Code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error calling API to create order: {ex.Message}");
            }
        }

        private async Task HandleCancelAction(RabbitMessage rabbitMessage)
        {
            var orderToCancelId = JsonConvert.DeserializeObject<int>(rabbitMessage.Payload.ToString());

            // Gọi API để hủy đơn hàng
            var client = _httpClientFactory.CreateClient();
            var response = await client.DeleteAsync($"{_apiUrl}api/order/{orderToCancelId}");

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Order {orderToCancelId} cancelled successfully via API.");
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning($"Failed to cancel order {orderToCancelId}. Response: {response.StatusCode}, Error: {errorContent}");
            }
        }

        private async Task HandleConfirmAction(RabbitMessage rabbitMessage)
        {
            var orderToConfirmId = JsonConvert.DeserializeObject<int>(rabbitMessage.Payload.ToString());

            // Gọi API để cập nhật trạng thái đơn hàng
            var client = _httpClientFactory.CreateClient();
            var content = new StringContent(JsonConvert.SerializeObject(new { Status = "Confirmed" }), Encoding.UTF8, "application/json");

            var response = await client.PutAsync($"{_apiUrl}api/order/{orderToConfirmId}/status", content);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Order {orderToConfirmId} confirmed successfully via API.");
            }
            else
            {
                _logger.LogWarning($"Failed to confirm order {orderToConfirmId}. Status Code: {response.StatusCode}");
            }
        }
    }
}
