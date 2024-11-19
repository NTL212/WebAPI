namespace OrderService.Services
{
    public class OrderConsumerBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public OrderConsumerBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // Đảm bảo rằng consumer chỉ được tạo trong scope hợp lệ
            using (var scope = _serviceProvider.CreateScope())
            {
                var orderConsumer = scope.ServiceProvider.GetRequiredService<OrderConsumer>();
                await orderConsumer.StartConsuming();
            }
        }
    }

}
