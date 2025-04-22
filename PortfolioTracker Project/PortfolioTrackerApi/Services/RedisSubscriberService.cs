using PortfolioTrackerApi.Entities;
using StackExchange.Redis;
using System.Text.Json;

namespace PortfolioTrackerApi.Services
{
    public class RedisSubscriberService : BackgroundService
    {
        private readonly IRedisService _redisService;
        private readonly WebSocketHandler _webSocketHandler;

        public RedisSubscriberService(IRedisService redisService, WebSocketHandler handler)
        {
            _redisService = redisService;
            _webSocketHandler = handler;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var subscriber = _redisService.GetSubscriber();

            await subscriber.SubscribeAsync("stock-updates", async (channel, message) =>
            {
                if (message == "prices-updated")
                {
                    await _webSocketHandler.NotifyStockPriceUpdate();
                }
            });
        }
    }
}