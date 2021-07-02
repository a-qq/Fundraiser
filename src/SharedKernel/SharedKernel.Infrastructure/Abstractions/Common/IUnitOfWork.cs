using Microsoft.EntityFrameworkCore.Storage;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Infrastructure.Abstractions.Common
{
    public interface IUnitOfWork
    {
        DbConnection GetDbConnection();
        IDbContextTransaction GetCurrentTransaction();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}