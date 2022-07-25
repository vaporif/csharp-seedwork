namespace SeedWork.Distributed.Services;

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

    protected async ValueTask RepublishAsync()
    {
        _logger.LogTrace("Running republishing integration events job");
        await _service.RepublishEventsAsync();
        _logger.LogTrace("Republish integration events successfully completed");
    }
}
