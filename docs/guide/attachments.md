# Working with Attachments

Signal.Bot supports sending and receiving various types of attachments including images, videos, documents, and voice messages.

## Sending Attachments

### Single Attachment

```csharp
await client.SendMessageAsync(
    number: "+1234567890",
    message: "Check out this image!",
    recipients: new[] { "+0987654321" },
    attachments: new[] { "/path/to/image.jpg" }
);
```

### Multiple Attachments

```csharp
await client.SendMessageAsync(
    number: "+1234567890",
    message: "Here are the files you requested",
    recipients: new[] { "+0987654321" },
    attachments: new[] 
    { 
        "/path/to/document.pdf",
        "/path/to/image.jpg",
        "/path/to/video.mp4"
    }
);
```

### Attachment Without Message

```csharp
// Just send the file without text
await client.SendMessageAsync(
    number: "+1234567890",
    message: "",
    recipients: new[] { "+0987654321" },
    attachments: new[] { "/path/to/file.pdf" }
);
```

## Supported File Types

Signal supports various file types:

| Category | Extensions | Max Size |
|----------|-----------|----------|
| Images | .jpg, .jpeg, .png, .gif, .webp | 100 MB |
| Videos | .mp4, .mov, .avi, .mkv | 100 MB |
| Audio | .mp3, .m4a, .aac, .wav | 100 MB |
| Documents | .pdf, .doc, .docx, .xls, .xlsx, .txt | 100 MB |
| Archives | .zip, .rar, .7z | 100 MB |

::: warning File Size Limit
Signal has a 100 MB limit for attachments. Larger files will be rejected.
:::

## Receiving Attachments

### Basic Attachment Handling

```csharp
await client.StartReceiving(
    botNumber,
    handleMessage: async (botClient, message, ct) =>
    {
        var attachments = message.DataMessage?.Attachments;
        
        if (attachments?.Any() == true)
        {
            foreach (var attachment in attachments)
            {
                Console.WriteLine($"Received: {attachment.Filename}");
                Console.WriteLine($"Type: {attachment.ContentType}");
                Console.WriteLine($"Size: {attachment.Size} bytes");
                Console.WriteLine($"ID: {attachment.Id}");
            }
        }
    },
    handleError: HandleError,
    cancellationToken: cts.Token
);
```

### Downloading Attachments

```csharp
await client.StartReceiving(
    botNumber,
    handleMessage: async (botClient, message, ct) =>
    {
        var attachments = message.DataMessage?.Attachments;
        
        if (attachments?.Any() == true)
        {
            foreach (var attachment in attachments)
            {
                // Download the attachment
                var fileData = await botClient.GetAttachmentAsync(
                    number: botNumber,
                    attachmentId: attachment.Id
                );
                
                // Save to disk
                var fileName = attachment.Filename ?? $"attachment_{attachment.Id}";
                var filePath = Path.Combine("downloads", fileName);
                
                Directory.CreateDirectory("downloads");
                await File.WriteAllBytesAsync(filePath, fileData, ct);
                
                Console.WriteLine($"Saved to: {filePath}");
            }
        }
    },
    handleError: HandleError,
    cancellationToken: cts.Token
);
```

## Working with Different Attachment Types

### Images

```csharp
await client.StartReceiving(
    botNumber,
    handleMessage: async (botClient, message, ct) =>
    {
        var images = message.DataMessage?.Attachments?
            .Where(a => a.ContentType.StartsWith("image/"))
            .ToList();
        
        if (images?.Any() == true)
        {
            foreach (var image in images)
            {
                Console.WriteLine($"Received image: {image.Filename}");
                
                // Download and process
                var imageData = await botClient.GetAttachmentAsync(botNumber, image.Id);
                
                // You could process the image here
                // e.g., resize, convert format, etc.
                
                // Save
                await File.WriteAllBytesAsync($"images/{image.Filename}", imageData, ct);
            }
        }
    },
    handleError: HandleError,
    cancellationToken: cts.Token
);
```

### Documents

