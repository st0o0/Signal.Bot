namespace Signal.Bot.Requests;

public class RemoveAttachmentRequest(string attachmentId) : RequestBase($"v1/attachments/{attachmentId}")
{
    [JsonIgnore] public string AttachmentId => attachmentId;
    public override HttpMethod HttpMethod => HttpMethod.Delete;
}