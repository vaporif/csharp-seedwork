public record EntityUpdates
{
    public object Entity { get; }

    public object? EntityId { get; }

    public EntityChangeState State { get; }

    public UpdatedEntityPropertyChange[] PropertyChanges { get; } = Array.Empty<UpdatedEntityPropertyChange>();

    public EntityUpdates(object entity)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        Entity = entity;
        State = EntityChangeState.Add;
    }

    public EntityUpdates(object entity, object entityId)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        Entity = entity;
        EntityId = entityId;
        State = EntityChangeState.Delete;
    }

    public EntityUpdates(object entity, object entityId, UpdatedEntityPropertyChange[] changes)
    {
        ArgumentNullException.ThrowIfNull(entity, nameof(entity));
        Entity = entity;
        EntityId = entityId;
        PropertyChanges = changes;
        State = EntityChangeState.Update;
    }
}
