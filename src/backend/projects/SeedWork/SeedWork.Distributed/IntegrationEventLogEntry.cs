using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json;

public class IntegrationEventLogEntry
{
    private IntegrationEventLogEntry() { }

    public IntegrationEventLogEntry(IIntegrationEvent @event, Guid transactionId, bool sendInBatch = false)
    {
        if (@event == null)
        {
            throw new ArgumentNullException(nameof(@event));
        }

        if (transactionId == default)
        {
            throw new ArgumentOutOfRangeException(nameof(transactionId));
        }

        EventId = @event.EventId;
        CreationTime = @event.CreationDate;
        EventTypeName = @event.GetType()!.FullName;
        Content = JsonSerializer.Serialize(@event, @event.GetType(), new JsonSerializerOptions
        {
            WriteIndented = true
        });
        TransactionId = transactionId.ToString();
        SendInBatch = sendInBatch;
    }
    public Guid EventId { get; private set; } = Guid.Empty;
    public string EventTypeName { get; private set; } = string.Empty;
    [NotMapped]
    public string EventTypeShortName => EventTypeName.Split('.')?.Last();
    [NotMapped]
    public IIntegrationEvent? IntegrationEvent { get; private set; }
    public EventState State { get; set; } = EventState.NotPublished;

    public bool SendInBatch { get; private set; }
    public int TimesSent { get; set; } = 0;
    public DateTime CreationTime { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public string TransactionId { get; private set; } = string.Empty;

    public IntegrationEventLogEntry DeserializeJsonContent(Type type)
    {
        IntegrationEvent = JsonSerializer.Deserialize(Content, type, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) as IIntegrationEvent;
        return this;
    }
}
