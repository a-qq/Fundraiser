using SchoolManagement.Core.SchoolAggregate.Schools;
using SixLabors.ImageSharp;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Services
{
    public interface IStorageService
    {
        Task SaveAsync(Image image, School school, CancellationToken cancellationToken = default);
        Task DeleteAsync(string fileName);
    }
}
