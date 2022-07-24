using System;

namespace SeedWork.Persistence;

public interface IRepository<out TDbContext> where TDbContext : IDbContext
{

}
