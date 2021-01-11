using CSharpFunctionalExtensions;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Users;
using System;
using System.Threading.Tasks;

namespace SchoolManagement.Core.Interfaces
{
    public interface ISchoolRepository 
    {
        Task<Maybe<School>> GetByIdAsync(Guid id);
        Task<bool> ExistByIdAsync(Guid id);
        Task<Maybe<User>> GetSchoolMemberByIdAsync(Guid id, Guid memberId);
        void Add(School school);
    }
}
