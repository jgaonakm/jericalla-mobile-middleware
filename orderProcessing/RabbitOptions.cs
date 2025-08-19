public sealed class RabbitOptions
{
    public string Host { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string VirtualHost { get; set; } = "/";
    public string User { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string QueueName { get; set; } = "phone-reservation";
    public ushort Prefetch { get; set; } = 32;
}