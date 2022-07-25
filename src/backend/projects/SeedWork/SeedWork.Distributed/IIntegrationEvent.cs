using System;

namespace SeedWork.Distributed;

public interface IIntegrationEvent
{
    Guid EventId { get; }

    DateTime CreationDate { get; }
}
