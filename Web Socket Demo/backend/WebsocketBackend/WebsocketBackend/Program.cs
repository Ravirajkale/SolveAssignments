
using Microsoft.EntityFrameworkCore;
using WebsocketBackend.data;
using WebsocketBackend.services;

namespace WebsocketBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register DbContext with In-Memory Database
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseInMemoryDatabase("MembersDb"));

            // Register WebSocket handler as a singleton
            builder.Services.AddSingleton<WebSocketHandler>();

            // Register background service to update member count
            builder.Services.AddHostedService<CountUpdaterService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            // Enable WebSockets
            app.UseWebSockets();
            app.Use(async (context, next) =>
            {
                if (context.Request.Path == "/ws/members")
                {
                    if (context.WebSockets.IsWebSocketRequest)
                    {
                        var webSocketHandler = context.RequestServices.GetRequiredService<WebSocketHandler>();
                        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                        await webSocketHandler.HandleWebSocketAsync(context, webSocket);
                    }
                    else
                    {
                        context.Response.StatusCode = 400;
                    }
                }
                else
                {
                    await next();
                }
            });

            app.MapControllers();

            app.Run();
        }
    }
}
