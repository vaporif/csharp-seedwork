namespace SeedWork.Multiservice.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Threading;
    using System.Threading.Tasks;
    using MassTransit;
    using MassTransit.AmazonSqsTransport;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class IntegrationEventService<TContext> : IIntegrationEventService
        where TContext : IBoundedDbContext
    {
        private const int LastCreatedEventsInMinutes = 30;

        private readonly IPublishEndpoint _bus;
        private readonly QueueConfiguration _config;
        private readonly TContext _context;
        private readonly IIntegrationEventOutboxService _outboxService;
        private readonly ILogger<IntegrationEventService<TContext>> _logger;

        public IntegrationEventService(
            IPublishEndpoint bus,
            TContext context,
            Func<DbConnection, IIntegrationEventOutboxService> outboxServiceFactory,
            IOptions<QueueConfiguration> config,
            ILogger<IntegrationEventService<TContext>> logger)
        {
            _bus = bus ?? throw new ArgumentNullException(nameof(bus));
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _outboxService = outboxServiceFactory(context.Connection);
            _config = config?.Value ?? throw new ArgumentNullException(nameof(config));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task PublishEventsThroughEventBusAsync(Guid transactionId)
        {
            var allEvents = await _outboxService.RetrieveEventLogsPendingToPublishAsync(transactionId);

            await PublishAsync(allEvents);
        }

        public async Task AddAndSaveEventAsync(IIntegrationEvent evt)
        {
            _logger.LogTrace(
                "----- Enqueuing integration event {IntegrationEventId} to repository ({@IntegrationEvent})", evt.EventId,
                evt);

            if (_context.CurrentTransaction == null)
            {
                throw new InvalidOperationException("transaction is required");
            }

            await _outboxService.AddEventAsync(evt, _context.CurrentTransaction.GetTransaction());
        }

        public async Task BulkInsertAndSaveAsync(IList<IIntegrationEvent> evt)
        {
            _logger.LogTrace(
                "----- Enqueuing integration events {IntegrationEventId} to repository ({@IntegrationEvent})",
                string.Join(',', evt.Select(x => x.EventId)), evt);

            if (_context.CurrentTransaction == null)
            {
                throw new InvalidOperationException("transaction is required");
            }

            await _outboxService.BulkInsertEventsAsync(evt, _context.CurrentTransaction.GetTransaction());
        }

        public async Task RepublishEventsAsync()
        {
            var lastCreated = TimeSpan.FromMinutes(LastCreatedEventsInMinutes);
            var beforeCreatedAt = DateTimeOffset.UtcNow.Subtract(lastCreated);
            var events = await _outboxService.RetrieveNotPublishedEventsAsync(beforeCreatedAt);
            await PublishAsync(events);
        }

        private async Task PublishAsync(IEnumerable<IntegrationEventLogEntry> entries)
        {
            var groups = entries
                .GroupBy(e => e.IntegrationEvent.GetType())
                .Select(e => new
                {
                    Type = e.Key,
                    Ids = e.Select(x => x.EventId).ToArray(),
                    Events = e.Select(x => x.IntegrationEvent).ToArray()
                })
                .ToArray();

            foreach (var group in groups)
            {
                _logger.LogTrace("Publishing events: {@ids}", group.Ids);
                await PublishEventsInternal(group.Ids, group.Events, group.Type);
            }
        }

        private async Task PublishEventsInternal(Guid[] logEntryIds, IEnumerable<IIntegrationEvent> events, Type type)
        {
            try
            {
                await _outboxService.SetInProgressStatusAsync(logEntryIds);
                await PublishBatchAsync(events, type);
                await _outboxService.SetPublishedStatusAsync(logEntryIds);

                _logger.LogTrace("Events successfully published: {@ids}", logEntryIds);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Publish integration events failed: {@ids}", logEntryIds);
                await _outboxService.SetFailedStatusAsync(logEntryIds);
            }
        }

        protected virtual async Task PublishBatchAsync(IEnumerable<object> messages, Type messageType, CancellationToken cancellationToken = default)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                await Task.WhenAll(messages.Select(x => _bus.Publish(x, messageType, cancellationToken)));
            }
            else
            {
                await Task.WhenAll(messages.Select(x => _bus.Publish(x, messageType, context => context.SetGroupId(_config.SqsQueueGroupId), cancellationToken)));
            }
        }
    }
}
