namespace Signal.Bot.Requests;

public record GetAttachmentRequest(string AttachmentId) : RequestBase($"v1/attachments/{AttachmentId}", HttpMethod.Get);