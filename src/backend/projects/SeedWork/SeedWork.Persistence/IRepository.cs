using System;

namespace SeedWork.Persistence;

public interface IRepository<TEntity, out TDbContext>
    where TEntity : AggregateRoot
    where TDbContext : IDbContext
{
    TDbContext DbContext { get; }
}
