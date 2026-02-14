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

```csharp
using Signal.Bot;
using Signal.Bot.Types;

class Program
{
    private static SignalBotClient _client;
    private static string _botNumber;

    static async Task Main(string[] args)
    {
        var apiUrl = "http://localhost:8080";
        _botNumber = "+1234567890";

        _client = new SignalBotClient(apiUrl);

        using var cts = new CancellationTokenSource();
        
        Console.CancelKeyPress += (sender, e) =>
        {
            Console.WriteLine("\nStopping bot...");
            e.Cancel = true;
            cts.Cancel();
        };

        Console.WriteLine("Command Bot is running...");
        Console.WriteLine("Available commands: /help, /time, /echo, /ping, /info");
        Console.WriteLine("Press Ctrl+C to stop\n");

        await _client.StartReceiving(
            _botNumber,
            handleMessage: HandleMessage,
            handleError: HandleError,
            cancellationToken: cts.Token
        );
    }

    static async Task HandleMessage(
        SignalBotClient client,
        SignalMessage message,
        CancellationToken cancellationToken)
    {
        var text = message.DataMessage?.Message;
        if (string.IsNullOrWhiteSpace(text)) return;

        var sender = message.Source;

        // Only process commands (messages starting with /)
        if (!text.StartsWith("/"))
        {
            // Optionally send a hint
            await SendMessage(
                sender,
                "Send /help to see available commands",
                cancellationToken
            );
            return;
        }

        // Parse command and arguments
        var parts = text.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
        var command = parts[0].ToLower();
        var args = parts.Length > 1 ? parts[1] : string.Empty;

        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] Command from {sender}: {command}");

        // Route to appropriate handler
        await (command switch
        {
            "/start" => HandleStart(sender, cancellationToken),
            "/help" => HandleHelp(sender, cancellationToken),
            "/time" => HandleTime(sender, cancellationToken),
            "/date" => HandleDate(sender, cancellationToken),
            "/echo" => HandleEcho(sender, args, cancellationToken),
            "/ping" => HandlePing(sender, cancellationToken),
            "/info" => HandleInfo(sender, cancellationToken),
            "/uptime" => HandleUptime(sender, cancellationToken),
            _ => HandleUnknownCommand(sender, command, cancellationToken)
        });
    }

    static async Task HandleStart(string sender, CancellationToken ct)
    {
        var message = @"👋 Welcome to Command Bot!

I'm a bot that responds to commands. Send /help to see what I can do!";

        await SendMessage(sender, message, ct);
    }

    static async Task HandleHelp(string sender, CancellationToken ct)
    {
        var message = @"🤖 Available Commands

📍 Basic:
/start - Welcome message
/help - Show this message
/ping - Check if bot is alive

⏰ Time & Date:
/time - Get current time
/date - Get current date
/uptime - Bot uptime

💬 Utilities:
/echo <text> - Echo back your message
/info - Bot information";

        await SendMessage(sender, message, ct);
    }

    static async Task HandleTime(string sender, CancellationToken ct)
    {
        var message = $"🕐 Current time: {DateTime.Now:HH:mm:ss}";
        await SendMessage(sender, message, ct);
    }

    static async Task HandleDate(string sender, CancellationToken ct)
    {
        var message = $"📅 Today is: {DateTime.Now:dddd, MMMM dd, yyyy}";
        await SendMessage(sender, message, ct);
    }

    static async Task HandleEcho(string sender, string args, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(args))
        {
            await SendMessage(sender, "❌ Usage: /echo <text>", ct);
            return;
        }

        await SendMessage(sender, args, ct);
    }

    static async Task HandlePing(string sender, CancellationToken ct)
    {
        var startTime = DateTime.Now;
        
        await SendMessage(sender, "🏓 Pong!", ct);
        
        var latency = (DateTime.Now - startTime).TotalMilliseconds;
        Console.WriteLine($"Ping latency: {latency:F0}ms");
    }

    static async Task HandleInfo(string sender, CancellationToken ct)
    {
        var message = $@"ℹ️ Bot Information

📱 Number: {_botNumber}
🤖 Framework: Signal.Bot
⚙️ Runtime: .NET {Environment.Version}
💻 Platform: {Environment.OSVersion}
📊 Memory: {GC.GetTotalMemory(false) / 1024 / 1024} MB";

        await SendMessage(sender, message, ct);
    }

    private static DateTime _startTime = DateTime.Now;

    static async Task HandleUptime(string sender, CancellationToken ct)
    {
        var uptime = DateTime.Now - _startTime;
        var message = $"⏱ Bot uptime: {uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
        
        await SendMessage(sender, message, ct);
    }

    static async Task HandleUnknownCommand(string sender, string command, CancellationToken ct)
    {
        var message = $"❓ Unknown command: {command}\n\nSend /help to see available commands.";
        await SendMessage(sender, message, ct);
    }

    static async Task SendMessage(string recipient, string message, CancellationToken ct)
    {
        try
        {
            await _client.SendMessageAsync(
                number: _botNumber,
                message: message,
                recipients: new[] { recipient },
                cancellationToken: ct
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error sending message: {ex.Message}");
        }
    }

    static async Task HandleError(
        SignalBotClient client,
        Exception exception,
        CancellationToken ct)
    {
        Console.WriteLine($"[ERROR] {exception.Message}");
        await Task.Delay(1000, ct);
    }
}
```

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

