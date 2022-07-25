public interface IIntegrationEventService
{
    Task PublishEventsThroughEventBusAsync(Guid transactionId);

    Task RepublishEventsAsync();

    Task AddAndSaveEventAsync(IIntegrationEvent evt);

    Task BulkInsertAndSaveAsync(IList<IIntegrationEvent> evt);
}
