using System;
using Signal.Bot.Polling;

namespace Signal.Bot;

public record Error(Exception? Exception, ErrorSource Source) : IError;