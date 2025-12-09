using System;
using Signal.Bot.Polling;

namespace Signal.Bot;

public interface IError
{
    Exception? Exception { get; }
    ErrorSource Source { get; }
}