namespace SeedWork.Multiservice.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore.Storage;

    public interface IIntegrationEventOutboxService
    {
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId);
        Task<IEnumerable<IntegrationEventLogEntry>> RetrieveNotPublishedEventsAsync(DateTimeOffset beforeCreatedAt);

        Task AddEventAsync(IIntegrationEvent @event, IDbContextTransaction transaction);
        Task MarkEventAsPublishedAsync(Guid eventId);
        Task MarkEventAsInProgressAsync(Guid eventId);
        Task MarkEventAsFailedAsync(Guid eventId);
        Task BulkInsertEventsAsync(IList<IIntegrationEvent> @event, IDbContextTransaction transaction);

        Task SetPublishedStatusAsync(Guid[] eventIds);
        Task SetInProgressStatusAsync(Guid[] eventIds);
        Task SetFailedStatusAsync(Guid[] eventIds);

    }
}
