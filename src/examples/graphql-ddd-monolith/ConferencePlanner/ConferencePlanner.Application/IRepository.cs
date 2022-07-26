public interface IRepository<T> where T : AggregateRoot
{
    ValueTask<T?> FindAsync(int id);
    IQueryable<T> GetQueryable();
    ValueTask<T> AddAsync(T entity, CancellationToken ct = default);
    bool Delete(int id);

    ValueTask SaveChangesAsync(CancellationToken ct = default);
}
