public interface IIntegrationEvent
{
    Guid EventId { get; }

    DateTime CreationDate { get; }
}
