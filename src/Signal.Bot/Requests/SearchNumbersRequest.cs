namespace Signal.Bot.Requests;

public class SearchNumbersRequest(string number) : RequestBase<ICollection<SearchResponse>>($"v1/search/{number}")
{
    [JsonIgnore] public string Number => number;
    public override HttpMethod HttpMethod => HttpMethod.Get;
    public ICollection<string>? Numbers { get; set; }
}

public class SearchResponse
{
    [JsonPropertyName("number")] public string? Number { get; set; } = null;

    [JsonPropertyName("registered")] public bool? Registered { get; set; } = null;
}