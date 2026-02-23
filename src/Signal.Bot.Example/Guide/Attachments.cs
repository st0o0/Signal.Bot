namespace Signal.Bot.Example.Guide;

public class Attachments
{
    private readonly SignalBotClient client = null;
    public async Task SingleAttachment()
    {
        #region SingleAttachment
        await client.SendMessageAsync(builder =>
            builder
                .WithMessage("Check out this image!")
                .WithRecipient("+0987654321")
                .WithAttachmentFromFile("/path/to/image.jpg", includeFilename: true));
        #endregion SingleAttachment
    }

    public async Task MultipleAttachments()
    {
        #region MultipleAttachments
        await client.SendMessageAsync(builder =>
            builder
            .WithAttachmentFromFile("/path/to/document.pdf")
            .WithAttachmentFromFile("/path/to/image.jpg")
            .WithAttachmentFromFile("/path/to/video.mp4"));
        #endregion MultipleAttachments
    }

    public async Task BasicAttachmentHandling()
    {
        var cts = new CancellationTokenSource();
        var ct = cts.Token;
        #region BasicAttachmentHandling
        client.StartReceiving(updateHandler: (botClient, message, ct) =>
        {
            var attachments = message.Envelope?.DataMessage?.Attachments ?? [];
            foreach (var attachment in attachments)
            {
                Console.WriteLine($"Received: {attachment.Filename}");
                Console.WriteLine($"Type: {attachment.ContentType}");
                Console.WriteLine($"Size: {attachment.Size} bytes");
                Console.WriteLine($"ID: {attachment.Id}");
            }
        }, errorHandler: async (botClient, err, ct) =>
        {
            if (err.Exception is null) return;
            Console.WriteLine($"Exception: {err.Exception.Message}");
            await Task.CompletedTask;
        }, optionBuilder => optionBuilder.WithTimeout(TimeSpan.FromSeconds(90)), ct);
        #endregion BasicAttachmentHandling
    }

    public async Task DownloadingAttachments()
    {
        var cts = new CancellationTokenSource();
        var ct = cts.Token;
        #region DownloadingAttachments
        client.StartReceiving(updateHandler: async (botClient, message, ct) =>
        {
            var attachments = message.Envelope?.DataMessage?.Attachments ?? [];
            foreach (var attachment in attachments)
            {
                var data = await botClient.GetAttachmentAsync(attachment.Id!, ct);
                var fileName = attachment.Filename ?? $"attachment_{attachment.Id}";
                var filePath = Path.Combine("downloads", fileName);

                Directory.CreateDirectory("downloads");
                await File.WriteAllBytesAsync(filePath, data, ct);

                Console.WriteLine($"Saved to: {filePath}");
            }
        }, errorHandler: async (botClient, err, ct) =>
        {
            if (err.Exception is null) return;
            Console.WriteLine($"Exception: {err.Exception.Message}");
            await Task.CompletedTask;
        }, optionBuilder => optionBuilder.WithTimeout(TimeSpan.FromSeconds(90)), ct);
        #endregion DownloadingAttachments
    }

    public async Task Images()
    {
        var cts = new CancellationTokenSource();
        var ct = cts.Token;
        #region Images
        client.StartReceiving(updateHandler: async (botClient, message, ct) =>
        {
            var images = message.Envelope?.DataMessage?.Attachments?
            .Where(a => a.ContentType!.StartsWith("image/"))
            .ToList() ?? [];

            foreach (var image in images)
            {
                Console.WriteLine($"Received image: {image.Filename}");

                // Download and process
                var imageData = await botClient.GetAttachmentAsync(image.Id!, ct);

                // You could process the image here
                // e.g., resize, convert format, etc.

                // Save
                await File.WriteAllBytesAsync($"images/{image.Filename}", imageData, ct);
            }
        }, errorHandler: async (botClient, err, ct) =>
        {
            if (err.Exception is null) return;
            Console.WriteLine($"Exception: {err.Exception.Message}");
            await Task.CompletedTask;
        }, optionBuilder => optionBuilder.WithTimeout(TimeSpan.FromSeconds(90)), ct);
        #endregion Images
    }

    public async Task Documents()
    {
        var cts = new CancellationTokenSource();
        var ct = cts.Token;
        #region Documents
        client.StartReceiving(updateHandler: async (botClient, message, ct) =>
        {
            var documents = message.Envelope?.DataMessage?.Attachments?
            .Where(a => a.ContentType == "application/pdf" || a.ContentType!.Contains("document"))
            .ToList() ?? [];

            foreach (var doc in documents)
            {
                Console.WriteLine($"Received document: {doc.Filename}");

                var docData = await client.GetAttachmentAsync(doc.Id!, ct);
                await File.WriteAllBytesAsync($"documents/{doc.Filename}", docData, ct);
            }
        }, errorHandler: async (botClient, err, ct) =>
        {
            if (err.Exception is null) return;
            Console.WriteLine($"Exception: {err.Exception.Message}");
            await Task.CompletedTask;
        }, optionBuilder => optionBuilder.WithTimeout(TimeSpan.FromSeconds(90)), ct);
        #endregion Documents
    }

    public async Task Audios()
    {
        var cts = new CancellationTokenSource();
        var ct = cts.Token;
        #region Audios
        client.StartReceiving(updateHandler: async (botClient, message, ct) =>
        {
            var audioFiles = message.Envelope?.DataMessage?.Attachments?
                    .Where(a => a.ContentType!.StartsWith("audio/"))
                    .ToList() ?? [];

            foreach (var audio in audioFiles)
            {
                Console.WriteLine($"Received audio: {audio.Filename}");

                var audioData = await client.GetAttachmentAsync(audio.Id!, ct);
                await File.WriteAllBytesAsync($"audio/{audio.Filename}", audioData, ct);
            }
        }, errorHandler: async (botClient, err, ct) =>
        {
            if (err.Exception is null) return;
            Console.WriteLine($"Exception: {err.Exception.Message}");
            await Task.CompletedTask;
        }, optionBuilder => optionBuilder.WithTimeout(TimeSpan.FromSeconds(90)), ct);
        #endregion Audios
    }

    public async Task Videos()
    {
        var cts = new CancellationTokenSource();
        var ct = cts.Token;
        #region Videos
        client.StartReceiving(updateHandler: async (botClient, message, ct) =>
        {
            var videos = message.Envelope?.DataMessage?.Attachments?
                    .Where(a => a.ContentType!.StartsWith("video/"))
                    .ToList() ?? [];

            foreach (var video in videos)
            {
                Console.WriteLine($"Received video: {video.Filename}");
                Console.WriteLine($"Size: {video.Size / 1024 / 1024} MB");

                var videoData = await client.GetAttachmentAsync(video.Id!, ct);
                await File.WriteAllBytesAsync($"videos/{video.Filename}", videoData, ct);
            }
        }, errorHandler: async (botClient, err, ct) =>
        {
            if (err.Exception is null) return;
            Console.WriteLine($"Exception: {err.Exception.Message}");
            await Task.CompletedTask;
        }, optionBuilder => optionBuilder.WithTimeout(TimeSpan.FromSeconds(90)), ct);
        #endregion Videos
    }
}