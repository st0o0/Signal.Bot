using System.Net.WebSockets;
using WebSocket.Rx;

namespace Signal.Bot;

public record DisconnectionError(
    DisconnectionType DisconnectionType,
    WebSocketCloseStatus? CloseStatus,
    string? CloseStatusDescription = null,
    string? SubProtocol = null,
    Exception? Exception = null)
    : Error(Exception, ErrorType.DisconnectionHappened)
{
    internal Action? CancelReconnectionAction;
    internal Action? CancelClosingAction;

    public void CancelReconnection() => CancelReconnectionAction?.Invoke();
    public void CancelClosing() => CancelClosingAction?.Invoke();
}

public enum DisconnectionType
{
    Undefined = 0,
    ConnectionLost = 1,
    Timeout = 2,
    ClientInitiated = 3,
    ServerInitiated = 4,
    Shutdown = 5
}

internal static class DisconnectionTypeExtensions
{
    internal static Error To(this Disconnected info)
    {
        return new DisconnectionError(
            info.Reason.To(),
            WebSocketCloseStatus.Empty,
            string.Empty,
            string.Empty,
            info.Exception)
        {
            CancelClosingAction = () => _ = true,
            CancelReconnectionAction = () => _ = true
        };
    }

    private static DisconnectionType To(this DisconnectReason value)
    {
        return value switch
        {
            DisconnectReason.Undefined => DisconnectionType.Undefined,
            DisconnectReason.ConnectionLost => DisconnectionType.ConnectionLost,
            DisconnectReason.Timeout => DisconnectionType.Timeout,
            DisconnectReason.ClientInitiated => DisconnectionType.ClientInitiated,
            DisconnectReason.ServerInitiated => DisconnectionType.ServerInitiated,
            DisconnectReason.Shutdown => DisconnectionType.Shutdown,
            _ => DisconnectionType.Undefined
        };
    }
}