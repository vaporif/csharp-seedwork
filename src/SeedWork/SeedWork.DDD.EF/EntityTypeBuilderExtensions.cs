using Microsoft.EntityFrameworkCore.Metadata.Builders;

public static class EntityTypeBuilderExtensions
{
    public static void HasDeletedQueryFilter<T>(this EntityTypeBuilder<T> entity)
        where T : class, ISoftDeleteEntity
    {
        entity.HasQueryFilter(x => !x.IsDeleted);
    }
}
