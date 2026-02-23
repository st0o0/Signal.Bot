# Working with Attachments

Signal.Bot supports sending and receiving various types of attachments including images, videos, documents, and voice messages.

## Sending Attachments

### Single Attachment

<<< ./../../src/Signal.Bot.Example/Guide/Attachments.cs#SingleAttachment{csharp}

### Multiple Attachments

<<< ./../../src/Signal.Bot.Example/Guide/Attachments.cs#MultipleAttachments{csharp}

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

<<< ./../../src/Signal.Bot.Example/Guide/Attachments.cs#BasicAttachmentHandling{csharp}

### Downloading Attachments

<<< ./../../src/Signal.Bot.Example/Guide/Attachments.cs#DownloadingAttachments{csharp}

## Working with Different Attachment Types

### Images

<<< ./../../src/Signal.Bot.Example/Guide/Attachments.cs#Images{csharp}

### Documents

<<< ./../../src/Signal.Bot.Example/Guide/Attachments.cs#Documents{csharp}

### Audio/Voice Messages

<<< ./../../src/Signal.Bot.Example/Guide/Attachments.cs#Audios{csharp}

### Videos

<<< ./../../src/Signal.Bot.Example/Guide/Attachments.cs#Videos{csharp}

## Next Steps

- Learn about [profile management](/guide/profiles)
- Check out [complete examples](/examples/)