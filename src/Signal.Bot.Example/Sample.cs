namespace Signal.Bot.Example;

public class Sample : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public Sample(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var client = _serviceProvider.GetRequiredService<ISignalBotClient>();
        var logger = _serviceProvider.GetRequiredService<ILogger<Sample>>();

        client.OnException.Subscribe(ex => logger.LogError(ex, "ERROR"));

        client.OnApiRequest.Subscribe(request => logger.LogInformation("API Request: {@Request}", request));

        client.OnApiResponse.Subscribe(response => logger.LogInformation("API Response: {@Response}", response));

        //var t1 = await client.GetAboutAsync(stoppingToken);
        //var t2 = await client.GetAccountsAsync(stoppingToken);
        //var t4 = await client.GetContactsAsync(stoppingToken);
        var t3 = await client.GetDevicesAsync(stoppingToken);
        foreach (var device in t3)
        {
            var created = DateTimeOffset.FromUnixTimeMilliseconds(device.Created ?? 0);
            logger.LogInformation("Created: {DateTimeOffset}", created.UtcDateTime);
            var lastSeen = DateTimeOffset.FromUnixTimeMilliseconds(device.LastSeen ?? 0);
            logger.LogInformation("LastSeen: {DateTimeOffset}", lastSeen.UtcDateTime);
        }
    }
}