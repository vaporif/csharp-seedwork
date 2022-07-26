public interface IIntegrationEventService : IPublishIntegarationEventsService
{
    Task PublishEventsThroughEventBusAsync(Guid transactionId);

    Task AddAndSaveEventAsync(IIntegrationEvent evt);

    Task BulkInsertAndSaveAsync(IList<IIntegrationEvent> evt);
}
