using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PortfolioTrackerApi.DAL;
using PortfolioTrackerApi.Entities;
using PortfolioTrackerApi.Repositories;
using PortfolioTrackerApi.Service_Interfaces;
using PortfolioTrackerApi.Services;
using PortfolioTrackerApi.Services.StockPriceApi;
using StackExchange.Redis;
using System;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#region cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin() // Allows requests from any origin
               .AllowAnyMethod()  // Allows any HTTP method (GET, POST, PUT, DELETE, etc.)
               .AllowAnyHeader(); // Allows any HTTP headers
    });
});
#endregion


#region Adding DBContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

#endregion

#region Dependency Injection
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPortfolioRepository, PortfolioRepository>();
builder.Services.AddScoped<IPortfolioService,PortfolioService>();
builder.Services.AddScoped<IStocksRepository, StocksRepository>();
builder.Services.AddScoped<IHistoricalStockPriceRepository,HistoricalStockPriceRepository>();
builder.Services.AddScoped<IStatisticsService,StatisticsService>();
builder.Services.AddScoped<StocksService>();

builder.Services.AddHttpClient();
builder.Services.AddHttpClient<AlphaVantageService>();
builder.Services.AddHttpClient<TwelveDataService>();
//builder.Services.AddSingleton<IRedisService, RedisService>();
//builder.Services.AddHostedService<StockPriceUpdaterService>();
builder.Services.Decorate<IHistoricalStockPriceRepository, HistoricalCacheDecorator>();

builder.Services.AddSingleton<WebSocketHandler>();

builder.Services.AddHostedService<RedisSubscriberService>();

builder.Services.AddTransient<IStockPriceApiService, AlphaVantageService>();
builder.Services.AddTransient<IStockPriceApiService, TwelveDataService>();
builder.Services.AddTransient<StockPriceApiManager>();
//builder.Services.AddHostedService<StockPriceGeneratorService>();
#endregion

builder.Services.Configure<HostOptions>(opts =>
{
    opts.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

#region Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(provider =>
{
    var config = ConfigurationOptions.Parse(builder.Configuration["Redis:ConnectionString"]);
    config.ConnectTimeout = 3000; // Increase timeout to 3 sec
    config.SyncTimeout = 3000;    // Increase sync timeout to 15 sec
    config.ConnectRetry = 1;
    config.AbortOnConnectFail = false; // Prevent failures on startup
    return ConnectionMultiplexer.Connect(config);
});
builder.Services.AddSingleton<IRedisService, RedisService>();
#endregion

#region Adding Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true, // Set to true!
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])), //Get from appsettings.json
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"], //Get from appsettings.json
            ValidAudience = builder.Configuration["JwtSettings:Audience"]
        };
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT Authentication Failed: {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };
    });


string str = builder.Configuration["JwtSettings:SecretKey"];
Console.WriteLine(str);
string issuer = builder.Configuration["JwtSettings:Issuer"];
Console.WriteLine(issuer);
string ValidAudience = builder.Configuration["JwtSettings:Audience"];
Console.WriteLine(ValidAudience);
#endregion
builder.Services.AddAuthorization();

var app = builder.Build();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseRouting();
app.UseWebSockets();
app.UseAuthentication();

app.UseAuthorization();


app.MapControllers();

//Websocket route
app.UseEndpoints(endpoints =>
{
    endpoints.Map("/ws/stocks", async context =>
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            var handler = context.RequestServices.GetRequiredService<WebSocketHandler>();
            await handler.HandleWebSocketAsync(context, webSocket);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    });

    endpoints.MapControllers();
});

app.Run();
