using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.AmazonSqsTransport;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SeedWork.Distributed;

namespace SeedWork.Distributed;

public sealed class SqsIntegrationEventService<TContext> : IntegrationEventService<TContext>
    where TContext : DbContext
{
    private readonly IOptions<QueueConfiguration> _config;

    public SqsIntegrationEventService(
        IOptions<QueueConfiguration> config,
        IPublishEndpoint bus,
        TContext context,
        Func<DbConnection, IIntegrationEventOutboxService> outboxServiceFactory,
        ILogger<SqsIntegrationEventService<TContext>> logger) : base(bus, context, outboxServiceFactory, logger)

    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    protected override async Task PublishBatchAsync(IEnumerable<object> messages, Type messageType, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(messages.Select(x => Bus.Publish(x, messageType, context => context.SetGroupId(_config.Value.SqsQueueGroupId), cancellationToken)));
    }
}
