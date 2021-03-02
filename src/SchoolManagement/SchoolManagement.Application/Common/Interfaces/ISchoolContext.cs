using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Common.Interfaces
{
    public interface ISchoolContext
    {
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}