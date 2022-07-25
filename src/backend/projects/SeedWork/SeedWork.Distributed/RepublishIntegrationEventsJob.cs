namespace SeedWork.Distributed.Services;

using System.Diagnostics.CodeAnalysis;
using Common.Infrastructure.BackgroundJobs;
using Common.Infrastructure.EventBus;
using Common.Infrastructure.EventBus.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

public class RepublishIntegrationEventsJob
{
    private readonly IIntegrationEventService _service;
    private readonly ILogger _logger;

    public RepublishIntegrationEventsJob(
        IIntegrationEventService service,
        ILogger<RepublishIntegrationEventsJob> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected async Task Execute(IJobExecutionContext context)
    {
        _logger.LogTrace("Running republishing integration events job");
        await _service.RepublishEventsAsync();
        _logger.LogTrace("Republish integration events successfully completed");
    }
}
