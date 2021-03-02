using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.EnrollMember
{
    [Authorize(Policy = "MustBeAtLeastHeadmaster")]
    public sealed class EnrollMemberCommand : CommandRequest<MemberDTO>
    {
        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string Role { get; }
        public string Gender { get; }
        public Guid SchoolId { get; }

        public EnrollMemberCommand(string firstName, string lastName, string email, string role, string gender, Guid schoolId)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Role = role;
            Gender = gender;
            SchoolId = schoolId;
        }
    }

    internal sealed class EnrollMemberHandler : IRequestHandler<EnrollMemberCommand, Result<MemberDTO, RequestError>>
    {
        private readonly ISchoolContext _context;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IEmailUniquenessChecker _checker;
        private readonly IMapper _mapper;

        public EnrollMemberHandler(
            ISchoolContext schoolContext,
            ISchoolRepository schoolRepository,
            IEmailUniquenessChecker checker,
            IMapper mapper)
        {
            _context = schoolContext;
            _schoolRepository = schoolRepository;
            _checker = checker;
            _mapper = mapper;
        }

        public async Task<Result<MemberDTO, RequestError>> Handle(EnrollMemberCommand request, CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var firstName = FirstName.Create(request.FirstName).Value;
            var lastName = LastName.Create(request.LastName).Value;
            var email = Email.Create(request.Email).Value;
            var gender = Gender.Create(request.Gender).Value;
            var role = Role.Create(request.Role).Value;

            var schoolOrNone = await _schoolRepository.GetByIdAsync(schoolId, cancellationToken);
            if (schoolOrNone.HasNoValue)
                return Result.Failure<MemberDTO, RequestError>(SharedRequestError.General.NotFound(schoolId, nameof(School)));

            if (!await _checker.IsUnique(email))
                return Result.Failure<MemberDTO, RequestError>(SharedRequestError.User.EmailIsTaken(email));

            var memberOrError = schoolOrNone.Value.EnrollCandidate(firstName, lastName, email, role, gender);

            if (memberOrError.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(memberOrError.Error);

            await _context.SaveChangesAsync(cancellationToken);

            var memberDto = _mapper.Map<MemberDTO>(memberOrError.Value);

            return memberDto;
        }
    }
}
