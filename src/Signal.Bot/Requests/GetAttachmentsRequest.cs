namespace Signal.Bot.Requests;

public class GetAttachmentsRequest() : RequestBase<ICollection<string>>("v1/attachments")
{
    public override HttpMethod HttpMethod => HttpMethod.Get;
}
