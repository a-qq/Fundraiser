using Microsoft.EntityFrameworkCore.Storage;
using SharedKernel.Infrastructure.Abstractions.Common;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Common.Interfaces
{
    public interface ISchoolContext : IUnitOfWork
    {
        public IExecutionStrategy CreateExecutionStrategy();
        bool HasActiveTransaction { get; }
        public Task<IDbContextTransaction> BeginTransactionAsync();
        public Task CommitTransactionAsync(IDbContextTransaction transaction);
    }
}