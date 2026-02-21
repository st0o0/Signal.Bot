# Echo Bot Example

A simple bot that echoes back any message it receives.

## Overview

This is the most basic example of a Signal bot. It demonstrates:
- Receiving messages
- Sending responses
- Basic error handling
- Graceful shutdown

## Code

<<< ./../../src/Signal.Bot.Example/Examples/EchoBot.cs#EchoBot{csharp}

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