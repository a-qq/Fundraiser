using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;
using SharedKernel.Infrastructure.Abstractions.Common;

namespace FundraiserManagement.Application.Common.Interfaces.Services
{
    public interface IFundraiserContext : IUnitOfWork
    {
        public IExecutionStrategy CreateExecutionStrategy();
        bool HasActiveTransaction { get; }
        public Task<IDbContextTransaction> BeginTransactionAsync();
        public Task CommitTransactionAsync(IDbContextTransaction transaction);
    }
}
