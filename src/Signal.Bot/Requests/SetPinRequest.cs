namespace Signal.Bot.Requests;

public record SetPinRequest(string Number) : RequestBase($"v1/accounts/{Number}/pin")
{
    public string? Pin { get; set; }
}