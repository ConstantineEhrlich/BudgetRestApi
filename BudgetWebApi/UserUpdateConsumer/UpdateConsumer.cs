using System.Text;
using System.Text.Json;
using BudgetServices;
using BudgetWebApi.Dto;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace BudgetWebApi.UserUpdateConsumer;

public class UpdateConsumer: BackgroundService
{
    private readonly IServiceProvider _provider;
    private readonly string _name;
    private readonly IModel _channel;
    private readonly ILogger<UpdateConsumer> _logger;

    public UpdateConsumer(IOptions<UpdateConsumerSettings> settings, ILogger<UpdateConsumer> logger, IServiceProvider provider)
    {
        _provider = provider;
        _logger = logger;
        ConnectionFactory factory = new();
        factory.UserName = settings.Value.UserName;
        factory.Password = settings.Value.Password;
        factory.HostName = settings.Value.HostName;
        factory.VirtualHost = settings.Value.VirtualHost;
        factory.Port = settings.Value.Port ?? 5555;
        IConnection connection = factory.CreateConnection();
        _channel = connection.CreateModel();
        _name = settings.Value.QueueName;
        _channel.QueueDeclare(queue: _name,
            durable: true,
            exclusive:false,
            autoDelete:false,
            arguments:null);
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        EventingBasicConsumer consumer = new(_channel);
        consumer.Received += async (_, ea) =>
        {
            string message = Encoding.UTF8.GetString(ea.Body.ToArray());
            UserDto? userInfo = JsonSerializer.Deserialize<UserDto>(message);
            await CreateUserAsync(userInfo!, ea.DeliveryTag);
        };
        _channel.BasicConsume(queue: _name, autoAck: false, consumer: consumer);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private async Task CreateUserAsync(UserDto userInfo, ulong deliveryTag)
    {
        using IServiceScope scope = _provider.CreateScope();
        UserService users = scope.ServiceProvider.GetRequiredService<UserService>();
        try
        {
            _logger.LogInformation("User created: {}", userInfo.Id);
            await users.CreateUserAsync(userInfo.Id!, userInfo.Name!, userInfo.Email!);
            _channel.BasicAck(deliveryTag, false);
        }
        catch (Exception e)
        {
            _logger.LogError("Unexpected error occured: {}", e.Message);
        }
    }
}