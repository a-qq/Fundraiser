using System.Threading;
using System.Threading.Tasks;

namespace IDP.Application.Common.Interfaces
{
    public interface IIdentityContext
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}