```csharp
var documents = message.DataMessage?.Attachments?
    .Where(a => a.ContentType == "application/pdf" || 
                a.ContentType.Contains("document"))
    .ToList();

if (documents?.Any() == true)
{
    foreach (var doc in documents)
    {
        Console.WriteLine($"Received document: {doc.Filename}");
        
        var docData = await client.GetAttachmentAsync(botNumber, doc.Id);
        await File.WriteAllBytesAsync($"documents/{doc.Filename}", docData);
    }
}
```

### Audio/Voice Messages

```csharp
var audioFiles = message.DataMessage?.Attachments?
    .Where(a => a.ContentType.StartsWith("audio/"))
    .ToList();

if (audioFiles?.Any() == true)
{
    foreach (var audio in audioFiles)
    {
        Console.WriteLine($"Received audio: {audio.Filename}");
        
        // Check if it's a voice message
        if (audio.VoiceNote == true)
        {
            Console.WriteLine("This is a voice message");
        }
        
        var audioData = await client.GetAttachmentAsync(botNumber, audio.Id);
        await File.WriteAllBytesAsync($"audio/{audio.Filename}", audioData);
    }
}
```

### Videos

```csharp
var videos = message.DataMessage?.Attachments?
    .Where(a => a.ContentType.StartsWith("video/"))
    .ToList();

if (videos?.Any() == true)
{
    foreach (var video in videos)
    {
        Console.WriteLine($"Received video: {video.Filename}");
        Console.WriteLine($"Size: {video.Size / 1024 / 1024} MB");
        
        var videoData = await client.GetAttachmentAsync(botNumber, video.Id);
        await File.WriteAllBytesAsync($"videos/{video.Filename}", videoData);
    }
}
```

## Attachment Processing Example

Here's a complete example of a bot that processes different types of attachments:

```csharp
public class AttachmentProcessorBot
{
    private readonly SignalBotClient _client;
    private readonly string _botNumber;
    private readonly string _downloadPath;

    public AttachmentProcessorBot(SignalBotClient client, string botNumber, string downloadPath)
    {
        _client = client;
        _botNumber = botNumber;
        _downloadPath = downloadPath;
        
        // Create directories
        Directory.CreateDirectory(Path.Combine(downloadPath, "images"));
        Directory.CreateDirectory(Path.Combine(downloadPath, "documents"));
        Directory.CreateDirectory(Path.Combine(downloadPath, "videos"));
        Directory.CreateDirectory(Path.Combine(downloadPath, "audio"));
        Directory.CreateDirectory(Path.Combine(downloadPath, "other"));
    }

    public async Task Start(CancellationToken cancellationToken)
    {
        await _client.StartReceiving(
            _botNumber,
            handleMessage: ProcessAttachments,
            handleError: HandleError,
            cancellationToken: cancellationToken
        );
    }

    private async Task ProcessAttachments(SignalBotClient client, SignalMessage message, CancellationToken ct)
    {
        var attachments = message.DataMessage?.Attachments;
        if (attachments?.Any() != true) return;

        var sender = message.Source;
        var processedFiles = new List<string>();

        foreach (var attachment in attachments)
        {
            try
            {
                var data = await client.GetAttachmentAsync(_botNumber, attachment.Id, ct);
                var category = GetCategory(attachment.ContentType);
                var fileName = attachment.Filename ?? $"file_{attachment.Id}";
                var filePath = Path.Combine(_downloadPath, category, fileName);
                
                await File.WriteAllBytesAsync(filePath, data, ct);
                processedFiles.Add($"{category}/{fileName}");
                
                Console.WriteLine($"Saved {fileName} to {category}/");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing {attachment.Filename}: {ex.Message}");
            }
        }

        // Send confirmation
        if (processedFiles.Any())
        {
            var message = $"✅ Processed {processedFiles.Count} file(s):\n" +
                         string.Join("\n", processedFiles.Select(f => $"- {f}"));
            
            await client.SendMessageAsync(
                number: _botNumber,
                message: message,
                recipients: new[] { sender },
                cancellationToken: ct
            );
        }
    }

    private string GetCategory(string contentType)
    {
        return contentType switch
        {
            var t when t.StartsWith("image/") => "images",
            var t when t.StartsWith("video/") => "videos",
            var t when t.StartsWith("audio/") => "audio",
            var t when t.Contains("pdf") || t.Contains("document") => "documents",
            _ => "other"
        };
    }

    private async Task HandleError(SignalBotClient client, Exception exception, CancellationToken ct)
    {
        Console.WriteLine($"Error: {exception.Message}");
    }
}

// Usage
var bot = new AttachmentProcessorBot(
    client,
    "+1234567890",
    "/path/to/downloads"
);

await bot.Start(cts.Token);
```

