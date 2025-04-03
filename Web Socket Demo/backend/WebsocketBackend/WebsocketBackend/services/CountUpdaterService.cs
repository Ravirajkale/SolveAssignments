using WebsocketBackend.data;

namespace WebsocketBackend.services
{
    public class CountUpdaterService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly WebSocketHandler _webSocketHandler;
        private readonly ILogger<CountUpdaterService> _logger;

        public CountUpdaterService(IServiceScopeFactory serviceScopeFactory, WebSocketHandler webSocketHandler, ILogger<CountUpdaterService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _webSocketHandler = webSocketHandler;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // Add a new member every 10 seconds
                dbContext.Members.Add(new Member { Name = $"Member {DateTime.UtcNow.Ticks}" });
                await dbContext.SaveChangesAsync();

                _logger.LogInformation("Updated member count.");
                await _webSocketHandler.SendMemberCountAsync();

                await Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
            }
        }
    }
}
