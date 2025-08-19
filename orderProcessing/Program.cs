using Microsoft.Extensions.Options;
using Prometheus;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
services.Configure<RabbitOptions>(builder.Configuration.GetSection("Rabbit"));
services.AddSingleton<IConnectionFactory>(sp =>
{
    var o = sp.GetRequiredService<IOptions<RabbitOptions>>().Value;
    return new ConnectionFactory
    {
        HostName = o.Host,
        Port = o.Port,
        VirtualHost = o.VirtualHost,
        UserName = o.User,
        Password = o.Password,
        AutomaticRecoveryEnabled = false
    };
});

services.AddSingleton<ConsumerHealthState>();
services.AddHostedService<RabbitConsumerService>();

var app = builder.Build();

// Prometheus & health endpoints
app.UseHttpMetrics();
app.MapMetrics("/metrics");
app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));
app.MapGet("/readyz", (ConsumerHealthState s) =>
    s.IsConnected ? Results.Ok(new { status = "ready" }) : Results.StatusCode(503));

app.Run();