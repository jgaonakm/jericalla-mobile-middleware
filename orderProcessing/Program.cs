
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;

var builder = Host.CreateApplicationBuilder(args);

var services = builder.Services;
services.AddDbContext<OrderDbContext>(opts =>
    opts.UseSqlServer(builder.Configuration.GetConnectionString("OrdersDb")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IOrderService, OrderService>();

services.Configure<RabbitOptions>(builder.Configuration.GetSection("Rabbit"));
services.AddScoped<IConnectionFactory>(sp =>
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

// services.AddSingleton<ConsumerHealthState>();
services.AddHostedService<RabbitConsumerService>();

var app = builder.Build();

// Apply migrations on startup
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    db.Database.Migrate();
}


// // Prometheus & health endpoints
// app.UseHttpMetrics();
// app.MapMetrics("/metrics");
// app.MapGet("/healthz", () => Results.Ok(new { status = "ok" }));
// app.MapGet("/readyz", (ConsumerHealthState s) =>
//     s.IsConnected ? Results.Ok(new { status = "ready" }) : Results.StatusCode(503));

app.Run();
