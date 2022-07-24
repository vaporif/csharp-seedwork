namespace SeedWork.Multiservice.Services;

using MassTransit;

public class FifoEntityNameFormatter : IEntityNameFormatter
{
    public string FormatEntityName<T>() => SnakeCaseEndpointNameFormatter.Instance.SanitizeName($"{typeof(T).Name}.fifo");
}
