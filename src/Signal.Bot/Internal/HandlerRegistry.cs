using System;
using System.Threading;
using System.Threading.Tasks;

namespace Signal.Bot.Internal;

public interface IHandlerRegistry<TIn>
{
    void Register(Func<TIn, CancellationToken, Task> handler);
    void Register(Action<TIn> handler);

    Task<bool> Handle(TIn value, CancellationToken cancellationToken = default);
};

public class HandlerRegistry<TIn> : IHandlerRegistry<TIn>
{
    private readonly List<Func<TIn, CancellationToken, Task>> _handlers = [];

    public void Register(Func<TIn, CancellationToken, Task> handler)
        => _handlers.Add(handler);

    public void Register(Action<TIn> handler)
    {
        _handlers.Add(async (args, _) =>
        {
            handler(args);
            await Task.CompletedTask;
        });
    }

    public async Task<bool> Handle(TIn value, CancellationToken cancellationToken = default)
    {
        if (_handlers.Count == 0) return false;
        foreach (var handler in _handlers)
        {
            await handler.Invoke(value, cancellationToken);
        }
        return true;
    }
}