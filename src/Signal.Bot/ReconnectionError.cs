using Signal.Bot.Polling;
using Websocket.Client;

namespace Signal.Bot;

public record ReconnectionError(ReconnectionType Type)
    : Error(null, ErrorSource.ReconnectionHappened);

public enum ReconnectionType
{
    Undefined = 0,

    /// <summary>
    /// Type used for initial connection to websocket stream
    /// </summary>
    Initial = 1,

    /// <summary>
    /// Type used when connection to websocket was lost in meantime
    /// </summary>
    Lost = 2,

    /// <summary>
    /// Type used when connection to websocket was lost by not receiving any message in given time-range
    /// </summary>
    NoMessageReceived = 3,

    /// <summary>
    /// Type used after unsuccessful previous reconnection
    /// </summary>
    Error = 4,

    /// <summary>
    /// Type used when reconnection was requested by user
    /// </summary>
    ByUser = 5,

    /// <summary>
    /// Type used when reconnection was requested by server
    /// </summary>
    ByServer = 6
}

internal static class ReconnectionTypeExtensions
{
    internal static ReconnectionError To(this ReconnectionInfo info)
    {
        return new ReconnectionError(info.Type.To());
    }

    internal static ReconnectionType To(this Websocket.Client.ReconnectionType value)
    {
        return value switch
        {
            Websocket.Client.ReconnectionType.Initial => ReconnectionType.Initial,
            Websocket.Client.ReconnectionType.Lost => ReconnectionType.Lost,
            Websocket.Client.ReconnectionType.NoMessageReceived => ReconnectionType.NoMessageReceived,
            Websocket.Client.ReconnectionType.Error => ReconnectionType.Error,
            Websocket.Client.ReconnectionType.ByUser => ReconnectionType.ByUser,
            Websocket.Client.ReconnectionType.ByServer => ReconnectionType.ByServer,
            _ => ReconnectionType.Undefined
        };
    }
}