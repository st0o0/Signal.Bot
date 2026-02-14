# Echo Bot Example

A simple bot that echoes back any message it receives.

## Overview

This is the most basic example of a Signal bot. It demonstrates:
- Receiving messages
- Sending responses
- Basic error handling
- Graceful shutdown

## Code

```csharp
using Signal.Bot;
using Signal.Bot.Types;

class Program
{
    static async Task Main(string[] args)
    {
        // Configuration
        var apiUrl = "http://localhost:8080";
        var botNumber = "+1234567890"; // Replace with your Signal number

        // Create client
        var client = new SignalBotClient(apiUrl);

        // Cancellation token for graceful shutdown
        using var cts = new CancellationTokenSource();
        
        // Handle Ctrl+C
        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("\nStopping bot...");
            e.Cancel = true;
            cts.Cancel();
        };

        Console.WriteLine("Echo Bot is starting...");
        Console.WriteLine($"Bot number: {botNumber}");
        Console.WriteLine("Press Ctrl+C to stop\n");

        // Start receiving messages
        await client.StartReceiving(
            botNumber,
            handleMessage: HandleMessage,
            handleError: HandleError,
            cancellationToken: cts.Token
        );

        Console.WriteLine("Bot stopped");
    }

    static async Task HandleMessage(
        SignalBotClient client,
        SignalMessage message,
        CancellationToken cancellationToken)
    {
        // Get the text message
        var text = message.DataMessage?.Message;
        
        // Ignore empty messages
        if (string.IsNullOrWhiteSpace(text))
            return;

        // Get sender
        var sender = message.Source;
        
        // Log received message
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] From {sender}: {text}");

        // Echo back with a prefix
        var echoMessage = $"You said: {text}";

        try
        {
            // Send the echo
            await client.SendMessageAsync(
                number: "+1234567890", // Your bot number
                message: echoMessage,
                recipients: new[] { sender },
                cancellationToken: cancellationToken
            );

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Echoed back to {sender}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending echo: {ex.Message}");
        }
    }

    static async Task HandleError(
        SignalBotClient client,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Console.WriteLine($"[ERROR] {exception.GetType().Name}: {exception.Message}");
        
        // Wait a bit before continuing to avoid tight error loops
        await Task.Delay(1000, cancellationToken);
    }
}
```

## Configuration

Before running, update these values:

```csharp
var apiUrl = "http://localhost:8080";  // Your signal-cli-rest-api URL
var botNumber = "+1234567890";         // Your registered Signal number
```

## Running the Bot

1. Make sure signal-cli-rest-api is running:
```bash
docker ps | grep signal-api
```

2. Run the bot:
```bash
dotnet run
```

3. Send a message to your bot's number from another Signal account

4. The bot will echo back your message with "You said: " prefix

## Expected Output

```
Echo Bot is starting...
Bot number: +1234567890
Press Ctrl+C to stop

[14:23:15] From +0987654321: Hello!
[14:23:15] Echoed back to +0987654321
[14:23:20] From +0987654321: How are you?
[14:23:20] Echoed back to +0987654321
```

## Enhancements

### 1. Ignore Bot's Own Messages

```csharp
static async Task HandleMessage(
    SignalBotClient client,
    SignalMessage message,
    CancellationToken cancellationToken)
{
    var botNumber = "+1234567890";
    
    // Ignore messages from the bot itself
    if (message.Source == botNumber)
        return;
    
    // ... rest of handler
}
```

### 2. Add Typing Indicator

```csharp
// Show typing before replying
await client.SendTypingIndicatorAsync(
    number: botNumber,
    recipient: sender,
    cancellationToken: cancellationToken
);

// Simulate thinking time
await Task.Delay(500, cancellationToken);

// Send the echo
await client.SendMessageAsync(...);
```

### 3. Handle Group Messages

```csharp
static async Task HandleMessage(
    SignalBotClient client,
    SignalMessage message,
    CancellationToken cancellationToken)
{
    var text = message.DataMessage?.Message;
    if (string.IsNullOrWhiteSpace(text)) return;

    var sender = message.Source;
    var groupId = message.DataMessage?.GroupId;

    // Handle group messages
    if (!string.IsNullOrEmpty(groupId))
    {
        await client.SendMessageAsync(
            number: botNumber,
            message: $"@{sender} said: {text}",
            groupId: groupId,
            cancellationToken: cancellationToken
        );
    }
    else
    {
        // Handle direct messages
        await client.SendMessageAsync(
            number: botNumber,
            message: $"You said: {text}",
            recipients: new[] { sender },
            cancellationToken: cancellationToken
        );
    }
}
```

### 4. Add Rate Limiting

```csharp
private static readonly Dictionary<string, DateTime> _lastMessageTime = new();

static async Task HandleMessage(
    SignalBotClient client,
    SignalMessage message,
    CancellationToken cancellationToken)
{
    var sender = message.Source;
    
    // Rate limit: max 1 message per 2 seconds per user
    if (_lastMessageTime.TryGetValue(sender, out var lastTime))
    {
        if (DateTime.Now - lastTime < TimeSpan.FromSeconds(2))
        {
            Console.WriteLine($"Rate limited: {sender}");
            return;
        }
    }
    
    _lastMessageTime[sender] = DateTime.Now;
    
    // ... rest of handler
}
```

### 5. Add Logging

```csharp
using Microsoft.Extensions.Logging;

class Program
{
    private static ILogger<Program> _logger;

    static async Task Main(string[] args)
    {
        // Setup logging
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder
                .AddConsole()
                .SetMinimumLevel(LogLevel.Information);
        });
        
        _logger = loggerFactory.CreateLogger<Program>();

        _logger.LogInformation("Echo Bot starting...");
        
        // ... rest of code
    }

    static async Task HandleMessage(
        SignalBotClient client,
        SignalMessage message,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received message from {Sender}: {Message}",
            message.Source,
            message.DataMessage?.Message
        );
        
        // ... rest of handler
    }
}
```

## Troubleshooting

**Bot not receiving messages?**
- Check signal-cli-rest-api is running: `curl http://localhost:8080/v1/about`
- Verify your number is registered
- Check console for errors

**Bot not responding?**
- Check the bot number in `SendMessageAsync` matches your registered number
- Verify network connectivity to signal-cli-rest-api
- Check signal-cli-rest-api logs: `docker logs signal-api`

**Messages received multiple times?**
- Normal behavior - messages stay in queue until acknowledged
- They'll stop appearing after being processed

## Next Steps

- Add command handling: [Command Bot Example](/examples/command-bot)
<!---
- Work with groups: [Group Manager Bot](/examples/group-manager)
- Process attachments: [File Processor Bot](/examples/file-processor)
-->