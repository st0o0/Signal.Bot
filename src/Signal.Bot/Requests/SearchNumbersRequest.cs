using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record SearchNumbersRequest(string Number)
    : RequestBase<List<Search>?>($"v1/search/{Number}", HttpMethod.Get);