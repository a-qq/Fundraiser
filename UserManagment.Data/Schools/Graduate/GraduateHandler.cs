using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using Fundraiser.SharedKernel.Utils;
using MediatR;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Data.Database;
using SchoolManagement.Data.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Schools.Graduate
{
    internal sealed class GraduateHandler : IRequestHandler<GraduateCommand, Result<bool, RequestError>>
    {
        private readonly IAuthorizationService _authService;
        private readonly ISchoolRepository _schoolRepository;
        private readonly SchoolContext _schoolContext;

        public GraduateHandler(
            IAuthorizationService authorizationService,
            ISchoolRepository schoolRepository,
            SchoolContext schoolContext)
        {
            _authService = authorizationService;
            _schoolRepository = schoolRepository;
            _schoolContext = schoolContext;
        }

        public async Task<Result<bool, RequestError>> Handle(GraduateCommand request, CancellationToken cancellationToken)
        {
            await _authService.VerifyAuthorizationAsync(request.SchoolId, request.AuthId, Role.Headmaster);

            Maybe<School> schoolOrNone = await _schoolRepository.GetSchoolWithGroupsWithStudentsAndFormTutorsByIdAsync(request.SchoolId);
            if (schoolOrNone.HasNoValue)
                return Result.Failure<bool, RequestError>(SharedRequestError.General.NotFound(request.SchoolId, nameof(School)));

            Result<bool, Error> result = schoolOrNone.Value.Graduate();
            if(result.IsFailure)
                return Result.Failure<bool, RequestError>(SharedRequestError.General.BusinessRuleViolation(result.Error));

            await _schoolContext.SaveChangesAsync(cancellationToken);

            return Result.Success<bool, RequestError>(true);
        }
    }
}
