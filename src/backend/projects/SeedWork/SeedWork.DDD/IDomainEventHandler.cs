using MediatR;

public interface IDomainEventHandler<TEvent> : INotificationHandler<TEvent> where TEvent : DomainEvent

{
}
