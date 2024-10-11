var builder = WebApplication.CreateBuilder(args);

builder.BrokerConfigure();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.MapGet("/{topic}", (string Topic) =>
{

});

app.MapPost("/game", (string Topic,string score) =>
{

});
app.MapPost("/ordering", (string catalog_id) =>
{

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
}