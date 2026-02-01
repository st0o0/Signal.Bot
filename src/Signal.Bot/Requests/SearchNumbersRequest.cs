using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public record SearchNumbersRequest(string Number)
    : RequestBase<ICollection<Search>?>($"v1/search/{Number}", HttpMethod.Get)
{
    public ICollection<string>? Numbers { get; set; }
}