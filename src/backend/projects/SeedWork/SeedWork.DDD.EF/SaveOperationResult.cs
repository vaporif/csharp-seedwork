public record SaveOperationResult
{
    public SaveOperationResult(
        int affectedRows, 
        IList<AuditEntity> addedEntities, 
        IList<AuditEntity> updatedEntities)
    {
        AffectedRows = affectedRows;
        AffectedRowsIncludingDeleted = affectedRowsIncludingDeleted;
        AddedEntities = addedEntities ?? throw new ArgumentNullException(nameof(addedEntities));
        UpdatedEntities = updatedEntities ?? throw new ArgumentNullException(nameof(updatedEntities));
    }
    
    public int AffectedRows { get; }
    public int AffectedRowsIncludingDeleted { get; }
    public IList<AuditEntity> AddedEntities { get; } = List.Empty<AuditEntity>();
    public IList<AuditEntity> UpdatedEntities { get; } = List.Empty<AuditEntity>();
}
