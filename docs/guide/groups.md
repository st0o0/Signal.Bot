# Working with Groups

Signal.Bot provides comprehensive support for managing Signal groups.

## Creating Groups

Create a new Signal group with members:

```csharp
var client = new SignalBotClient("http://localhost:8080");
var botNumber = "+1234567890";

var group = await client.CreateGroupAsync(
    number: botNumber,
    name: "My Bot Group",
    members: new[] 
    { 
        "+1111111111",
        "+2222222222",
        "+3333333333"
    }
);

Console.WriteLine($"Created group: {group.Name}");
Console.WriteLine($"Group ID: {group.Id}");
```

### With Description and Avatar

```csharp
var group = await client.CreateGroupAsync(
    number: botNumber,
    name: "Project Team",
    members: new[] { "+1111111111", "+2222222222" },
    description: "Our awesome project team",
    avatar: "/path/to/group-avatar.jpg"
);
```

## Listing Groups

Get all groups the bot is a member of:

```csharp
var groups = await client.ListGroupsAsync(botNumber);

foreach (var group in groups)
{
    Console.WriteLine($"Group: {group.Name}");
    Console.WriteLine($"  ID: {group.Id}");
    Console.WriteLine($"  Members: {group.Members?.Count ?? 0}");
    Console.WriteLine($"  Is Blocked: {group.IsBlocked}");
    Console.WriteLine();
}
```

## Getting Group Details

Retrieve detailed information about a specific group:

```csharp
var groupId = "your-group-id-here";
var groupInfo = await client.GetGroupAsync(botNumber, groupId);

Console.WriteLine($"Name: {groupInfo.Name}");
Console.WriteLine($"Description: {groupInfo.Description}");
Console.WriteLine($"Members:");
foreach (var member in groupInfo.Members)
{
    Console.WriteLine($"  - {member.Number}");
}
```

## Updating Groups

### Update Group Name

```csharp
await client.UpdateGroupAsync(
    number: botNumber,
    groupId: groupId,
    name: "Updated Group Name"
);
```

### Update Description

```csharp
await client.UpdateGroupAsync(
    number: botNumber,
    groupId: groupId,
    description: "New group description"
);
```

### Update Avatar

```csharp
await client.UpdateGroupAsync(
    number: botNumber,
    groupId: groupId,
    avatar: "/path/to/new-avatar.jpg"
);
```

### Add Members

```csharp
await client.UpdateGroupAsync(
    number: botNumber,
    groupId: groupId,
    addMembers: new[] 
    { 
        "+4444444444",
        "+5555555555"
    }
);
```

### Remove Members

```csharp
await client.UpdateGroupAsync(
    number: botNumber,
    groupId: groupId,
    removeMembers: new[] { "+4444444444" }
);
```

### Make Admins

```csharp
await client.UpdateGroupAsync(
    number: botNumber,
    groupId: groupId,
    admins: new[] { "+1111111111" }
);
```

### Update Everything at Once

```csharp
await client.UpdateGroupAsync(
    number: botNumber,
    groupId: groupId,
    name: "Updated Name",
    description: "Updated description",
    addMembers: new[] { "+6666666666" },
    removeMembers: new[] { "+4444444444" },
    admins: new[] { "+1111111111", "+2222222222" }
);
```

## Leaving Groups

Leave a group:

```csharp
await client.QuitGroupAsync(botNumber, groupId);
Console.WriteLine($"Left group {groupId}");
```

## Deleting Groups

Delete a group (admin only):

```csharp
await client.DeleteGroupAsync(botNumber, groupId);
Console.WriteLine($"Deleted group {groupId}");
```

## Sending Messages to Groups

Send a message to a group:

```csharp
await client.SendMessageAsync(
    number: botNumber,
    message: "Hello everyone!",
    groupId: groupId
);
```

### With Mentions

Mention specific members in the group:

```csharp
await client.SendMessageAsync(
    number: botNumber,
    message: "Hey @John, check this out!",
    groupId: groupId,
    mentions: new[] 
    { 
        new Mention 
        { 
            Number = "+1111111111",
            Start = 4,  // Position of @John in the message
            Length = 5  // Length of "John"
        } 
    }
);
```

## Handling Group Messages

Receive and process group messages:

```csharp
await client.StartReceiving(
    botNumber,
    handleMessage: async (botClient, message, ct) =>
    {
        // Check if it's a group message
        if (!string.IsNullOrEmpty(message.DataMessage?.GroupId))
        {
            var groupId = message.DataMessage.GroupId;
            var sender = message.Source;
            var text = message.DataMessage.Message;
            
            Console.WriteLine($"[Group: {groupId}] {sender}: {text}");
            
            // Respond to group commands
            if (text?.StartsWith("/group") == true)
            {
                await botClient.SendMessageAsync(
                    number: botNumber,
                    message: "Group command received!",
                    groupId: groupId,
                    cancellationToken: ct
                );
            }
        }
    },
    handleError: async (botClient, exception, ct) =>
    {
        Console.WriteLine($"Error: {exception.Message}");
    },
    cancellationToken: CancellationToken.None
);
```

## Group Management Bot Example

A complete example of a group management bot:

