using System;
using System.Net;

namespace Signal.Bot.Exceptions;

public class RequestException : Exception
{
    public HttpStatusCode? HttpStatusCode { get; }

    public RequestException(string message) : base(message)
    {
    }

    public RequestException(string message, Exception innerException) : base(message, innerException)
    {
    }

    public RequestException(string message, HttpStatusCode httpStatusCode) : base(message)
        => HttpStatusCode = httpStatusCode;

    public RequestException(string message, HttpStatusCode httpStatusCode, Exception? innerException)
        : base(message, innerException) => HttpStatusCode = httpStatusCode;
}