using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using GreenDonut;
using api.Data;
using api.Data.Entities;

namespace api.DataLoader
{
    public class EmployeeByIdDataLoader : BatchDataLoader<int, Employee>
    {
        private readonly IDbContextFactory<AppDbContext> _dbContextFactory;

        public EmployeeByIdDataLoader(
            IBatchScheduler batchScheduler, 
            IDbContextFactory<AppDbContext> dbContextFactory)
            : base(batchScheduler)
        {
            _dbContextFactory = dbContextFactory ?? 
                throw new ArgumentNullException(nameof(dbContextFactory));
        }

        protected override async Task<IReadOnlyDictionary<int, Employee>> LoadBatchAsync(
            IReadOnlyList<int> keys, 
            CancellationToken cancellationToken)
        {
            await using AppDbContext dbContext = 
                _dbContextFactory.CreateDbContext();
            
            return await dbContext.Employees
                .Where(s => keys.Contains(s.Id))
                .ToDictionaryAsync(t => t.Id, cancellationToken);
        }
    }
}
