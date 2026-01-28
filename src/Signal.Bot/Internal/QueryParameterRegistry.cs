using System;
using System.Text;

namespace Signal.Bot.Internal;

public interface IQueryParameterRegistry
{
    T AddAndGet<T>(string valueName, T value, Func<T, string>? valueFactory = null) where T : notnull;
    void Add<T>(string valueName, T value, Func<T, string>? valueFactory = null) where T : notnull;
    string Build();
}

public class QueryParameterRegistry : IQueryParameterRegistry
{
    private readonly StringBuilder _builder = new(string.Empty);

    public T AddAndGet<T>(string valueName, T value, Func<T, string>? valueFactory = null) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(valueName);
        ArgumentNullException.ThrowIfNull(value);
        Add(valueName, value, valueFactory);
        return value;
    }

    public void Add<T>(string valueName, T value, Func<T, string>? valueFactory = null) where T : notnull
    {
        ArgumentNullException.ThrowIfNull(valueName);
        ArgumentNullException.ThrowIfNull(value);
        var valueAsString = value as string ?? valueFactory?.Invoke(value) ?? value.ToString();
        var queryParameter = $"{valueName}={valueAsString}";
        if (_builder.Length == 0)
        {
            queryParameter = "?" + queryParameter;
        }
        else
        {
            queryParameter = "&" + queryParameter;
        }

        _builder.Append(queryParameter);
    }

    public string Build()
        => _builder.ToString();
}