using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using MediatR;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Data.Database;
using SchoolManagement.Data.Services;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Schools.ExpellMember
{
    internal sealed class ExpellMemberHandler : IRequestHandler<ExpellMemberCommand, Result<bool, RequestError>>
    {
        private readonly IAuthorizationService _authService;
        private readonly ISchoolRepository _schoolRepository;
        private readonly SchoolContext _schoolContext;

        public ExpellMemberHandler(
            IAuthorizationService authorizationService,
            ISchoolRepository schoolRepository,
            SchoolContext schoolContext)
        {
            this._authService = authorizationService;
            this._schoolRepository = schoolRepository;
            this._schoolContext = schoolContext;
        }
        public async Task<Result<bool, RequestError>> Handle(ExpellMemberCommand request, CancellationToken cancellationToken)
        {
            await _authService.VerifyAuthorizationAsync(request.SchoolId, request.AuthId, Role.Headmaster);

            //retrieved form cache 
            Maybe<School> schoolOrNone = await _schoolRepository.GetByIdAsync(request.SchoolId);
            if (schoolOrNone.HasNoValue)
                return Result.Failure<bool, RequestError>(SharedRequestError.General.NotFound(request.SchoolId, nameof(School)));

            //in order not to load full collection of members into memeory to delete one student
            Maybe<Member> memberOrNone = await _schoolRepository.GetSchoolMemberByIdAsync(request.SchoolId, request.MemberId);
            if (memberOrNone.HasNoValue)
                return Result.Failure<bool, RequestError>(SharedRequestError.General.NotFound(request.MemberId, nameof(Member)));

            memberOrNone.Value.School.ExpellMember(memberOrNone.Value);

            await _schoolContext.SaveChangesAsync(cancellationToken);

            return Result.Success<bool, RequestError>(true);
        }
    }
}
