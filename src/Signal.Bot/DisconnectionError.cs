using System;
using System.Net.WebSockets;
using Signal.Bot.Polling;
using Websocket.Client;

namespace Signal.Bot;

public record DisconnectionError(
    DisconnectionType Type,
    WebSocketCloseStatus? CloseStatus,
    string? CloseStatusDescription = null,
    string? SubProtocol = null,
    Exception? Exception = null)
    : Error(Exception, ErrorSource.DisconnectionHappened)
{
    internal Action? CancelReconnectionAction;
    internal Action? CancelClosingAction;

    public void CancelReconnection() => CancelReconnectionAction?.Invoke();
    public void CancelClosing() => CancelClosingAction?.Invoke();
}

public enum DisconnectionType
{
    Undefined = 0,

    /// <summary>
    /// Type used for exit event, disposing of the websocket client
    /// </summary>
    Exit = 1,

    /// <summary>
    /// Type used when connection to websocket was lost in meantime
    /// </summary>
    Lost = 2,

    /// <summary>
    /// Type used when connection to websocket was lost by not receiving any message in given time-range
    /// </summary>
    NoMessageReceived = 3,

    /// <summary>
    /// Type used when connection or reconnection returned error
    /// </summary>
    Error = 4,

    /// <summary>
    /// Type used when disconnection was requested by user
    /// </summary>
    ByUser = 5,

    /// <summary>
    /// Type used when disconnection was requested by server
    /// </summary>
    ByServer = 6
}

internal static class DisconnectionTypeExtensions
{
    internal static DisconnectionError To(this DisconnectionInfo info)
    {
        return new DisconnectionError(
            info.Type.To(),
            info.CloseStatus,
            info.CloseStatusDescription,
            info.SubProtocol,
            info.Exception)
        {
            CancelClosingAction = () => info.CancelClosing = true,
            CancelReconnectionAction = () => info.CancelReconnection = true
        };
    }

    internal static DisconnectionType To(this Websocket.Client.DisconnectionType value)
    {
        return value switch
        {
            Websocket.Client.DisconnectionType.Exit => DisconnectionType.Exit,
            Websocket.Client.DisconnectionType.Lost => DisconnectionType.Lost,
            Websocket.Client.DisconnectionType.NoMessageReceived => DisconnectionType.NoMessageReceived,
            Websocket.Client.DisconnectionType.Error => DisconnectionType.Error,
            Websocket.Client.DisconnectionType.ByUser => DisconnectionType.ByUser,
            Websocket.Client.DisconnectionType.ByServer => DisconnectionType.ByServer,
            _ => DisconnectionType.Undefined
        };
    }
}