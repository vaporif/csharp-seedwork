namespace SeedWork.Multiservice
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text.Json;

    public class IntegrationEventLogEntry
    {
        private IntegrationEventLogEntry() { }

        public IntegrationEventLogEntry(IIntegrationEvent @event, Guid transactionId, bool sendInBatch = false)
        {
            EventId = @event.EventId;
            CreationTime = @event.CreationDate;
            EventTypeName = @event.GetType().FullName;
            Content = JsonSerializer.Serialize(@event, @event.GetType(), new JsonSerializerOptions
            {
                WriteIndented = true
            });
            State = EventState.NotPublished;
            TimesSent = 0;
            TransactionId = transactionId.ToString();
            SendInBatch = sendInBatch;
        }
        public Guid EventId { get; private set; }
        public string EventTypeName { get; private set; }
        [NotMapped]
        public string EventTypeShortName => EventTypeName.Split('.')?.Last();
        [NotMapped]
        public IIntegrationEvent IntegrationEvent { get; private set; }
        public EventState State { get; set; }

        public bool SendInBatch { get; private set; }
        public int TimesSent { get; set; }
        public DateTime CreationTime { get; private set; }
        public string Content { get; private set; }
        public string TransactionId { get; private set; }

        public IntegrationEventLogEntry DeserializeJsonContent(Type type)
        {
            IntegrationEvent = JsonSerializer.Deserialize(Content, type, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true }) as IIntegrationEvent;
            return this;
        }
    }
}
