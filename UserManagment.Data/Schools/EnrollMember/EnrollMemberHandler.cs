using AutoMapper;
using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.ResultErrors;
using Fundraiser.SharedKernel.Utils;
using MediatR;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Core.SchoolAggregate.Users;
using SchoolManagement.Data.Database;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Schools.EnrollMember
{
    public class EnrollMemberHandler : IRequestHandler<EnrollMemberCommand, Result<UserDTO, RequestError>>
    {
        private readonly SchoolContext _schoolContext;
        private readonly IEmailUniquenessChecker _checker;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IMapper _mapper;

        public EnrollMemberHandler(
            SchoolContext schoolContext,
            IEmailUniquenessChecker checker,
            ISchoolRepository schoolRepository,
            IMapper mapper)
        {
            _schoolContext = schoolContext;
            _checker = checker;
            _schoolRepository = schoolRepository;
            _mapper = mapper;
        }

        public async Task<Result<UserDTO, RequestError>> Handle(EnrollMemberCommand command, CancellationToken cancellationToken)
        {
            Maybe<School> school = await _schoolRepository.GetByIdAsync(command.SchoolId);
            User currentUser;

            if (command.AuthId == User.Admin.Id)
            {
                if (school.HasNoValue)
                    return Result.Failure<UserDTO, RequestError>(SharedErrors.General.NotFound(nameof(School), command.SchoolId.ToString()));
                currentUser = User.Admin;
            }
            else {
                if (school.HasNoValue)
                    return Result.Failure<UserDTO, RequestError>(SharedErrors.General.Unauthorized(command.AuthId.ToString()));

                Maybe<User> currentUserOrNone = await _schoolRepository.GetSchoolMemberByIdAsync(school.Value.Id, command.AuthId);
                if (currentUserOrNone.HasNoValue)
                    return Result.Failure<UserDTO, RequestError>(SharedErrors.General.Unauthorized(command.AuthId.ToString()));

                currentUser = currentUserOrNone.Value;
            }



            FirstName firstName = FirstName.Create(command.FirstName).Value;
            LastName lastName = LastName.Create(command.LastName).Value;
            Email email = Email.Create(command.Email).Value;
            Gender gender = Gender.Create(command.Gender).Value;
            Role role = Role.Create(command.Role).Value;

            if(!_checker.IsUnique(email))
                return Result.Failure<UserDTO, RequestError>(SharedErrors.User.EmailIsTaken(email.Value));

            var memberOrError = currentUser.Role == Role.Administrator 
                ? currentUser.CreateMemberAndEnrollToSchool(firstName, lastName, email, role, gender, school.Value)
                : currentUser.CreateMemberAndEnrollToSchool(firstName, lastName, email, role, gender);

            if (memberOrError.IsFailure)
                return memberOrError.ConvertFailure<UserDTO>();

            await _schoolContext.SaveChangesAsync(cancellationToken);

            UserDTO userDTO = _mapper.Map<UserDTO>(memberOrError.Value);
            return Result.Success<UserDTO, RequestError>(userDTO);
        }
    }
}
