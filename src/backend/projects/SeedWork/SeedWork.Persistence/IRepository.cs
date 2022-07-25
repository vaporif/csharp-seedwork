using System;
using SeedWork.DDD;
using Microsoft.EntityFrameworkCore;

public interface IRepository<TEntity, out TDbContext>
    where TEntity : AggregateRoot
    where TDbContext : DbContext
{
    TDbContext DbContext { get; }
}