## File Path Considerations

### Docker Volume Mounts

When using signal-cli-rest-api with Docker, you need to ensure attachment paths are accessible:

```bash
docker run -d --name signal-api \
  -p 8080:8080 \
  -v $HOME/.local/share/signal-api:/home/.local/share/signal-cli \
  -v /path/to/attachments:/attachments \
  bbernhard/signal-cli-rest-api
```

Then in your code:

```csharp
// This path should be inside the mounted volume
await client.SendMessageAsync(
    number: "+1234567890",
    message: "Here's your file",
    recipients: new[] { "+0987654321" },
    attachments: new[] { "/attachments/myfile.pdf" }
);
```

### Using Absolute Paths

Always use absolute paths for attachments:

```csharp
// ✅ Good
var absolutePath = Path.GetFullPath("myfile.pdf");
await client.SendMessageAsync(..., attachments: new[] { absolutePath });

// ❌ Bad - relative paths may not work with Docker
await client.SendMessageAsync(..., attachments: new[] { "myfile.pdf" });
```

## Temporary Files

For processing temporary files:

```csharp
public async Task SendProcessedImage(byte[] imageData, string recipient)
{
    // Create temporary file
    var tempPath = Path.Combine(Path.GetTempPath(), $"processed_{Guid.NewGuid()}.jpg");
    
    try
    {
        await File.WriteAllBytesAsync(tempPath, imageData);
        
        await _client.SendMessageAsync(
            number: _botNumber,
            message: "Here's the processed image",
            recipients: new[] { recipient },
            attachments: new[] { tempPath }
        );
    }
    finally
    {
        // Clean up
        if (File.Exists(tempPath))
        {
            File.Delete(tempPath);
        }
    }
}
```

## Best Practices

### 1. Validate File Sizes

```csharp
public bool IsFileSizeValid(string filePath, long maxSizeBytes = 100 * 1024 * 1024)
{
    var fileInfo = new FileInfo(filePath);
    return fileInfo.Exists && fileInfo.Length <= maxSizeBytes;
}
```

### 2. Check File Types

```csharp
private readonly HashSet<string> _allowedExtensions = new()
{
    ".jpg", ".jpeg", ".png", ".gif",
    ".pdf", ".doc", ".docx",
    ".mp4", ".mov"
};

public bool IsFileTypeAllowed(string filePath)
{
    var extension = Path.GetExtension(filePath).ToLower();
    return _allowedExtensions.Contains(extension);
}
```

### 3. Handle Large Files

```csharp
// Warn users about large files
var fileInfo = new FileInfo(filePath);
if (fileInfo.Length > 50 * 1024 * 1024) // 50 MB
{
    await client.SendMessageAsync(
        number: botNumber,
        message: "⚠️ Large file detected. This may take a while...",
        recipients: new[] { recipient }
    );
}
```

### 4. Organize Downloads

```csharp
public string GetDownloadPath(Attachment attachment, string sender)
{
    var date = DateTime.Now.ToString("yyyy-MM-dd");
    var category = GetCategory(attachment.ContentType);
    var sanitizedSender = sender.Replace("+", "");
    
    return Path.Combine(
        _downloadPath,
        date,
        category,
        sanitizedSender,
        attachment.Filename
    );
}
```

## Next Steps

- Learn about [profile management](/guide/profiles)
- Check out [complete examples](/examples/)