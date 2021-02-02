using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using MediatR;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Data.Database;
using SchoolManagement.Data.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Schools.DivestTreasurer
{
    internal sealed class DivestTreasurerHandler : IRequestHandler<DivestTreasurerCommand, Result<bool, RequestError>>
    {
        private readonly IAuthorizationService _authService;
        private readonly ISchoolRepository _schoolRepository;
        private readonly SchoolContext _schoolContext;

        public DivestTreasurerHandler(
            IAuthorizationService authorizationService,
            ISchoolRepository schoolRepository,
            SchoolContext schoolContext)
        {
            _authService = authorizationService;
            _schoolRepository = schoolRepository;
            _schoolContext = schoolContext;
        }

        public async Task<Result<bool, RequestError>> Handle(DivestTreasurerCommand request, CancellationToken cancellationToken)
        {
            await _authService.VerifyFormTutorAuthorizationAsync(request.SchoolId, request.AuthId, request.GroupId);

            if (!await _schoolRepository.ExistByIdAsync(request.SchoolId))
                return Result.Failure<bool, RequestError>(SharedRequestError.General.NotFound(request.SchoolId, nameof(School)));

            Maybe<Group> groupOrNone = await _schoolRepository.GetGroupWithTreasurerByIdAsync(request.SchoolId, request.GroupId);
            if (groupOrNone.HasNoValue)
                return Result.Failure<bool, RequestError>(SharedRequestError.General.NotFound(request.GroupId, nameof(Group)));

            Result result = groupOrNone.Value.School.DivestTreasurer(groupOrNone.Value);
            if (result.IsFailure)
                return Result.Failure<bool, RequestError>(SharedRequestError.General.BusinessRuleViolation(result.Error));

            await _schoolContext.SaveChangesAsync(cancellationToken);

            return Result.Success<bool, RequestError>(true);
        }
    }
}
