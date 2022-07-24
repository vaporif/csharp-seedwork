namespace SeedWork.Multiservice.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IIntegrationEventService
    {
        Task PublishEventsThroughEventBusAsync(Guid transactionId);

        Task RepublishEventsAsync();

        Task AddAndSaveEventAsync(IIntegrationEvent evt);

        Task BulkInsertAndSaveAsync(IList<IIntegrationEvent> evt);
    }
}
