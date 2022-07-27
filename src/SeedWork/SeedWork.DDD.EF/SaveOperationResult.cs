public record SaveOperationResult
{
    public SaveOperationResult(
        int affectedRows,
        IList<IAuditEntity> addedEntities,
        IList<IAuditEntity> updatedEntities)
    {
        AffectedRows = affectedRows;
        AddedEntities = addedEntities ?? throw new ArgumentNullException(nameof(addedEntities));
        UpdatedEntities = updatedEntities ?? throw new ArgumentNullException(nameof(updatedEntities));
    }

    public int AffectedRows { get; }
    public int AffectedRowsIncludingDeleted { get; }
    public IList<IAuditEntity> AddedEntities { get; } = Array.Empty<IAuditEntity>();
    public IList<IAuditEntity> UpdatedEntities { get; } = Array.Empty<IAuditEntity>();
}