```csharp
public class GroupManagerBot
{
    private readonly SignalBotClient _client;
    private readonly string _botNumber;
    private readonly HashSet<string> _adminNumbers;

    public GroupManagerBot(SignalBotClient client, string botNumber, IEnumerable<string> admins)
    {
        _client = client;
        _botNumber = botNumber;
        _adminNumbers = new HashSet<string>(admins);
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        await _client.StartReceiving(
            _botNumber,
            handleMessage: HandleGroupMessage,
            handleError: HandleError,
            cancellationToken: cancellationToken
        );
    }

    private async Task HandleGroupMessage(SignalBotClient client, SignalMessage message, CancellationToken ct)
    {
        var groupId = message.DataMessage?.GroupId;
        if (string.IsNullOrEmpty(groupId)) return;

        var text = message.DataMessage?.Message;
        if (string.IsNullOrEmpty(text)) return;

        var sender = message.Source;
        var isAdmin = _adminNumbers.Contains(sender);

        switch (text.ToLower())
        {
            case "/groupinfo":
                await ShowGroupInfo(groupId, ct);
                break;

            case "/members":
                await ListMembers(groupId, ct);
                break;

            case var cmd when cmd.StartsWith("/kick ") && isAdmin:
                var number = cmd.Replace("/kick ", "").Trim();
                await KickMember(groupId, number, ct);
                break;

            case var cmd when cmd.StartsWith("/add ") && isAdmin:
                var newMember = cmd.Replace("/add ", "").Trim();
                await AddMember(groupId, newMember, ct);
                break;

            case "/help":
                await ShowHelp(groupId, isAdmin, ct);
                break;
        }
    }

    private async Task ShowGroupInfo(string groupId, CancellationToken ct)
    {
        var group = await _client.GetGroupAsync(_botNumber, groupId);
        
        var info = $@"📋 Group Information
Name: {group.Name}
Description: {group.Description ?? "N/A"}
Members: {group.Members?.Count ?? 0}
Admins: {group.Admins?.Count ?? 0}";

        await _client.SendMessageAsync(
            number: _botNumber,
            message: info,
            groupId: groupId,
            cancellationToken: ct
        );
    }

    private async Task ListMembers(string groupId, CancellationToken ct)
    {
        var group = await _client.GetGroupAsync(_botNumber, groupId);
        
        var memberList = "👥 Group Members:\n" + 
            string.Join("\n", group.Members?.Select(m => $"- {m.Number}") ?? Array.Empty<string>());

        await _client.SendMessageAsync(
            number: _botNumber,
            message: memberList,
            groupId: groupId,
            cancellationToken: ct
        );
    }

    private async Task KickMember(string groupId, string number, CancellationToken ct)
    {
        try
        {
            await _client.UpdateGroupAsync(
                number: _botNumber,
                groupId: groupId,
                removeMembers: new[] { number }
            );

            await _client.SendMessageAsync(
                number: _botNumber,
                message: $"✅ Removed {number} from the group",
                groupId: groupId,
                cancellationToken: ct
            );
        }
        catch (Exception ex)
        {
            await _client.SendMessageAsync(
                number: _botNumber,
                message: $"❌ Failed to remove member: {ex.Message}",
                groupId: groupId,
                cancellationToken: ct
            );
        }
    }

    private async Task AddMember(string groupId, string number, CancellationToken ct)
    {
        try
        {
            await _client.UpdateGroupAsync(
                number: _botNumber,
                groupId: groupId,
                addMembers: new[] { number }
            );

            await _client.SendMessageAsync(
                number: _botNumber,
                message: $"✅ Added {number} to the group",
                groupId: groupId,
                cancellationToken: ct
            );
        }
        catch (Exception ex)
        {
            await _client.SendMessageAsync(
                number: _botNumber,
                message: $"❌ Failed to add member: {ex.Message}",
                groupId: groupId,
                cancellationToken: ct
            );
        }
    }

    private async Task ShowHelp(string groupId, bool isAdmin, CancellationToken ct)
    {
        var help = @"🤖 Group Bot Commands

Available to everyone:
/help - Show this message
/groupinfo - Show group information
/members - List all members";

        if (isAdmin)
        {
            help += @"

Admin commands:
/kick <number> - Remove a member
/add <number> - Add a member";
        }

        await _client.SendMessageAsync(
            number: _botNumber,
            message: help,
            groupId: groupId,
            cancellationToken: ct
        );
    }

    private async Task HandleError(SignalBotClient client, Exception exception, CancellationToken ct)
    {
        Console.WriteLine($"Error: {exception.Message}");
    }
}

// Usage
var bot = new GroupManagerBot(
    client,
    "+1234567890",
    new[] { "+9999999999" } // Admin numbers
);

await bot.Start(cts.Token);
```

## Best Practices

### 1. Store Group IDs Persistently

```csharp
// Save group IDs to a database or file
public class GroupStore
{
    private readonly Dictionary<string, string> _groups = new();

    public void SaveGroup(string name, string id)
    {
        _groups[name] = id;
        File.WriteAllText("groups.json", JsonSerializer.Serialize(_groups));
    }

    public string GetGroupId(string name)
    {
        return _groups.TryGetValue(name, out var id) ? id : null;
    }
}
```

### 2. Validate Permissions

```csharp
private bool IsGroupAdmin(string groupId, string number)
{
    var group = await _client.GetGroupAsync(_botNumber, groupId);
    return group.Admins?.Any(a => a.Number == number) ?? false;
}
```

### 3. Handle Group Events

```csharp
await client.StartReceiving(
    botNumber,
    handleMessage: async (botClient, message, ct) =>
    {
        // Member joined
        if (message.DataMessage?.GroupInfo?.Type == "MEMBER_ADDED")
        {
            await botClient.SendMessageAsync(
                number: botNumber,
                message: "Welcome to the group! 👋",
                groupId: message.DataMessage.GroupId,
                cancellationToken: ct
            );
        }
        
        // Member left
        if (message.DataMessage?.GroupInfo?.Type == "MEMBER_LEFT")
        {
            // Handle member leaving
        }
    },
    handleError: HandleError,
    cancellationToken: cts.Token
);
```

## Next Steps

- Learn about [attachments](/guide/attachments)
- Explore [profile management](/guide/profiles)
- Check out [advanced examples](/examples/)