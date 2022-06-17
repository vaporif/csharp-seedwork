using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using HotChocolate;
using HotChocolate.Types;
using api.Data.Entities;
using api.Data;
using api.DataLoader;

namespace api.Types
{
    public class DivisionType : ObjectType<Division>
    {
        protected override void Configure(IObjectTypeDescriptor<Division> descriptor)
        {
            descriptor
                .Field(t => t.Employees)
                .ResolveWith<EmployeeResolvers>(t => t.GetEmployeesAsync(default!, default!, default!, default))
                .UseDbContext<AppDbContext>()
                .Name(nameof(Division.Employees).ToLower());
        }

        private class EmployeeResolvers
        {
            public async Task<IEnumerable<Employee>> GetEmployeesAsync(
                Division Division,
                [ScopedService] AppDbContext dbContext,
                EmployeeByIdDataLoader employeeById,
                CancellationToken cancellationToken)
            {
                int[] sessionIds = await dbContext.Divisions
                    .Where(s => s.Id == Division.Id)
                    .Include(s => s.Employees)
                    .SelectMany(s => s.Employees.Select(t => t.EmployeeId))
                    .ToArrayAsync();

                return await employeeById.LoadAsync(sessionIds, cancellationToken);
            }
        }
    }
}
