namespace Signal.Bot.Requests;

public class GetAttachmentRequest(string attachmentId) : RequestBase<byte[]>($"v1/attachments/{attachmentId}")
{
    [JsonIgnore] public string AttachmentId => attachmentId;
    public override HttpMethod HttpMethod => HttpMethod.Get;
}