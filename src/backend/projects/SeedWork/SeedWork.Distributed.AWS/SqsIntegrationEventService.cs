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

namespace SeedWork.Distributed;

public class SqsIntegrationEventService<TContext> : IIntegrationEventService
    where TContext : DbContext
{
    private readonly IOptions<QueueConfiguration> _config;

    public IntegrationEventService(
        IOptions<QueueConfiguration> config,
        IPublishEndpoint bus,
        TContext context,
        Func<DbConnection, IIntegrationEventOutboxService> outboxServiceFactory,
        ILogger<SqsIntegrationEventService<TContext>> logger) : base(bus, context, outboxServiceFactory) 

    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
    }

    protected override async Task PublishBatchAsync(IEnumerable<object> messages, Type messageType, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(messages.Select(x => _bus.Publish(x, messageType, context => context.SetGroupId(_config.Value.SqsQueueGroupId), cancellationToken)));
    }
}