### 1. Command with Multiple Arguments

```csharp
static async Task HandleCalculate(string sender, string args, CancellationToken ct)
{
    // Usage: /calc 5 + 3
    var parts = args.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    
    if (parts.Length != 3)
    {
        await SendMessage(sender, "❌ Usage: /calc <number> <op> <number>", ct);
        return;
    }

    if (!double.TryParse(parts[0], out var num1) || 
        !double.TryParse(parts[2], out var num2))
    {
        await SendMessage(sender, "❌ Invalid numbers", ct);
        return;
    }

    var result = parts[1] switch
    {
        "+" => num1 + num2,
        "-" => num1 - num2,
        "*" => num1 * num2,
        "/" => num2 != 0 ? num1 / num2 : double.NaN,
        _ => double.NaN
    };

    if (double.IsNaN(result))
    {
        await SendMessage(sender, "❌ Invalid operation or division by zero", ct);
    }
    else
    {
        await SendMessage(sender, $"🔢 Result: {result}", ct);
    }
}
```

### 2. Admin-Only Commands

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

### 3. Command Cooldowns

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

### 4. Command Aliases

```csharp
private static readonly Dictionary<string, string> _aliases = new()
{
    ["/h"] = "/help",
    ["/t"] = "/time",
    ["/d"] = "/date",
    ["/p"] = "/ping",
    ["/e"] = "/echo"
};

static string ResolveAlias(string command)
{
    return _aliases.TryGetValue(command, out var actual) ? actual : command;
}

// Usage in HandleMessage
var command = ResolveAlias(parts[0].ToLower());
```

### 5. Command Statistics

```csharp
private static readonly Dictionary<string, int> _commandStats = new();

static void TrackCommand(string command)
{
    if (!_commandStats.ContainsKey(command))
        _commandStats[command] = 0;
    
    _commandStats[command]++;
}

static async Task HandleStats(string sender, CancellationToken ct)
{
    var stats = string.Join("\n", 
        _commandStats
            .OrderByDescending(x => x.Value)
            .Select(x => $"{x.Key}: {x.Value} uses"));

    var message = $"📊 Command Statistics\n\n{stats}";
    await SendMessage(sender, message, ct);
}

// Track in main handler
TrackCommand(command);
```

## Configuration File

Create `config.json`:

```json
{
  "ApiUrl": "http://localhost:8080",
  "BotNumber": "+1234567890",
  "AdminNumbers": [
    "+9999999999"
  ],
  "Commands": {
    "Cooldowns": {
      "/expensive": 60,
      "/api": 30
    }
  }
}
```

Load configuration:

```csharp
using System.Text.Json;

public class BotConfig
{
    public string ApiUrl { get; set; }
    public string BotNumber { get; set; }
    public List<string> AdminNumbers { get; set; }
}

static BotConfig LoadConfig()
{
    var json = File.ReadAllText("config.json");
    return JsonSerializer.Deserialize<BotConfig>(json);
}
```
<!---
## Next Steps

- Add persistence: [Database Bot Example](/examples/database-bot)
- Manage groups: [Group Manager Bot](/examples/group-manager)
- External APIs: [Integration Bot](/examples/integration-bot)
-->