# Examples

Explore practical examples of what you can build with Signal.Bot.

## Basic Examples

### Echo Bot

The simplest bot - echoes back everything you send it.

[View Echo Bot Example →](/examples/echo-bot)

### Command Bot

A bot that responds to commands like `/help`, `/time`, `/weather`.

[View Command Bot Example →](/examples/command-bot)

## Intermediate Examples
<!---
### Group Manager Bot

Manage Signal groups with commands to add/remove members, update settings, and more.

[View Group Manager Bot Example →](/examples/group-manager)

### File Processor Bot

Process different types of attachments - images, documents, videos.

[View File Processor Bot Example →](/examples/file-processor)

### Reminder Bot

Set reminders and get notifications at scheduled times.

[View Reminder Bot Example →](/examples/reminder-bot)

## Advanced Examples

### Multi-User Chat Bot

Handle conversations with multiple users simultaneously with context awareness.

[View Multi-User Chat Bot Example →](/examples/multi-user-chat)

### Integration Bot

Integrate with external APIs and services (weather, news, etc.).

[View Integration Bot Example →](/examples/integration-bot)

### Database-Backed Bot

Use Entity Framework Core to persist data and user preferences.

[View Database Bot Example →](/examples/database-bot)

## Use Case Examples

### Customer Support Bot

Automated customer support with ticket creation and FAQ responses.

[View Support Bot Example →](/examples/support-bot)

### Notification Bot

Send system alerts and notifications to users or groups.

[View Notification Bot Example →](/examples/notification-bot)

### Poll Bot

Create and manage polls in Signal groups.

[View Poll Bot Example →](/examples/poll-bot)
-->
## Quick Snippets

### Send a Message

```csharp
var client = new SignalBotClient("http://localhost:8080");
await client.SendMessageAsync(
    number: "+1234567890",
    message: "Hello!",
    recipients: new[] { "+0987654321" }
);
```

### Receive Messages

```csharp
await client.StartReceiving(
    "+1234567890",
    handleMessage: async (client, message, ct) =>
    {
        Console.WriteLine($"Received: {message.DataMessage?.Message}");
    },
    handleError: async (client, ex, ct) =>
    {
        Console.WriteLine($"Error: {ex.Message}");
    },
    cancellationToken: CancellationToken.None
);
```

### Create a Group

```csharp
var group = await client.CreateGroupAsync(
    number: "+1234567890",
    name: "My Group",
    members: new[] { "+1111111111", "+2222222222" }
);
```

### Send Attachment

```csharp
await client.SendMessageAsync(
    number: "+1234567890",
    message: "Check this out!",
    recipients: new[] { "+0987654321" },
    attachments: new[] { "/path/to/file.jpg" }
);
```

## Running the Examples

All examples are available in the [GitHub repository](https://github.com/st0o0/Signal.Bot/tree/main/src/examples).

To run an example:

1. Clone the repository
2. Navigate to the example directory
3. Update the configuration with your Signal number
4. Run with `dotnet run`

```bash
git clone https://github.com/st0o0/Signal.Bot.git
cd Signal.Bot/examples/echo-bot
dotnet run
```

## Contributing Examples

Have a cool bot you've built with Signal.Bot? We'd love to feature it!

1. Fork the repository
2. Add your example to the `examples/` directory
3. Include a README with setup instructions
4. Submit a pull request

## Need Help?

If you're stuck on any example or have questions:

- Open an [issue on GitHub](https://github.com/st0o0/Signal.Bot/issues)
- Review the [guides](/guide/getting-started)