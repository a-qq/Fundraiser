using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using MediatR;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Data.Database;
using SchoolManagement.Data.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Schools.MakeTeacherFormTutor
{
    public sealed class MakeTeacherFormTutorHandler : IRequestHandler<MakeTeacherFormTutorCommand, Result<bool, RequestError>>
    {
        private readonly IAuthorizationService _authService;
        private readonly ISchoolRepository _schoolRepository;
        private readonly SchoolContext _schoolContext;

        public MakeTeacherFormTutorHandler(
            IAuthorizationService authorizationService,
            ISchoolRepository schoolRepository,
            SchoolContext schoolContext)
        {
            _authService = authorizationService;
            _schoolRepository = schoolRepository;
            _schoolContext = schoolContext;
        }

        public async Task<Result<bool, RequestError>> Handle(MakeTeacherFormTutorCommand request, CancellationToken cancellationToken)
        {
            await _authService.VerifyAuthorizationAsync(request.SchoolId, request.AuthId, Role.Headmaster);

            Maybe<School> schoolOrNone = await _schoolRepository.GetSchoolWithGroupAndFormTutors(request.SchoolId);
            if (schoolOrNone.HasNoValue)
                return Result.Failure<bool, RequestError>(SharedErrors.General.NotFound(request.SchoolId, nameof(School)));

            Maybe<Group> groupOrNone = schoolOrNone.Value.Groups.TryFirst(g => g.Id == request.GroupId);
            if(groupOrNone.HasNoValue)
                return Result.Failure<bool, RequestError>(SharedErrors.General.NotFound(request.GroupId, nameof(Group)));

            Maybe<Member> teacherOrNone = await _schoolRepository.GetSchoolMemberByIdAsync(request.SchoolId, request.TeacherId);
            if (teacherOrNone.HasNoValue)
                return Result.Failure<bool, RequestError>(SharedErrors.General.NotFound(request.AuthId, nameof(Member)));

            Result result = schoolOrNone.Value.MakeTeacherFormTutor(teacherOrNone.Value, groupOrNone.Value);
            if (result.IsFailure)
                return Result.Failure<bool, RequestError>(SharedErrors.General.BusinessRuleViolation(result.Error));

            await _schoolContext.SaveChangesAsync(cancellationToken);

            return Result.Success<bool, RequestError>(true);
        }
    }
}
