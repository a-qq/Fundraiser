using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using Fundraiser.SharedKernel.Utils;
using MediatR;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Data.Database;
using SchoolManagement.Data.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Schools.AddStudentsToGroup
{
    public class AddStudentsToGroupHandler : IRequestHandler<AddStudentsToGroupCommand, Result<bool, RequestError>>
    {
        private readonly IAuthorizationService _authService;
        private readonly ISchoolRepository _schoolRepository;
        private readonly SchoolContext _schoolContext;

        public AddStudentsToGroupHandler(
            IAuthorizationService authorizationService,
            ISchoolRepository schoolRepository,
            SchoolContext schoolContext)
        {
            _authService = authorizationService;
            _schoolRepository = schoolRepository;
            _schoolContext = schoolContext;
        }

        public async Task<Result<bool, RequestError>> Handle(AddStudentsToGroupCommand request, CancellationToken cancellationToken)
        {
            await _authService.VerifyAuthorizationAsync(request.SchoolId, request.AuthId, Role.Headmaster);

            if (!await _schoolRepository.ExistByIdAsync(request.SchoolId))
                return Result.Failure<bool, RequestError>(SharedRequestError.General.NotFound(request.SchoolId, nameof(School)));

            Maybe<Group> groupOrNone = await _schoolRepository.GetGroupWithStudentsByIdAsync(request.SchoolId, request.GroupId);

            if (groupOrNone.HasNoValue)
                return Result.Failure<bool, RequestError>(SharedRequestError.General.NotFound(request.GroupId, nameof(Group)));

            List<Member> membersToAdd = await _schoolRepository.GetSchoolMembersByIdAsync(request.SchoolId, request.StudentIds);
            if (membersToAdd.Count != request.StudentIds.Count)
            {
                var missingMembersIds = request.StudentIds.Except(membersToAdd.Select(m => m.Id));
                return Result.Failure<bool, RequestError>(SharedRequestError.General.NotFound(missingMembersIds, nameof(Member)));
            }

            Result<bool, Error> result = groupOrNone.Value.School.AssignMembersToGroup(groupOrNone.Value, membersToAdd);
            if (result.IsFailure)
                return Result.Failure<bool, RequestError>(SharedRequestError.General.BusinessRuleViolation(result.Error));

            await _schoolContext.SaveChangesAsync(cancellationToken);

            return Result.Success<bool, RequestError>(true);
        }
    }
}
