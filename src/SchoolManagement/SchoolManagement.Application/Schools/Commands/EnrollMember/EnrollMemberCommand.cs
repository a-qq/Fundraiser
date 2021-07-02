using Ardalis.GuardClauses;
using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Abstractions.Requests;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.EnrollMember
{
    [Authorize(Policy = PolicyNames.MustBeAtLeastHeadmaster)]
    public sealed class EnrollMemberCommand : IUserCommand<MemberDTO>, ISchoolAuthorizationRequest
    {
        public EnrollMemberCommand(string firstName, string lastName, string email, string role, string gender,
            Guid schoolId)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Role = role;
            Gender = gender;
            SchoolId = schoolId;
        }

        public string FirstName { get; }
        public string LastName { get; }
        public string Email { get; }
        public string Role { get; }
        public string Gender { get; }
        public Guid SchoolId { get; }
    }

    internal sealed class EnrollMemberCommandHandler : IRequestHandler<EnrollMemberCommand, Result<MemberDTO, RequestError>>
    {
        private readonly IEmailUniquenessChecker _checker;
        private readonly IMapper _mapper;
        private readonly ISchoolRepository _schoolRepository;

        public EnrollMemberCommandHandler(
            ISchoolRepository schoolRepository,
            IEmailUniquenessChecker emailUniquenessChecker,
            IMapper mapper)
        {
            _schoolRepository = Guard.Against.Null(schoolRepository, nameof(schoolRepository));
            _checker = Guard.Against.Null(emailUniquenessChecker, nameof(emailUniquenessChecker));
            _mapper = Guard.Against.Null(mapper, nameof(mapper));
        }

        public async Task<Result<MemberDTO, RequestError>> Handle(EnrollMemberCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);
            var firstName = FirstName.Create(request.FirstName).Value;
            var lastName = LastName.Create(request.LastName).Value;
            var email = Email.Create(request.Email).Value;
            var gender = Gender.Create(request.Gender).Value;
            var role = Role.Create(request.Role).Value;

            var schoolOrNone = await _schoolRepository.GetByIdAsync(schoolId, cancellationToken);
            if (schoolOrNone.HasNoValue)
                return Result.Failure<MemberDTO, RequestError>(
                    SharedRequestError.General.NotFound(schoolId, nameof(School)));

            if (!await _checker.IsUnique(email))
                return Result.Failure<MemberDTO, RequestError>(SharedRequestError.User.EmailIsTaken(email));

            var memberOrError = schoolOrNone.Value.EnrollCandidate(firstName, lastName, email, role, gender);

            if (memberOrError.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(memberOrError.Error);

            var memberDto = _mapper.Map<MemberDTO>(memberOrError.Value);

            return memberDto;
        }
    }
}