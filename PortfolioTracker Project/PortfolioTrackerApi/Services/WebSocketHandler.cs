using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using System.Collections.Concurrent;
using PortfolioTrackerApi.Service_Interfaces;

namespace PortfolioTrackerApi.Services
{
    public class WebSocketHandler
    {
        private static readonly ConcurrentBag<WebSocket> _sockets = new();
        private readonly IRedisService _redisService;

        public WebSocketHandler(IRedisService redisService)
        {
            _redisService = redisService;
        }

        public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
        {
            _sockets.Add(webSocket);

            try
            {
                var buffer = new byte[1024 * 4];
                while (webSocket.State == WebSocketState.Open)
                {
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    if (result.MessageType == WebSocketMessageType.Close)
                    {
                        _sockets.TryTake(out _); // Remove socket safely
                        await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"WebSocket Error: {ex.Message}");
            }
            finally
            {
                _sockets.TryTake(out _); // Ensure socket is removed
            }
        }

        public async Task NotifyStockPriceUpdate()
        {
           // var stocks = await _redisService.GetStockPricesAsync();
            var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { type = "price-updated"}));

            var buffer = new ArraySegment<byte>(message);
            var tasks = new List<Task>();

            foreach (var socket in _sockets)

            {
                if (socket.State == WebSocketState.Open)
                {
                    tasks.Add(socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None));
                }
            }

            await Task.WhenAll(tasks);
        }
    }
}