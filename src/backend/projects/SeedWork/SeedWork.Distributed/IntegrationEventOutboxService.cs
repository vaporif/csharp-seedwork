using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace SeedWork.Distributed.Services;

public class IntegrationEventOutboxService<TLogDbStore> : IIntegrationEventOutboxService, IDisposable
    where TLogDbStore : IntegrationEventLogDbContext
{
    private readonly IntegrationEventLogDbContext _integrationEventLogContext;
    private volatile bool disposedValue;

    private readonly Lazy<List<Type>> _integrationTypes;

    // NOTE: We need to inject dbconnection of worker dbcontext
    // to ensure same connection and transaction within business operation + outbox event save
    public IntegrationEventOutboxService(DbContextOptions<TLogDbStore> options)
    {
        // TODO: Using activator to create instance is overkill, simplify it
        _integrationEventLogContext = (TLogDbStore)Activator.CreateInstance(
            typeof(TLogDbStore), options);

        _integrationTypes = new Lazy<List<Type>>(AppDomain.CurrentDomain.GetAssemblies().SelectMany(c => c.GetTypes())
            .Where(t => t.IsAssignableTo(typeof(IIntegrationEvent)) && !t.IsAbstract)
            .ToList());
    }

    public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveEventLogsPendingToPublishAsync(Guid transactionId)
    {
        var tid = transactionId.ToString();

        var result = await _integrationEventLogContext.IntegrationEventLogs
            .Where(e => e.TransactionId == tid && e.State == EventState.NotPublished).ToListAsyncEF();

        if (result != null && result.Any())
        {
            return result.OrderBy(o => o.CreationTime)
                .Select(e => e.DeserializeJsonContent(_integrationTypes.Value.Find(t => t.Name == e.EventTypeShortName)!));
        }

        return Enumerable.Empty<IntegrationEventLogEntry>();
    }

    public async Task<IEnumerable<IntegrationEventLogEntry>> RetrieveNotPublishedEventsAsync(DateTimeOffset beforeCreatedAt)
    {
        var events = await _integrationEventLogContext.IntegrationEventLogs
            .AsNoTracking()
            .Where(e => e.CreationTime >= beforeCreatedAt && e.State != EventState.Published)
            .OrderBy(e => e.CreationTime)
            .ToListAsyncEF();

        return events.Select(e => e.DeserializeJsonContent(_integrationTypes.Value.Find(t => t.Name == e.EventTypeShortName)));
    }

    public async Task AddEventAsync(IIntegrationEvent @event, IDbContextTransaction transaction)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        var eventLogEntry = new IntegrationEventLogEntry(@event, transaction.TransactionId);

        if (_integrationEventLogContext.Database.CurrentTransaction == null)
        {
            // TODO: Why without overriding transaction id it's different from passed transaction id?
            await _integrationEventLogContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), transaction.TransactionId);
        }
        else if (_integrationEventLogContext.Database.CurrentTransaction.TransactionId != transaction.TransactionId)
        {
            throw new InvalidOperationException($"Integration events should be saved within same transaction. Event transaction {transaction.TransactionId}. Current DB transaction {_integrationEventLogContext.Database.CurrentTransaction.TransactionId}");
        }

        _integrationEventLogContext.IntegrationEventLogs.Add(eventLogEntry);

        await _integrationEventLogContext.SaveChangesAsync();
    }

    public async Task BulkInsertEventsAsync(IList<IIntegrationEvent> @event, IDbContextTransaction transaction)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        var eventLogEntrys = new List<IntegrationEventLogEntry>(
            @event.Select(x => new IntegrationEventLogEntry(x, transaction.TransactionId, sendInBatch: true))
        );

        if (_integrationEventLogContext.Database.CurrentTransaction == null)
        {
            // TODO: Why without overriding transaction id it's different from passed transaction id?
            await _integrationEventLogContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), transaction.TransactionId);
        }
        else if (_integrationEventLogContext.Database.CurrentTransaction.TransactionId != transaction.TransactionId)
        {
            throw new InvalidOperationException($"Integration events should be saved within same transaction. Event transaction {transaction.TransactionId}. Current DB transaction {_integrationEventLogContext.Database.CurrentTransaction.TransactionId}");
        }

        await _integrationEventLogContext.BulkCopyAsync(new BulkCopyOptions { BulkCopyType = BulkCopyType.MultipleRows }, eventLogEntrys);

        await _integrationEventLogContext.SaveChangesAsync();
    }

    public Task MarkEventAsPublishedAsync(Guid eventId) => UpdateEventStatusAsync(eventId, EventState.Published);

    public Task MarkEventAsInProgressAsync(Guid eventId) => UpdateEventStatusAsync(eventId, EventState.InProgress);

    public Task MarkEventAsFailedAsync(Guid eventId) => UpdateEventStatusAsync(eventId, EventState.PublishedFailed);

    public Task SetPublishedStatusAsync(Guid[] eventIds) => UpdateEventStatusAsync(eventIds, EventState.Published);

    public Task SetInProgressStatusAsync(Guid[] eventIds) => UpdateEventStatusAsync(eventIds, EventState.InProgress);

    public Task SetFailedStatusAsync(Guid[] eventIds) => UpdateEventStatusAsync(eventIds, EventState.PublishedFailed);

    private Task UpdateEventStatusAsync(Guid eventId, EventState status)
    {
        var eventLogEntry = _integrationEventLogContext.IntegrationEventLogs.Single(ie => ie.EventId == eventId);
        eventLogEntry.State = status;

        if (status == EventState.InProgress)
        {
            eventLogEntry.TimesSent++;
        }

        _integrationEventLogContext.IntegrationEventLogs.Update(eventLogEntry);

        return _integrationEventLogContext.SaveChangesAsync();
    }

    private async Task UpdateEventStatusAsync(Guid[] eventIds, EventState status)
    {
        await _integrationEventLogContext
            .IntegrationEventLogs
            .ToLinqToDBTable()
            .Where(t => eventIds.Contains(t.EventId))
            .Set(t => t.State, status)
            .UpdateAsync();
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                _integrationEventLogContext?.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
