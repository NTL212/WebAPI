using OrderService;



public class OrderConsumerBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<OrderConsumerBackgroundService> _logger;

    public OrderConsumerBackgroundService(IServiceProvider serviceProvider, ILogger<OrderConsumerBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        
        using (var scope = _serviceProvider.CreateScope())
        {
            var orderConsumer = scope.ServiceProvider.GetRequiredService<OrderConsumer>();
            await orderConsumer.StartConsuming();
        }
    }
}
