// using Prometheus;

// static class RabbitMetrics
// {
//     public static readonly Counter MessagesReceived = Metrics.CreateCounter(
//         "rabbit_consumer_messages_total", "Total messages received from broker", new CounterConfiguration { LabelNames = new[] { "queue" } });

//     public static readonly Counter Processed = Metrics.CreateCounter(
//         "rabbit_consumer_processed_total", "Total messages processed successfully", new CounterConfiguration { LabelNames = new[] { "queue" } });

//     public static readonly Counter Failures = Metrics.CreateCounter(
//         "rabbit_consumer_failures_total", "Total message processing failures", new CounterConfiguration { LabelNames = new[] { "queue" } });

//     public static readonly Counter Acked = Metrics.CreateCounter(
//         "rabbit_consumer_ack_total", "Total acks sent to broker", new CounterConfiguration { LabelNames = new[] { "queue" } });

//     public static readonly Counter Nacked = Metrics.CreateCounter(
//         "rabbit_consumer_nack_total", "Total nacks sent to broker", new CounterConfiguration { LabelNames = new[] { "queue" } });

//     public static readonly Histogram ProcessingSeconds = Metrics.CreateHistogram(
//         "rabbit_consumer_processing_seconds", "Time to process a message",
//         new HistogramConfiguration
//         {
//             LabelNames = new[] { "queue" },
//             Buckets = Histogram.ExponentialBuckets(0.01, 2.0, 15)
//         });

//     public static readonly Gauge Inflight = Metrics.CreateGauge(
//         "rabbit_consumer_inflight", "Current in-flight messages being processed");

//     public static readonly Counter Reconnects = Metrics.CreateCounter(
//         "rabbit_consumer_reconnects_total", "Number of reconnect attempts");

//     public static readonly Gauge ConnectionUp = Metrics.CreateGauge(
//         "rabbit_consumer_connection_up", "1 if connected to RabbitMQ, otherwise 0");
// }