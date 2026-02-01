namespace Signal.Bot.Requests;

public record GetAttachmentRequest(string AttachmentId)
    : RequestBase<byte[]>($"v1/attachments/{AttachmentId}", HttpMethod.Get);