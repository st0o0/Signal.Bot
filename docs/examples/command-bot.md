# Command Bot Example

A bot that responds to slash commands like `/help`, `/time`, `/echo`, and more.

## Overview

This example demonstrates:
- Command parsing
- Command routing
- Help system
- State management
- Error messages

## Code

<<< ./../../src/Signal.Bot.Example/DocuCode/EchoBot.cs#EchoBot{csharp}

## Usage Examples

**User:** `/help`  
**Bot:** Shows list of available commands

**User:** `/time`  
**Bot:** 🕐 Current time: 14:25:30

**User:** `/echo Hello World`  
**Bot:** Hello World

**User:** `/ping`  
**Bot:** 🏓 Pong!

**User:** `/unknown`  
**Bot:** ❓ Unknown command: /unknown. Send /help to see available commands.

## Advanced Features

### 1. Admin-Only Commands

```csharp
private static readonly HashSet<string> _adminNumbers = new()
{
    "+9999999999"
};

static async Task HandleMessage(
    SignalBotClient client,
    SignalMessage message,
    CancellationToken cancellationToken)
{
    var text = message.DataMessage?.Message;
    if (string.IsNullOrWhiteSpace(text) || !text.StartsWith("/")) return;

    var sender = message.Source;
    var command = text.Split(' ')[0].ToLower();

    // Check admin commands
    if (command.StartsWith("/admin"))
    {
        if (!_adminNumbers.Contains(sender))
        {
            await SendMessage(sender, "🔒 Access denied: Admin only", cancellationToken);
            return;
        }
        
        await HandleAdminCommand(sender, text, cancellationToken);
        return;
    }

    // ... regular command handling
}

static async Task HandleAdminCommand(string sender, string text, CancellationToken ct)
{
    var parts = text.Split(' ', 2);
    var command = parts[0].ToLower();

    switch (command)
    {
        case "/adminstats":
            await SendAdminStats(sender, ct);
            break;
        case "/adminbroadcast":
            if (parts.Length > 1)
                await BroadcastMessage(parts[1], ct);
            break;
    }
}
```

### 2. Command Cooldowns

```csharp
private static readonly Dictionary<string, Dictionary<string, DateTime>> _commandCooldowns = new();

static bool IsOnCooldown(string sender, string command, int cooldownSeconds)
{
    if (!_commandCooldowns.ContainsKey(command))
        _commandCooldowns[command] = new Dictionary<string, DateTime>();

    if (_commandCooldowns[command].TryGetValue(sender, out var lastUse))
    {
        var remaining = (lastUse.AddSeconds(cooldownSeconds) - DateTime.Now).TotalSeconds;
        if (remaining > 0)
        {
            return true;
        }
    }

    _commandCooldowns[command][sender] = DateTime.Now;
    return false;
}

// Usage
static async Task HandleExpensiveCommand(string sender, CancellationToken ct)
{
    if (IsOnCooldown(sender, "/expensive", 60))
    {
        await SendMessage(sender, "⏳ Please wait before using this command again", ct);
        return;
    }

    // Process command
    await SendMessage(sender, "✅ Command executed", ct);
}
```
<!---
## Next Steps

- Add persistence: [Database Bot Example](/examples/database-bot)
- Manage groups: [Group Manager Bot](/examples/group-manager)
- External APIs: [Integration Bot](/examples/integration-bot)
-->