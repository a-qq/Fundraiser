using SchoolManagement.Domain.SchoolAggregate.Schools;
using SixLabors.ImageSharp;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Common.Interfaces
{
    public interface ILogoStorageService
    {
        Task UpsertLogoAsync(Image image, School school, CancellationToken cancellationToken = default);
        Task DeleteLogoAsync(School school, CancellationToken cancellationToken = default);
    }
}
