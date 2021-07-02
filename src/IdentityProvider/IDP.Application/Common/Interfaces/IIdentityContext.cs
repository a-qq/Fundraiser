using Microsoft.EntityFrameworkCore.Storage;
using SharedKernel.Infrastructure.Abstractions.Common;
using System.Threading.Tasks;

namespace IDP.Application.Common.Interfaces
{
    public interface IIdentityContext : IUnitOfWork
    {
        public IExecutionStrategy CreateExecutionStrategy();
        bool HasActiveTransaction { get; }
        public Task<IDbContextTransaction> BeginTransactionAsync();
        public Task CommitTransactionAsync(IDbContextTransaction transaction);
    }
}