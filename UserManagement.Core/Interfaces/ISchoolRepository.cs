using CSharpFunctionalExtensions;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Members;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SchoolManagement.Core.Interfaces
{
    public interface ISchoolRepository
    {
        Task<Maybe<School>> GetByIdAsync(Guid id);
        Task<bool> ExistByIdAsync(Guid id);
        Task<Maybe<Member>> GetSchoolMemberByIdAsync(Guid schoolId, Guid memberId);
        Task<Maybe<Group>> GetGroupWithStudentsByIdAsync(Guid schoolId, long groupId);
        Task<Maybe<Group>> GetGroupWithFormTutorByIdAsync(Guid schoolId, long groupId);
        Task<List<Member>> GetSchoolMembersByIdAsync(Guid schoolId, IEnumerable<Guid> userIds);
        Task<Maybe<School>> GetSchoolWithGroupAndFormTutors(Guid schoolId);
        void Add(School school);
        void Remove(School school);
    }
}
