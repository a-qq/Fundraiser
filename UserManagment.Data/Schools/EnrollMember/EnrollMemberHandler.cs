using AutoMapper;
using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using Fundraiser.SharedKernel.Utils;
using MediatR;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Users;
using SchoolManagement.Data.Database;
using SchoolManagement.Data.Services;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Schools.EnrollMember
{
    public class EnrollMemberHandler : IRequestHandler<EnrollMemberCommand, Result<UserDTO, RequestError>>
    {
        private readonly SchoolContext _schoolContext;
        private readonly IEmailUniquenessChecker _checker;
        private readonly IAuthorizationService _authorizationService;
        private readonly IMapper _mapper;

        public EnrollMemberHandler(
            SchoolContext schoolContext,
            IEmailUniquenessChecker checker,
            IAuthorizationService authorizationService,
            IMapper mapper)
        {
            _schoolContext = schoolContext;
            _checker = checker;
            _authorizationService = authorizationService;
            _mapper = mapper;
        }

        public async Task<Result<UserDTO, RequestError>> Handle(EnrollMemberCommand command, CancellationToken cancellationToken)
        {
            Result<Tuple<School, User>, RequestError> result = await _authorizationService.GetAuthorizationContextAsync(command.SchoolId, command.AuthId);
            if (result.IsFailure)
                result.ConvertFailure<UserDTO>();

            School school = result.Value.Item1;
            User currentUser = result.Value.Item2;
            FirstName firstName = FirstName.Create(command.FirstName).Value;
            LastName lastName = LastName.Create(command.LastName).Value;
            Email email = Email.Create(command.Email).Value;
            Gender gender = Gender.Create(command.Gender).Value;
            Role role = Role.Create(command.Role).Value;

            if(!_checker.IsUnique(email))
                return Result.Failure<UserDTO, RequestError>(SharedErrors.User.EmailIsTaken(email.Value));

            Result<User> memberOrError = currentUser.EnrollToSchool(firstName, lastName, email, role, gender, school);

            if (memberOrError.IsFailure)
                return Result.Failure<UserDTO, RequestError>(SharedErrors.General.BusinessRuleViolation(memberOrError.Error));

            await _schoolContext.SaveChangesAsync(cancellationToken);

            UserDTO userDTO = _mapper.Map<UserDTO>(memberOrError.Value);
            return Result.Success<UserDTO, RequestError>(userDTO);
        }
    }
}
