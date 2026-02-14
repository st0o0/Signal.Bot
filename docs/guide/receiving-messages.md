# Receiving Messages

Signal.Bot provides a convenient `StartReceiving` method that handles all the complexity of polling for new messages.

## Basic Message Receiving

The simplest way to start receiving messages:

```csharp
var client = new SignalBotClient("http://localhost:8080");
var botNumber = "+1234567890";

using var cts = new CancellationTokenSource();

await client.StartReceiving(
    botNumber,
    handleMessage: async (botClient, message, ct) =>
    {
        Console.WriteLine($"Received: {message.DataMessage?.Message}");
    },
    handleError: async (botClient, exception, ct) =>
    {
        Console.WriteLine($"Error: {exception.Message}");
    },
    cancellationToken: cts.Token
);

// Keep running
Console.ReadKey();
cts.Cancel();
```

## Message Structure

Messages in Signal.Bot follow this structure:

```csharp
public class SignalMessage
{
    public Envelope Envelope { get; set; }
    public string Source { get; set; }         // Sender's phone number
    public DataMessage DataMessage { get; set; } // Contains the actual message
    public long Timestamp { get; set; }
    // ... other properties
}

public class DataMessage
{
    public string Message { get; set; }        // Text content
    public List<Attachment> Attachments { get; set; }
    public string GroupId { get; set; }
    public List<Mention> Mentions { get; set; }
    public Quote Quote { get; set; }           // If replying to a message
    // ... other properties
}
```

## Handling Different Message Types

### Text Messages

```csharp
await client.StartReceiving(
    botNumber,
    handleMessage: async (botClient, message, ct) =>
    {
        if (message.DataMessage?.Message != null)
        {
            var text = message.DataMessage.Message;
            var sender = message.Source;
            
            Console.WriteLine($"{sender}: {text}");
        }
    },
    handleError: HandleError,
    cancellationToken: cts.Token
);
```

### Messages with Attachments

```csharp
await client.StartReceiving(
    botNumber,
    handleMessage: async (botClient, message, ct) =>
    {
        if (message.DataMessage?.Attachments?.Any() == true)
        {
            foreach (var attachment in message.DataMessage.Attachments)
            {
                Console.WriteLine($"Received attachment: {attachment.ContentType}");
                Console.WriteLine($"Filename: {attachment.Filename}");
                Console.WriteLine($"ID: {attachment.Id}");
            }
        }
    },
    handleError: HandleError,
    cancellationToken: cts.Token
);
```

### Group Messages

```csharp
await client.StartReceiving(
    botNumber,
    handleMessage: async (botClient, message, ct) =>
    {
        if (!string.IsNullOrEmpty(message.DataMessage?.GroupId))
        {
            Console.WriteLine($"Group message in: {message.DataMessage.GroupId}");
            Console.WriteLine($"From: {message.Source}");
            Console.WriteLine($"Text: {message.DataMessage.Message}");
        }
    },
    handleError: HandleError,
    cancellationToken: cts.Token
);
```

### Reactions

```csharp
await client.StartReceiving(
    botNumber,
    handleMessage: async (botClient, message, ct) =>
    {
        if (message.DataMessage?.Reaction != null)
        {
            var reaction = message.DataMessage.Reaction;
            Console.WriteLine($"{message.Source} reacted with {reaction.Emoji}");
            Console.WriteLine($"To message from: {reaction.TargetAuthor}");
        }
    },
    handleError: HandleError,
    cancellationToken: cts.Token
);
```

## Command Handling

Build a simple command system:

```csharp
await client.StartReceiving(
    botNumber,
    handleMessage: async (botClient, message, ct) =>
    {
        var text = message.DataMessage?.Message;
        if (string.IsNullOrEmpty(text)) return;
        
        var sender = message.Source;
        
        // Parse command
        if (text.StartsWith("/"))
        {
            var parts = text.Split(' ', 2);
            var command = parts[0].ToLower();
            var args = parts.Length > 1 ? parts[1] : string.Empty;
            
            switch (command)
            {
                case "/start":
                    await botClient.SendMessageAsync(
                        number: botNumber,
                        message: "Welcome! Use /help to see available commands.",
                        recipients: new[] { sender },
                        cancellationToken: ct
                    );
                    break;
                    
                case "/help":
                    await botClient.SendMessageAsync(
                        number: botNumber,
                        message: @"Available commands:
/start - Start the bot
/help - Show this message
/time - Get current time
/echo <text> - Echo back your message",
                        recipients: new[] { sender },
                        cancellationToken: ct
                    );
                    break;
                    
                case "/time":
                    await botClient.SendMessageAsync(
                        number: botNumber,
                        message: $"Current time: {DateTime.Now:HH:mm:ss}",
                        recipients: new[] { sender },
                        cancellationToken: ct
                    );
                    break;
                    
                case "/echo":
                    if (!string.IsNullOrEmpty(args))
                    {
                        await botClient.SendMessageAsync(
                            number: botNumber,
                            message: args,
                            recipients: new[] { sender },
                            cancellationToken: ct
                        );
                    }
                    break;
            }
        }
    },
    handleError: HandleError,
    cancellationToken: cts.Token
);
```

## Advanced Pattern: Message Router

Create a more structured message handling system:

