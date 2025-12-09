namespace Signal.Bot.Requests;

public class GetQRCodeRequest() : RequestBase("v1/qrcodelink")
{
    public override HttpMethod HttpMethod => HttpMethod.Get;

    public string? DeviceName { get; set; }

    [JsonPropertyName("qrcode_version")] public int QRCodeVersion { get; set; } = 10;
}