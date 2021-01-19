using CSharpFunctionalExtensions;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Core.SchoolAggregate.Schools;
using System;
using System.Threading.Tasks;

namespace SchoolManagement.Core.Interfaces
{
    public interface ISchoolRepository 
    {
        Task<Maybe<School>> GetByIdAsync(Guid id);
        Task<bool> ExistByIdAsync(Guid id);
        Task<Maybe<Member>> GetSchoolMemberByIdAsync(Guid schoolId, Guid memberId);
        void Add(School school);
    }
}
