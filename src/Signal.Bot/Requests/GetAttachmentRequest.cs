namespace Signal.Bot.Requests;

public class GetAttachmentRequest(string attachmentId) : RequestBase<string>($"v1/attachments/{attachmentId}")
{
    [JsonIgnore] public string AttachmentId => attachmentId;
    public override HttpMethod HttpMethod => HttpMethod.Get;
}