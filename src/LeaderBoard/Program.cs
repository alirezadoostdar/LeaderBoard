using LeaderBoard.Database;
using LeaderBoard.Subscriptions.PlayerScoreSubscriber;
using LeaderBoard.Subscriptions.TopSoldProductSubscriber;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using System.Numerics;

var builder = WebApplication.CreateBuilder(args);

builder.BrokerConfigure();
builder.ConfigureApplicationDbContext();
builder.Services.AddSingleton<SortedInMemoryDatabase>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/{topic}/inmemorydatabase", (string Topic,int k,LeaderBoardDbContext dbContext) =>
{
    if (Topic == "order")
    {
        var items = dbContext.MostSoldProducts.OrderByDescending(d=>d.Score).Take(k).ToList();
        return Results.Ok(items);
    }
    else if(Topic == "game")
    {
        var items = dbContext.PlayerScores.OrderByDescending(d => d.Score).Take(k).ToList();
        return Results.Ok(items);
    }
    throw new InvalidOperationException();
});

app.MapGet("/{topic}/sorted-set", (string Topic, int k, SortedInMemoryDatabase sortedSet) =>
{
    if (Topic == "order")
    {
        var items = sortedSet.MostSoldProducts.OrderByDescending(d => d.Score).Take(k).ToList();
        return Results.Ok(items);
    }
    else if (Topic == "game")
    {
        var items = sortedSet.PlayerScores.OrderByDescending(d => d.Score).Take(k).ToList();
        return Results.Ok(items);
    }
    throw new InvalidOperationException();
});

app.MapPost("/game",async (string player, int score ,IPublishEndpoint endpoint) =>
{
   await endpoint.Publish(new PlayerScoreChangedEvent(player, score));
});
app.MapPost("/ordering", async (string catalog_id, IPublishEndpoint endpoint) =>
{
  await  endpoint.Publish(new SoldProductEvent(catalog_id));
});

app.Run();

public static class WebApplicationExtensions
{
    public static void BrokerConfigure(this IHostApplicationBuilder builder)
    {
        builder.Services.AddMassTransit(configure =>
        {
            var brokerConfig = builder.Configuration.GetSection(BrokerOptions.SectionName)
                                                    .Get<BrokerOptions>();
            if (brokerConfig is null)
                throw new ArgumentNullException(nameof(BrokerOptions));

            configure.AddConsumers(Assembly.GetExecutingAssembly());
            configure.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(brokerConfig.Host, hostConfigure =>
                {
                    hostConfigure.Username(brokerConfig.Username);
                    hostConfigure.Password(brokerConfig.Password);
                });

                cfg.ConfigureEndpoints(context);
            });
        });
    }

    public static void ConfigureApplicationDbContext(this IHostApplicationBuilder builder)
    {
        builder.Services.AddDbContext<LeaderBoardDbContext>(configure =>
        {
            configure.UseInMemoryDatabase(nameof(LeaderBoardDbContext));
        });
    }

    public static void ConfigureRedis(this WebApplicationBuilder builder)
    {
        builder.Services.AddSingleton<IConnectionMultiplexer>(ps =>
        {
           var connectionString =builder.Configuration.GetConnectionString("RedisConnection");
            if(connectionString is null)
                throw new ArgumentNullException(nameof(connectionString));
            return ConnectionMultiplexer.Connect(connectionString);
        });
    }
}