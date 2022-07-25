using System;
using System.Threading.Tasks;
using D3SK.NetCore.Common.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MS.Common.Infrastructure.EventBus;
using MS.Common.Infrastructure.EventBus.Services;
using Serilog.Context;

namespace MS.Common.Infrastructure.Domain;

public class CommandRunnerWithEvents<TContext>
    where TContext : DbContext
{
    private readonly ILogger<CommandRunnerWithEvents<TContext>> _logger;
    private readonly TContext _dbContext;
    private readonly IIntegrationEventService _integrationEventService;

    public CommandRunnerWithEvents(TContext dbContext,
        IIntegrationEventService integrationEventService,
        ILogger<CommandRunnerWithEvents<TContext>> logger)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _integrationEventService = integrationEventService ?? throw new ArgumentNullException(nameof(integrationEventService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task WithTransactionAndBusAsync(Func<Task> action)
    {
        try
        {
            var strategy = _dbContext.CreateExecutionStrategy();

            await strategy.ExecuteAsync(async () =>
            {
                Guid transactionId;

                using (var transaction = await _dbContext.BeginTransactionAsync())
                using (LogContext.PushProperty("TransactionContext", transaction.TransactionId))
                {
                    await action();

                    _dbContext.CommitTransaction();

                    transactionId = transaction.TransactionId;
                }

                await _integrationEventService.PublishEventsThroughEventBusAsync(transactionId);
            });

            return;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "ERROR Handling transaction");

            throw;
        }
    }
}
