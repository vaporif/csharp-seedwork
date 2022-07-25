using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public abstract class DomainEventHandlerWrapper
{
    public abstract ValueTask HandleAsync(DomainEvent domainEvent, CancellationToken cancellationToken, ServiceFactory serviceFactory,
        Func<IEnumerable<Func<DomainEvent, CancellationToken, ValueTask>>, DomainEvent, CancellationToken, ValueTask> publish);
}

public class DomainEventHandlerWrapperImpl<TDomainEvent> : DomainEventHandlerWrapper
    where TDomainEvent : DomainEvent
{
    public override ValueTask HandleAsync(DomainEvent domainEvent, CancellationToken cancellationToken, ServiceFactory serviceFactory,
        Func<IEnumerable<Func<DomainEvent, CancellationToken, ValueTask>>, DomainEvent, CancellationToken, ValueTask> publish)
    {
        var handlers = serviceFactory
            .GetInstances<IDomainEventHandler<TDomainEvent>>()
            .Select(x => new Func<DomainEvent, CancellationToken, ValueTask>((theDomainEvent, theCancellationToken) => x.HandleAsync((TDomainEvent)theDomainEvent, theCancellationToken)));

        return publish(handlers, domainEvent, cancellationToken);
    }
}
