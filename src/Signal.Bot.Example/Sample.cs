using Signal.Bot.Polling;
using Signal.Bot.Types;

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
        var handler = _serviceProvider.GetRequiredService<IReceivedMessageHandler>();
        var logger = _serviceProvider.GetRequiredService<ILogger<Sample>>();

        client.OnException.Subscribe(ex => logger.LogError(ex, "ERROR"));

        client.OnApiRequest.Subscribe(request => logger.LogInformation("API Request: {@Request}", request));

        client.OnApiResponse.Subscribe(response => logger.LogInformation("API Response: {@Response}", response));
        client.StartReceiving(handler,
            builder => builder
                .WithIgnoreAttachments(true)
                .WithIgnoreStories(true)
                .WithTimeout(TimeSpan.MaxValue),
            stoppingToken);

        // var t1 = await client.GetAboutAsync(stoppingToken);
        // var t2 = await client.GetAccountsAsync(stoppingToken);
        // var t4 = await client.GetContactsAsync(stoppingToken);
        // var t3 = await client.GetDevicesAsync(stoppingToken);
        // var t5 = await client.GetGroupsAsync(stoppingToken);
        // var t6 = await client.GetIdentitiesAsync(stoppingToken);
        // var t7 = (await client.GetAttachmentsAsync(stoppingToken)).ToArray();
        // var t8 = await client.GetAttachmentAsync(t7[0], stoppingToken);
        var t9 = await client.GetStickerPacksAsync(cancellationToken: stoppingToken);

        // TEST SEND TO ME
        // await client.SendMessageAsync(builder =>
        // {
        //     var number = Environment.GetEnvironmentVariable("NUMBER")!;
        //     builder
        //         .WithMessage("TEST 12345")
        //         .WithRecipient(number)
        //         .WithNumber(number)
        //         .WithNotifySelf();
        // }, stoppingToken);

        await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
    }
}

public class TestHandler : IReceivedMessageHandler
{
    private readonly ILogger<TestHandler> _logger;

    public TestHandler(ILogger<TestHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(ISignalBotClient client, ReceivedMessage message, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received message: {@Message}", message);
        return Task.CompletedTask;
    }

    public Task HandleErrorAsync(ISignalBotClient client, Error error, CancellationToken cancellationToken)
    {
        _logger.LogError(error.Exception, "Error: {@Error}", error);
        return Task.CompletedTask;
    }
}