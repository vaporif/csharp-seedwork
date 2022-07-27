public record SaveOperationResult
{
    public SaveOperationResult(
        int affectedRows,
        EntityUpdates[] updates)
    {
        AffectedRows = affectedRows;
        Updates = updates ?? throw new ArgumentNullException(nameof(updates));
    }

    public int AffectedRows { get; }
    public EntityUpdates[] Updates { get; } = Array.Empty<EntityUpdates>();
}
