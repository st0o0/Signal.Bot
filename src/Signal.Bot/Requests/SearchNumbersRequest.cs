using Signal.Bot.Types;

namespace Signal.Bot.Requests;

public class SearchNumbersRequest(string number) : RequestBase<ICollection<Search>>($"v1/search/{number}")
{
    [JsonIgnore] public string Number => number;
    public override HttpMethod HttpMethod => HttpMethod.Get;
    public override HttpContent ToHttpContent() => new StringContent(string.Empty);
    public ICollection<string>? Numbers { get; set; }
}