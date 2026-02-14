using System.Net.WebSockets;
using WebSocket.Rx;

namespace Signal.Bot.Internal;

internal static class Extensions
{
    internal static Error To(this Connected info)
    {
        return new ConnectionError(info.Reason.To());
    }

    internal static Error To(this Disconnected info)
    {
        return new DisconnectionError(
            info.Reason.To(),
            WebSocketCloseStatus.Empty,
            string.Empty,
            string.Empty,
            info.Exception)
        {
            CancelClosingAction = info.CancelClosing,
            CancelReconnectionAction = info.CancelReconnection
        };
    }

    private static DisconnectionEvent To(this DisconnectReason value)
    {
        return value switch
        {
            DisconnectReason.Undefined => DisconnectionEvent.Undefined,
            DisconnectReason.ClientInitiated => DisconnectionEvent.ClientInitiated,
            DisconnectReason.ServerInitiated => DisconnectionEvent.ServerInitiated,
            DisconnectReason.TimedOut => DisconnectionEvent.TimedOut,
            DisconnectReason.Dropped => DisconnectionEvent.Dropped,
            DisconnectReason.Closed => DisconnectionEvent.Closed,
            _ => DisconnectionEvent.Undefined
        };
    }

    private static ConnectionEvent To(this ConnectReason value)
    {
        return value switch
        {
            ConnectReason.Undefined => ConnectionEvent.Undefined,
            ConnectReason.Initialized => ConnectionEvent.Initialized,
            ConnectReason.Reconnected => ConnectionEvent.Reconnected,
            _ => ConnectionEvent.Undefined
        };
    }
    
    internal static QueryParameterRegistry AsQueryParameter(this ReceiverOptions options)
    {
        var result = new QueryParameterRegistry();
        result.Add("timeout", options.Timeout, x => x.Seconds.ToString());
        result.Add("ignore_attachments", options.IgnoreAttachments);
        result.Add("ignore_stories", options.IgnoreStories);
        result.Add("max_messages", options.MaxMessages);
        result.Add("send_read_receipts", options.SendReadReceipts);

        return result;
    }
}