using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainEventsDispatcher(this IServiceCollection services)
    {
        services.TryAddTransient<ServiceFactory>(p => p.GetRequiredService);
        services.TryAddScoped<IDomainEventDispatcher, DomainEventDispatcher>();
        return services;
    }
}
