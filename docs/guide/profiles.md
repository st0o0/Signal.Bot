# Profile Management

Manage your bot's Signal profile including name, about text, and avatar.

## Updating Profile Name

```csharp
var client = new SignalBotClient("http://localhost:8080");
var botNumber = "+1234567890";

await client.UpdateProfileAsync(
    number: botNumber,
    name: "My Helpful Bot"
);
```

## Updating About/Status

```csharp
await client.UpdateProfileAsync(
    number: botNumber,
    about: "I'm a bot that helps with daily tasks! 🤖"
);
```

## Updating Avatar

```csharp
await client.UpdateProfileAsync(
    number: botNumber,
    avatar: "/path/to/avatar.jpg"
);
```

::: tip Avatar Requirements
- Supported formats: JPEG, PNG
- Recommended size: 512x512 pixels
- Max file size: 5 MB
:::

## Update Everything at Once

```csharp
await client.UpdateProfileAsync(
    number: botNumber,
    name: "Support Bot",
    about: "24/7 automated support",
    avatar: "/path/to/avatar.jpg"
);
```

## Getting Profile Information

```csharp
var profile = await client.GetProfileAsync(botNumber);

Console.WriteLine($"Name: {profile.Name}");
Console.WriteLine($"About: {profile.About}");
Console.WriteLine($"Avatar: {profile.AvatarUrl}");
```

## Getting Other Users' Profiles

```csharp
var userProfile = await client.GetProfileAsync(
    number: botNumber,
    recipient: "+0987654321"
);

Console.WriteLine($"User Name: {userProfile.Name}");
Console.WriteLine($"User About: {userProfile.About}");
```

## Profile Best Practices

### 1. Set Profile on Startup

```csharp
public class Bot
{
    private readonly SignalBotClient _client;
    private readonly string _botNumber;

    public async Task Initialize()
    {
        // Set up profile when bot starts
        await _client.UpdateProfileAsync(
            number: _botNumber,
            name: "Support Bot",
            about: "Ask me anything! Type /help for commands"
        );
        
        Console.WriteLine("Bot profile initialized");
    }
}
```

### 2. Use Descriptive Names

```csharp
// ✅ Good - clear and descriptive
await client.UpdateProfileAsync(
    number: botNumber,
    name: "Weather Bot",
    about: "Get weather forecasts and alerts"
);

// ❌ Bad - vague or confusing
await client.UpdateProfileAsync(
    number: botNumber,
    name: "Bot",
    about: "Stuff"
);
```

### 3. Update Status Dynamically

```csharp
public async Task SetStatus(string status)
{
    await _client.UpdateProfileAsync(
        number: _botNumber,
        about: $"Status: {status} | Last updated: {DateTime.Now:HH:mm}"
    );
}

// Usage
await SetStatus("Online");
await SetStatus("Maintenance mode");
await SetStatus("Processing requests");
```

### 4. Professional Avatars

```csharp
public class AvatarManager
{
    private readonly string _defaultAvatar = "/avatars/default.jpg";
    private readonly string _busyAvatar = "/avatars/busy.jpg";
    private readonly string _offlineAvatar = "/avatars/offline.jpg";

    public async Task SetOnline(SignalBotClient client, string number)
    {
        await client.UpdateProfileAsync(
            number: number,
            avatar: _defaultAvatar,
            about: "🟢 Online - Ready to help!"
        );
    }

    public async Task SetBusy(SignalBotClient client, string number)
    {
        await client.UpdateProfileAsync(
            number: number,
            avatar: _busyAvatar,
            about: "🟡 Busy - Please wait"
        );
    }

    public async Task SetOffline(SignalBotClient client, string number)
    {
        await client.UpdateProfileAsync(
            number: number,
            avatar: _offlineAvatar,
            about: "🔴 Offline - Back soon"
        );
    }
}
```

## Profile Update Notifications

Notify users when your profile changes:

```csharp
public async Task UpdateProfileWithNotification(string newName, string newAbout, List<string> subscribers)
{
    // Update profile
    await _client.UpdateProfileAsync(
        number: _botNumber,
        name: newName,
        about: newAbout
    );

    // Notify subscribers
    var notification = $"📢 Profile Updated!\n\nNew name: {newName}\nNew status: {newAbout}";
    
    foreach (var subscriber in subscribers)
    {
        await _client.SendMessageAsync(
            number: _botNumber,
            message: notification,
            recipients: new[] { subscriber }
        );
    }
}
```

