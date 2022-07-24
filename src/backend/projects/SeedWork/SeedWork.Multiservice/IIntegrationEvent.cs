using System;

namespace SeedWork.Multiservice;

public interface IIntegrationEvent
{
    Guid EventId { get; }

    DateTime CreationDate { get; }
}
