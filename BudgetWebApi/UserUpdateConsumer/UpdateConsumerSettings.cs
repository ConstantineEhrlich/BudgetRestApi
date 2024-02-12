namespace BudgetWebApi.UserUpdateConsumer;

public class UpdateConsumerSettings
{
    public string UserName { get; set; } = null!;
    public string HostName { get; set; } = null!;
    public int? Port { get; set; }

    public string QueueName { get; set; } = null!;

    public string Password => Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ??
                              throw new KeyNotFoundException("RABBITMQ_PASSWORD variable not set");
}