## Avatar Helper Methods

### Create Avatar from Initials

```csharp
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.Fonts;

public async Task<string> CreateInitialsAvatar(string name, string outputPath)
{
    // Get initials
    var initials = string.Join("", name.Split(' ')
        .Take(2)
        .Select(word => word[0].ToString().ToUpper()));

    // Create image
    using var image = new Image<Rgba32>(512, 512);
    
    image.Mutate(ctx =>
    {
        // Background
        ctx.Fill(Color.Blue);
        
        // Text
        var font = SystemFonts.CreateFont("Arial", 200, FontStyle.Bold);
        var textOptions = new RichTextOptions(font)
        {
            Origin = new PointF(256, 256),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center
        };
        
        ctx.DrawText(textOptions, initials, Color.White);
    });

    await image.SaveAsync(outputPath);
    return outputPath;
}

// Usage
var avatarPath = await CreateInitialsAvatar("Support Bot", "/tmp/avatar.jpg");
await client.UpdateProfileAsync(
    number: botNumber,
    avatar: avatarPath
);
```

## Profile Information Display

Create a profile card for users:

```csharp
public async Task ShowProfileCard(string userNumber, string targetNumber = null)
{
    var profile = await _client.GetProfileAsync(
        number: _botNumber,
        recipient: targetNumber ?? _botNumber
    );

    var card = $@"👤 Profile Information

Name: {profile.Name ?? "Not set"}
About: {profile.About ?? "Not set"}
Number: {profile.Number}
Registered: {(profile.Registered ? "Yes" : "No")}";

    await _client.SendMessageAsync(
        number: _botNumber,
        message: card,
        recipients: new[] { userNumber }
    );
}
```

## Advanced Profile Management

### Profile Configuration Class

```csharp
public class BotProfile
{
    public string Name { get; set; }
    public string About { get; set; }
    public string AvatarPath { get; set; }
    public Dictionary<string, string> Statuses { get; set; } = new();

    public static BotProfile Load(string configPath)
    {
        var json = File.ReadAllText(configPath);
        return JsonSerializer.Deserialize<BotProfile>(json);
    }

    public void Save(string configPath)
    {
        var json = JsonSerializer.Serialize(this, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        File.WriteAllText(configPath, json);
    }
}

// config.json
{
  "Name": "Support Bot",
  "About": "Your 24/7 assistant",
  "AvatarPath": "/avatars/default.jpg",
  "Statuses": {
    "online": "🟢 Online and ready!",
    "busy": "🟡 Processing requests",
    "maintenance": "🔧 Under maintenance",
    "offline": "🔴 Currently offline"
  }
}

// Usage
var profile = BotProfile.Load("config.json");

await client.UpdateProfileAsync(
    number: botNumber,
    name: profile.Name,
    about: profile.Statuses["online"],
    avatar: profile.AvatarPath
);
```

### Scheduled Profile Updates

```csharp
public class ScheduledProfileManager
{
    private readonly SignalBotClient _client;
    private readonly string _botNumber;
    private readonly Timer _timer;

    public ScheduledProfileManager(SignalBotClient client, string botNumber)
    {
        _client = client;
        _botNumber = botNumber;
        _timer = new Timer(UpdateStatus, null, TimeSpan.Zero, TimeSpan.FromHours(1));
    }

    private async void UpdateStatus(object state)
    {
        var hour = DateTime.Now.Hour;
        string status;

        if (hour >= 9 && hour < 18)
        {
            status = "🟢 Online - Business hours";
        }
        else if (hour >= 18 && hour < 22)
        {
            status = "🟡 Limited support";
        }
        else
        {
            status = "🔴 Offline - Back at 9 AM";
        }

        await _client.UpdateProfileAsync(
            number: _botNumber,
            about: status
        );
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}

// Usage
var profileManager = new ScheduledProfileManager(client, botNumber);
// Profile will update automatically every hour
```

## Emojis in Profiles

Use emojis to make your profile more engaging:

```csharp
await client.UpdateProfileAsync(
    number: botNumber,
    name: "🤖 Helper Bot",
    about: "📊 Stats | 💬 Support | ⚙️ Automation"
);
```

## Next Steps

- Check out [examples](/examples/)