using Signal.Bot.Polling;
using Websocket.Client;

namespace Signal.Bot;

public record DisconnectionInfoError(DisconnectionInfo Info)
    : Error(Info.Exception, ErrorSource.DisconnectionHappened);