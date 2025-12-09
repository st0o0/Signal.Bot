using Signal.Bot.Polling;
using Websocket.Client;

namespace Signal.Bot;

public record ReconnectionInfoError(ReconnectionInfo Info)
    : Error(null, ErrorSource.ReconnectionHappened);