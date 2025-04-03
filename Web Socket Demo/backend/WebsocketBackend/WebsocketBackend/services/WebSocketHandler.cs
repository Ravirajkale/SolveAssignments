using System.Net.WebSockets;
using System.Text.Json;
using System.Text;
using WebsocketBackend.data;
using Microsoft.EntityFrameworkCore;

namespace WebsocketBackend.services
{
    public class WebSocketHandler
    {
        private static readonly List<WebSocket> _sockets = new List<WebSocket>();
        private readonly IServiceScopeFactory _scopeFactory;

        public WebSocketHandler(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public async Task HandleWebSocketAsync(HttpContext context, WebSocket webSocket)
        {
            _sockets.Add(webSocket);
            await SendMemberCountAsync();

            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    _sockets.Remove(webSocket);
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closed", CancellationToken.None);
                }
            }
        }

        public async Task SendMemberCountAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var members = await dbContext.Members
           .Select(m => new { m.Id, m.Name })  // Select only required fields
           .ToListAsync();

            var message = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(new { members }));
            var buffer = new ArraySegment<byte>(message);

            foreach (var socket in _sockets)
            {
                if (socket.State == WebSocketState.Open)
                {
                    await socket.SendAsync(buffer, WebSocketMessageType.Text, true, CancellationToken.None);
                }
            }
        }
    }
}