```csharp
public class MessageRouter
{
    private readonly SignalBotClient _client;
    private readonly string _botNumber;
    private readonly Dictionary<string, Func<SignalBotClient, SignalMessage, CancellationToken, Task>> _handlers;

    public MessageRouter(SignalBotClient client, string botNumber)
    {
        _client = client;
        _botNumber = botNumber;
        _handlers = new Dictionary<string, Func<SignalBotClient, SignalMessage, CancellationToken, Task>>();
    }

    public void OnCommand(string command, Func<SignalBotClient, SignalMessage, CancellationToken, Task> handler)
    {
        _handlers[command.ToLower()] = handler;
    }

    public async Task HandleMessage(SignalBotClient botClient, SignalMessage message, CancellationToken ct)
    {
        var text = message.DataMessage?.Message;
        if (string.IsNullOrEmpty(text) || !text.StartsWith("/")) return;

        var command = text.Split(' ')[0].ToLower();
        
        if (_handlers.TryGetValue(command, out var handler))
        {
            await handler(botClient, message, ct);
        }
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        await _client.StartReceiving(
            _botNumber,
            handleMessage: HandleMessage,
            handleError: async (client, ex, ct) => Console.WriteLine($"Error: {ex.Message}"),
            cancellationToken: cancellationToken
        );
    }
}

// Usage
var router = new MessageRouter(client, botNumber);

router.OnCommand("/start", async (client, message, ct) =>
{
    await client.SendMessageAsync(
        number: botNumber,
        message: "Welcome!",
        recipients: new[] { message.Source },
        cancellationToken: ct
    );
});

router.OnCommand("/help", async (client, message, ct) =>
{
    await client.SendMessageAsync(
        number: botNumber,
        message: "Available commands: /start, /help",
        recipients: new[] { message.Source },
        cancellationToken: ct
    );
});

await router.Start(cts.Token);
```

## Filtering Messages

### Ignore Bot Messages

```csharp
await client.StartReceiving(
    botNumber,
    handleMessage: async (botClient, message, ct) =>
    {
        // Ignore messages from the bot itself
        if (message.Source == botNumber) return;
        
        // Process message...
    },
    handleError: HandleError,
    cancellationToken: cts.Token
);
```

### Group-Only Messages

```csharp
await client.StartReceiving(
    botNumber,
    handleMessage: async (botClient, message, ct) =>
    {
        // Only handle group messages
        if (string.IsNullOrEmpty(message.DataMessage?.GroupId)) return;
        
        // Process group message...
    },
    handleError: HandleError,
    cancellationToken: cts.Token
);
```

### Private Messages Only

```csharp
await client.StartReceiving(
    botNumber,
    handleMessage: async (botClient, message, ct) =>
    {
        // Only handle private messages
        if (!string.IsNullOrEmpty(message.DataMessage?.GroupId)) return;
        
        // Process private message...
    },
    handleError: HandleError,
    cancellationToken: cts.Token
);
```

## Error Handling

Properly handle errors during message processing:

```csharp
async Task HandleError(SignalBotClient client, Exception exception, CancellationToken ct)
{
    Console.WriteLine($"Error occurred: {exception.GetType().Name}");
    Console.WriteLine($"Message: {exception.Message}");
    
    if (exception is HttpRequestException httpEx)
    {
        Console.WriteLine("Network error - check signal-cli-rest-api connection");
    }
    else if (exception is TaskCanceledException)
    {
        Console.WriteLine("Operation was cancelled");
    }
    else
    {
        Console.WriteLine($"Stack trace: {exception.StackTrace}");
    }
    
    // Optionally notify admin
    // await NotifyAdmin(exception.Message);
}
```

## Polling Configuration

The `StartReceiving` method polls for new messages at regular intervals. While the default settings work for most cases, you can customize the polling behavior if needed.

::: tip
The current implementation uses a simple polling mechanism. For high-traffic bots, consider implementing exponential backoff or rate limiting.
:::

## Best Practices

### 1. Always Use Cancellation Tokens

```csharp
var cts = new CancellationTokenSource();

// Handle Ctrl+C gracefully
Console.CancelKeyPress += (sender, e) =>
{
    e.Cancel = true;
    cts.Cancel();
};

await client.StartReceiving(..., cancellationToken: cts.Token);
```

### 2. Separate Concerns

```csharp
// ✅ Good - separate message handling logic
async Task HandleTextMessage(SignalBotClient client, SignalMessage message, CancellationToken ct)
{
    // Handle text message
}

async Task HandleAttachment(SignalBotClient client, SignalMessage message, CancellationToken ct)
{
    // Handle attachment
}

// ❌ Bad - everything in one handler
await client.StartReceiving(..., handleMessage: async (c, m, ct) => {
    // 500 lines of code here
});
```

### 3. Log Everything Important

```csharp
await client.StartReceiving(
    botNumber,
    handleMessage: async (botClient, message, ct) =>
    {
        _logger.LogInformation(
            "Received message from {Sender} at {Time}",
            message.Source,
            DateTimeOffset.FromUnixTimeMilliseconds(message.Timestamp)
        );
        
        // Process message...
    },
    handleError: async (botClient, exception, ct) =>
    {
        _logger.LogError(exception, "Error processing message");
    },
    cancellationToken: cts.Token
);
```

## Next Steps

- Learn about [working with groups](/guide/groups)
- Explore [attachment handling](/guide/attachments)
- Check out [complete examples](/examples/)