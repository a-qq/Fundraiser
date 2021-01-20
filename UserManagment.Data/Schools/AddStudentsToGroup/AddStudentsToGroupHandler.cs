using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using MediatR;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Data.Database;
using SchoolManagement.Data.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fundraiser.SharedKernel.Utils;

namespace SchoolManagement.Data.Schools.AddStudentsToGroup
{
    public class AddStudentsToGroupHandler : IRequestHandler<AddStudentsToGroupCommand, Result<bool, RequestError>>
    {
        private readonly IAuthorizationService _authService;
        private readonly ISchoolRepository _schoolReposioty;
        private readonly SchoolContext _schoolContext;

        public AddStudentsToGroupHandler(
            IAuthorizationService authorizationService,
            ISchoolRepository schoolReposioty,
            SchoolContext schoolContext)
        {
            _authService = authorizationService;
            _schoolReposioty = schoolReposioty;
            _schoolContext = schoolContext;
        }

        public async Task<Result<bool, RequestError>> Handle(AddStudentsToGroupCommand request, CancellationToken cancellationToken)
        {
            Result<School, RequestError> schoolOrError =
                await _authService.VerifyAuthorizationAsync(request.SchoolId, request.AuthId, Role.Headmaster);

            if (schoolOrError.IsFailure)
                return schoolOrError.ConvertFailure<bool>();

            Maybe<Group> groupOrNone = await _schoolReposioty.GetGroupByIdAsync(request.SchoolId, request.GroupId);

            if (groupOrNone.HasNoValue)
                return Result.Failure<bool, RequestError>(SharedErrors.General.NotFound(request.GroupId, nameof(Group)));

            List<Member> membersToAdd = await _schoolReposioty.GetSchoolMembersByIdAsync(request.SchoolId, request.StudentIds);
            if (membersToAdd.Count != request.StudentIds.Count)
            {
                var missingMembersIds = request.StudentIds.Except(membersToAdd.Select(m => m.Id));
                return Result.Failure<bool, RequestError>(SharedErrors.General.NotFound(missingMembersIds, nameof(Member)));
            }

            Result<bool, Error> result = schoolOrError.Value.AssignMembersToGroup(groupOrNone.Value, membersToAdd);
            if (result.IsFailure)
                return Result.Failure<bool, RequestError>(SharedErrors.General.BusinessRuleViolation(result.Error));

            await _schoolContext.SaveChangesAsync(cancellationToken);

            return Result.Success<bool, RequestError>(true);
        }
    }
}
