namespace Signal.Bot.Requests;

public record RemoveAttachmentRequest(string AttachmentId)
    : RequestBase($"v1/attachments/{AttachmentId}", HttpMethod.Delete);