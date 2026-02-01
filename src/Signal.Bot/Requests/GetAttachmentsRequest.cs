namespace Signal.Bot.Requests;

public record GetAttachmentsRequest() : RequestBase<ICollection<string>?>("v1/attachments", HttpMethod.Get);