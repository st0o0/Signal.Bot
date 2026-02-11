using WebSocket.Rx;

namespace Signal.Bot;

public record ConnectionError(ConnectionType Type)
    : Error(null, ErrorType.ConnectionHappened);

public enum ConnectionType
{
    Undefined = 0,
    Initial = 1,
    Reconnect = 2
}

internal static class ReconnectionTypeExtensions
{
    internal static Error To(this Connected info)
    {
        return new ConnectionError(info.Reason.To());
    }

    private static ConnectionType To(this ConnectReason value)
    {
        return value switch
        {
            ConnectReason.Undefined => ConnectionType.Undefined,
            ConnectReason.Initial => ConnectionType.Initial,
            ConnectReason.Reconnect => ConnectionType.Reconnect,
            _ => ConnectionType.Undefined
        };
    }
}