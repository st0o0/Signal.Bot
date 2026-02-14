using System.Text;

namespace Signal.Bot.Internal;

internal sealed class QueryParameterRegistry : IQueryParameterRegistry
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
        var valueAsString = valueFactory?.Invoke(value) ?? value.ToString()!;
        var queryParameter = $"{valueName}={Uri.EscapeDataString(valueAsString)}";
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

    public void AddRange<T>(string valueName, IEnumerable<T> values, Func<T, string>? valueFactory = null)
        where T : notnull
    {
        foreach (var value in values)
        {
            Add(valueName, value, valueFactory);
        }
    }

    public string Build() => _builder.ToString();
}