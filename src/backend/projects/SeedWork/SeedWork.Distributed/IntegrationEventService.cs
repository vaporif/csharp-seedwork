global using System;
global using MassTransit;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Logging;
global using System.Threading;
global using System.Threading.Tasks;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Options;


// TODO: Add republish
public class IntegrationEventService<TContext> : IIntegrationEventService
    where TContext : DbContext
{
    private const int LastCreatedEventsInMinutes = 30;
    private readonly TContext _context;
    private readonly IIntegrationEventOutboxService _outboxService;
    private readonly ILogger<IntegrationEventService<TContext>> _logger;

    protected readonly IPublishEndpoint Bus;

    public IntegrationEventService(
        IPublishEndpoint bus,
        TContext context,
        Func<DbConnection, IIntegrationEventOutboxService> outboxServiceFactory,
        ILogger<IntegrationEventService<TContext>> logger)
    {
        Bus = bus ?? throw new ArgumentNullException(nameof(bus));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _outboxService = outboxServiceFactory(context.Database.GetDbConnection());
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

        if (_context.Database.CurrentTransaction == null)
        {
            throw new InvalidOperationException("transaction is required");
        }

        await _outboxService.AddEventAsync(evt, _context.Database.CurrentTransaction);
    }

    public async Task BulkInsertAndSaveAsync(IList<IIntegrationEvent> evt)
    {
        _logger.LogTrace(
            "----- Enqueuing integration events {IntegrationEventId} to repository ({@IntegrationEvent})",
            string.Join(',', evt.Select(x => x.EventId)), evt);

        if (_context.Database.CurrentTransaction == null)
        {
            throw new InvalidOperationException("transaction is required");
        }

        await _outboxService.BulkInsertEventsAsync(evt, _context.Database.CurrentTransaction);
    }

    public async ValueTask PulishEventsAsync()
    {
        var lastCreated = TimeSpan.FromMinutes(LastCreatedEventsInMinutes);
        var beforeCreatedAt = DateTimeOffset.UtcNow.Subtract(lastCreated);
        var events = await _outboxService.RetrieveNotPublishedEventsAsync(beforeCreatedAt);
        await PublishAsync(events);
    }

    private async Task PublishAsync(IEnumerable<IntegrationEventLogEntry> entries)
    {
        var groups = entries
            .GroupBy(e => e.IntegrationEvent!.GetType())
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
            await PublishEventsInternal(group.Ids, group.Events!, group.Type!);
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
        await Task.WhenAll(messages.Select(x => Bus.Publish(x, messageType, cancellationToken)));
    }
}
