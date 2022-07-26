using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

// NOTE: This class is used by the framework to dispatch events. Is a service locator.
public sealed class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly ServiceFactory _serviceFactory;

    private static readonly ConcurrentDictionary<Type, DomainEventHandlerWrapper> _eventHandlers = new();

    public DomainEventDispatcher(ServiceFactory serviceFactory)
    {
        _serviceFactory = serviceFactory ?? throw new ArgumentNullException(nameof(serviceFactory));
    }

    public ValueTask DispatchAsync(DomainEvent @event, CancellationToken ct = default)
    {
        var domainEventType = @event.GetType();
        var handler = _eventHandlers.GetOrAdd(domainEventType,
            static t => (DomainEventHandlerWrapper) (Activator.CreateInstance(typeof(DomainEventHandlerWrapperImpl<>).MakeGenericType(t)) 
                ?? throw new InvalidOperationException($"Could not create wrapper for type {t}")));

        return handler.HandleAsync(@event, ct, _serviceFactory, PublishCoreAsync);;
    }

    private async ValueTask PublishCoreAsync(IEnumerable<Func<DomainEvent, CancellationToken, ValueTask>> allHandlers, DomainEvent @event, CancellationToken cancellationToken)
    {
        foreach (var handler in allHandlers)
        {
            await handler(@event, cancellationToken).ConfigureAwait(false);
        }
    }
}
