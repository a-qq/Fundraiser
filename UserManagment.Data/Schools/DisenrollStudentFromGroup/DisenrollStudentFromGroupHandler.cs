using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using MediatR;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Data.Database;
using SchoolManagement.Data.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Schools.DisenrollStudentFromGroup
{
    public sealed class DisenrollStudentFromGroupHandler : IRequestHandler<DisenrollStudentFromGroupCommand, Result<bool, RequestError>>
    {
        private readonly IAuthorizationService _authService;
        private readonly ISchoolRepository _schoolRepository;
        private readonly SchoolContext _schoolContext;

        public DisenrollStudentFromGroupHandler(
            IAuthorizationService authorizationService,
            ISchoolRepository schoolRepository,
            SchoolContext schoolContext)
        {
            this._authService = authorizationService;
            this._schoolRepository = schoolRepository;
            this._schoolContext = schoolContext;
        }

        public async Task<Result<bool, RequestError>> Handle(DisenrollStudentFromGroupCommand request, CancellationToken cancellationToken)
        {
            await _authService.VerifyAuthorizationAsync(request.SchoolId, request.AuthId, Role.Headmaster);

            if (!await _schoolRepository.ExistByIdAsync(request.SchoolId))
                return Result.Failure<bool, RequestError>(SharedRequestError.General.NotFound(request.SchoolId, nameof(School)));

            Maybe<Group> groupOrNone = await _schoolRepository.GetGroupWithStudentsByIdAsync(request.SchoolId, request.GroupId);

            if (groupOrNone.HasNoValue)
                return Result.Failure<bool, RequestError>(SharedRequestError.General.NotFound(request.GroupId, nameof(Group)));

            Maybe<Member> studentOrNone = groupOrNone.Value.Students.TryFirst(s => s.Id == request.StudentId);
            if (studentOrNone.HasNoValue)
                return Result.Failure<bool, RequestError>(SharedRequestError.General.NotFound(request.StudentId, "Student"));

            groupOrNone.Value.School.DisenrollStudentFromGroup(groupOrNone.Value, studentOrNone.Value);

            await _schoolContext.SaveChangesAsync(cancellationToken);

            return Result.Success<bool, RequestError>(true);
        }
    }
}
