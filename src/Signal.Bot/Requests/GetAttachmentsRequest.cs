namespace Signal.Bot.Requests;

public record GetAttachmentsRequest() : RequestBase<List<string>?>("v1/attachments", HttpMethod.Get);