using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using StockPriceWorkerService.DB;
using StockPriceWorkerService.Repositories;
using StockPriceWorkerService.Services;

namespace StockPriceWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = Host.CreateApplicationBuilder(args);

            var configuration = builder.Configuration;

            // Register AppDbContext with SQL Server (or your DB provider)
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
           
            builder.Services.Configure<HostOptions>(opts =>
            {
                opts.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
            });
            // Register Redis connection
            builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var endpoints = config.GetSection("Redis:Endpoints").Get<string[]>();

                var options = new ConfigurationOptions
                {
                    AbortOnConnectFail = false,
                    ConnectRetry = 3,
                    ConnectTimeout = 3000,
                    KeepAlive = 10,
                };

                foreach (var endpoint in endpoints)
                {
                    options.EndPoints.Add(endpoint);
                }

                return ConnectionMultiplexer.Connect(options);
            });

            // Register services and repositories
            builder.Services.AddScoped<IStockPriceRepository, StockPriceRepository>();
            builder.Services.AddScoped<IHistoricalStockPriceRepository, HistoricalStockPriceRepository>();
            builder.Services.AddSingleton<IRedisService, RedisService>();

            // Register background service
            builder.Services.AddHostedService<StockPriceGeneratorService>();

            var host = builder.Build();
            host.Run();
        }
    }
}