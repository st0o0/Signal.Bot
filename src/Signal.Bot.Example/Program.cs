using Serilog;
using Signal.Bot;
using Signal.Bot.Example;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((_, config) => config.WriteTo.Console());
Console.WriteLine($"Phone {Environment.GetEnvironmentVariable("NUMBER")!}");
builder.Services.AddSingleton<IReceivedMessageHandler, TestHandler>();
builder.Services
    .AddHttpClient("signalbot_client", client => client.BaseAddress = new Uri("http://localhost:1337"))
    .AddTypedClient<ISignalBotClient>((httpClient, _) =>
        new SignalBotClient(optionsBuilder =>
            optionsBuilder
                .WithBaseUrl("http://localhost:1337")
                .WithNumber(Environment.GetEnvironmentVariable("NUMBER")!)
                .WithHttpClient(httpClient)))
    .AddStandardResilienceHandler(options =>
    {
        // Retry
        options.Retry.MaxRetryAttempts = 3;
        options.Retry.Delay = TimeSpan.FromMilliseconds(200);

        // Circuit Breaker
        options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
        options.CircuitBreaker.FailureRatio = 0.5;
        options.CircuitBreaker.MinimumThroughput = 10;

        // Timeout (per request)
        options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(10);
    });
builder.Services.AddHostedService<ForTesting>();

var app = builder.Build();

await app.RunAsync();