# Sending Messages

Learn how to send different types of messages using Signal.Bot.

## Basic Text Messages

The simplest way to send a message:

```csharp
var client = new SignalBotClient("http://localhost:8080");

await client.SendMessageAsync(
    number: "+1234567890",
    message: "Hello from Signal.Bot!",
    recipients: new[] { "+0987654321" }
);
```

## Sending to Multiple Recipients

Send the same message to multiple people:

```csharp
await client.SendMessageAsync(
    number: "+1234567890",
    message: "Broadcast message",
    recipients: new[] 
    { 
        "+0987654321",
        "+1111111111",
        "+2222222222"
    }
);
```

## Sending to Groups

Send a message to a Signal group:

```csharp
await client.SendMessageAsync(
    number: "+1234567890",
    message: "Hello everyone!",
    groupId: "your-group-id-here"
);
```

::: tip
You can get the group ID by listing your groups with `ListGroupsAsync`. See the [Groups](/guide/groups) guide for more details.
:::

## Messages with Attachments

Send messages with files attached:

```csharp
// Send a single attachment
await client.SendMessageAsync(
    number: "+1234567890",
    message: "Check out this image!",
    recipients: new[] { "+0987654321" },
    attachments: new[] { "/path/to/image.jpg" }
);

// Send multiple attachments
await client.SendMessageAsync(
    number: "+1234567890",
    message: "Here are some files",
    recipients: new[] { "+0987654321" },
    attachments: new[] 
    { 
        "/path/to/document.pdf",
        "/path/to/image.jpg",
        "/path/to/video.mp4"
    }
);
```

::: warning
Make sure the file paths are absolute and accessible by the signal-cli-rest-api Docker container. You may need to mount volumes appropriately.
:::

## Messages with Mentions

Mention specific users in group chats:

```csharp
await client.SendMessageAsync(
    number: "+1234567890",
    message: "Hey @user, check this out!",
    groupId: "your-group-id",
    mentions: new[] 
    { 
        new Mention 
        { 
            Number = "+0987654321",
            Start = 4,
            Length = 5
        } 
    }
);
```

## Quote Replies

Reply to a specific message:

```csharp
await client.SendMessageAsync(
    number: "+1234567890",
    message: "I agree!",
    recipients: new[] { "+0987654321" },
    quoteTimestamp: originalMessageTimestamp,
    quoteAuthor: "+0987654321",
    quoteMessage: "Original message text"
);
```

## Reactions

React to messages with emojis:

```csharp
await client.SendReactionAsync(
    number: "+1234567890",
    recipient: "+0987654321",
    reaction: "👍",
    targetAuthor: "+0987654321",
    targetTimestamp: messageTimestamp
);

// Remove a reaction
await client.SendReactionAsync(
    number: "+1234567890",
    recipient: "+0987654321",
    reaction: "",
    targetAuthor: "+0987654321",
    targetTimestamp: messageTimestamp,
    remove: true
);
```

## Typing Indicators

Show typing indicators to let users know you're preparing a response:

```csharp
// Start typing
await client.SendTypingIndicatorAsync(
    number: "+1234567890",
    recipient: "+0987654321"
);

// Simulate some processing
await Task.Delay(2000);

// Send the actual message
await client.SendMessageAsync(
    number: "+1234567890",
    message: "Here's your response!",
    recipients: new[] { "+0987654321" }
);

// Stop typing (automatically stopped when message is sent)
await client.SendTypingIndicatorAsync(
    number: "+1234567890",
    recipient: "+0987654321",
    stop: true
);
```

## Read Receipts

Mark messages as read:

```csharp
await client.SendReceiptAsync(
    number: "+1234567890",
    recipient: "+0987654321",
    timestamps: new[] { messageTimestamp },
    type: ReceiptType.Read
);
```

## Error Handling

Always handle potential errors when sending messages:

```csharp
try
{
    await client.SendMessageAsync(
        number: "+1234567890",
        message: "Hello!",
        recipients: new[] { "+0987654321" }
    );
    Console.WriteLine("Message sent successfully!");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Network error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error sending message: {ex.Message}");
}
```

## Best Practices

### 1. Use Async/Await Properly

```csharp
// ✅ Good
await client.SendMessageAsync(...);

// ❌ Bad - blocks the thread
client.SendMessageAsync(...).Wait();
```

### 2. Handle Cancellation

```csharp
var cts = new CancellationTokenSource();

try
{
    await client.SendMessageAsync(
        number: "+1234567890",
        message: "Hello!",
        recipients: new[] { "+0987654321" },
        cancellationToken: cts.Token
    );
}
catch (OperationCanceledException)
{
    Console.WriteLine("Operation was cancelled");
}
```

### 3. Validate Phone Numbers

```csharp
public bool IsValidPhoneNumber(string number)
{
    // Must start with + and contain only digits after
    return number.StartsWith("+") && 
           number.Substring(1).All(char.IsDigit) &&
           number.Length >= 10;
}

var recipient = "+0987654321";
if (IsValidPhoneNumber(recipient))
{
    await client.SendMessageAsync(...);
}
```

### 4. Batch Operations Carefully

```csharp
// Don't spam - add delays between messages
foreach (var recipient in recipients)
{
    await client.SendMessageAsync(
        number: botNumber,
        message: "Broadcast",
        recipients: new[] { recipient }
    );
    
    await Task.Delay(1000); // 1 second delay
}
```

## Next Steps

- Learn about [receiving messages](/guide/receiving-messages)
- Work with [groups](/guide/groups)
- Explore [attachments in detail](/guide/attachments)