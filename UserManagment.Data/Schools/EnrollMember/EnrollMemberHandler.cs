using AutoMapper;
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

namespace SchoolManagement.Data.Schools.EnrollMember
{
    public class EnrollMemberHandler : IRequestHandler<EnrollMemberCommand, Result<MemberDTO, RequestError>>
    {
        private readonly SchoolContext _schoolContext;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IEmailUniquenessChecker _checker;
        private readonly IAuthorizationService _authService;
        private readonly IMapper _mapper;

        public EnrollMemberHandler(
            SchoolContext schoolContext,
            ISchoolRepository schoolRepository,
            IEmailUniquenessChecker checker,
            IAuthorizationService authorizationService,
            IMapper mapper)
        {
            _schoolContext = schoolContext;
            _schoolRepository = schoolRepository;
            _checker = checker;
            _authService = authorizationService;
            _mapper = mapper;
        }

        public async Task<Result<MemberDTO, RequestError>> Handle(EnrollMemberCommand command, CancellationToken cancellationToken)
        {
            await _authService.VerifyAuthorizationAsync(command.SchoolId, command.AuthId, Role.Headmaster);

            Maybe<School> schoolOrNone = await _schoolRepository.GetByIdAsync(command.SchoolId);
            if (schoolOrNone.HasNoValue)
                return Result.Failure<MemberDTO, RequestError>(SharedRequestError.General.NotFound(command.SchoolId, nameof(School)));

            FirstName firstName = FirstName.Create(command.FirstName).Value;
            LastName lastName = LastName.Create(command.LastName).Value;
            Email email = Email.Create(command.Email).Value;
            Gender gender = Gender.Create(command.Gender).Value;
            Role role = Role.Create(command.Role).Value;

            if(!await _checker.IsUnique(email))
                return Result.Failure<MemberDTO, RequestError>(SharedRequestError.User.EmailIsTaken(email));

            Result<Member> memberOrError = schoolOrNone.Value.EnrollCandidate(firstName, lastName, email, role, gender);

            if (memberOrError.IsFailure)
                return Result.Failure<MemberDTO, RequestError>(SharedRequestError.General.BusinessRuleViolation(memberOrError.Error));

            await _schoolContext.SaveChangesAsync(cancellationToken);

            MemberDTO userDTO = _mapper.Map<MemberDTO>(memberOrError.Value);
            return Result.Success<MemberDTO, RequestError>(userDTO);
        }
    }
}
