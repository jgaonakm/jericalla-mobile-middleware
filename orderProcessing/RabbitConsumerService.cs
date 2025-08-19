
using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

public sealed class RabbitConsumerService : BackgroundService
{
    private readonly IConnectionFactory _factory;
    private readonly IOptionsMonitor<RabbitOptions> _options;
    private readonly ILogger<RabbitConsumerService> _logger;
    private readonly ConsumerHealthState _health;

    private IConnection? _conn;
    private IChannel? _ch; 

    public RabbitConsumerService(
        IConnectionFactory factory,
        IOptionsMonitor<RabbitOptions> options,
        ILogger<RabbitConsumerService> logger,
        ConsumerHealthState health)
    {
        _factory = factory;
        _options = options;
        _logger = logger;
        _health = health;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var retry = 0;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                EnsureConnected();
                _health.IsConnected = true;
                RabbitMetrics.ConnectionUp.Set(1);

                await _ch!.BasicQosAsync(0, _options.CurrentValue.Prefetch, global: false, cancellationToken: stoppingToken);

                await _ch.QueueDeclareAsync(
                    queue: _options.CurrentValue.QueueName,
                    durable: true,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null,
                    cancellationToken: stoppingToken);

                var consumer = new AsyncEventingBasicConsumer(_ch);
                consumer.ReceivedAsync += OnMessageAsync; 

                // Start consuming
                var tag = await _ch.BasicConsumeAsync(
                    queue: _options.CurrentValue.QueueName,
                    autoAck: false,
                    consumerTag: string.Empty,
                    noLocal: false,
                    exclusive: false,
                    arguments: null,
                    consumer: consumer,
                    cancellationToken: stoppingToken);

                _logger.LogInformation("Consuming on queue '{Queue}' with tag {Tag}", _options.CurrentValue.QueueName, tag);

                // Keep alive while channel is open
                while (!stoppingToken.IsCancellationRequested && _conn!.IsOpen && _ch!.IsOpen)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }

                _logger.LogWarning("Channel or connection closed; will attempt reconnect.");
                _health.IsConnected = false;
                RabbitMetrics.ConnectionUp.Set(0);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Consumer loop error.");
                _health.IsConnected = false;
                RabbitMetrics.ConnectionUp.Set(0);
            }
            finally
            {
                await SafeCloseAsync();
            }

            retry++;
            RabbitMetrics.Reconnects.Inc();
            var delay = TimeSpan.FromSeconds(Math.Min(30, Math.Pow(2, Math.Min(retry, 5))));
            await Task.Delay(delay, stoppingToken);
        }
    }

    // Async Received handler with manual ack/nack
    private async Task OnMessageAsync(object sender, BasicDeliverEventArgs ea)
    {
        var queue = _options.CurrentValue.QueueName;
        RabbitMetrics.MessagesReceived.WithLabels(queue).Inc();

        RabbitMetrics.Inflight.Inc();
        var sw = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Received message from queue '{Queue}': {Message}", queue, ea.BasicProperties?.MessageId ?? "No ID");
            var text = Encoding.UTF8.GetString(ea.Body.Span);
            _logger.LogInformation("Message content: {Text}", text);

            // TODO: business logic (can be awaited)
            // e.g. await DoWorkAsync(text);
            // ...

            await _ch!.BasicAckAsync(ea.DeliveryTag, multiple: false);
            RabbitMetrics.Acked.WithLabels(queue).Inc();
            RabbitMetrics.Processed.WithLabels(queue).Inc();
            _logger.LogInformation("Processed message from queue '{Queue}': {Message}", queue, ea.BasicProperties?.MessageId ?? "No ID");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to process message. Nacking.");
            await _ch!.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: false);
            RabbitMetrics.Nacked.WithLabels(queue).Inc();
            RabbitMetrics.Failures.WithLabels(queue).Inc();
        }
        finally
        {
            sw.Stop();
            RabbitMetrics.ProcessingSeconds.WithLabels(queue).Observe(sw.Elapsed.TotalSeconds);
            RabbitMetrics.Inflight.Dec();
        }
    }

    private async void EnsureConnected()
    {
        if (_conn is { IsOpen: true } && _ch is { IsOpen: true }) return;

        SafeCloseSync();

        _conn = await _factory.CreateConnectionAsync();
        _ch = await _conn.CreateChannelAsync(); 

        _conn.ConnectionShutdownAsync += async (_, args) =>
        {
            _logger.LogWarning("RabbitMQ connection shutdown: {ReplyText}", args.ReplyText);
            _health.IsConnected = false;
            RabbitMetrics.ConnectionUp.Set(0);
            await Task.CompletedTask;
        };
        _ch.ChannelShutdownAsync += async (_, args) =>
        {
            _logger.LogWarning("RabbitMQ channel shutdown: {ReplyText}", args.ReplyText);
            _health.IsConnected = false;
            RabbitMetrics.ConnectionUp.Set(0);
            await Task.CompletedTask;
        };
    }

    private async Task SafeCloseAsync()
    {
        try { if (_ch != null) await _ch.CloseAsync(); } catch { }
        try { _ch?.Dispose(); } catch { }
        try { if (_conn != null) await _conn.CloseAsync(); } catch { }
        try { _conn?.Dispose(); } catch { }
        _ch = null;
        _conn = null;
    }

    private void SafeCloseSync()
    {
        try { _ch?.CloseAsync().GetAwaiter().GetResult(); } catch { }
        try { _ch?.Dispose(); } catch { }
        try { _conn?.CloseAsync().GetAwaiter().GetResult(); } catch { }
        try { _conn?.Dispose(); } catch { }
        _ch = null;
        _conn = null;
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Stopping consumer...");
        await SafeCloseAsync();
        await base.StopAsync(cancellationToken);
    }
}
