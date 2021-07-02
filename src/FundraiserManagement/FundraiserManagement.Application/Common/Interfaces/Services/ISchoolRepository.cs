using CSharpFunctionalExtensions;
using FundraiserManagement.Domain.Common.Models;
using FundraiserManagement.Domain.SchoolAggregate;
using System.Threading;
using System.Threading.Tasks;

namespace FundraiserManagement.Application.Common.Interfaces.Services
{
    public interface ISchoolRepository
    {
        Task<bool> ExistByIdAsync(SchoolId schoolId, CancellationToken token);
        Task<Maybe<School>> GetByIdAsync(SchoolId schoolId, CancellationToken token);
        void Add(School school);
    }
}