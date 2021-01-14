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
using static SchoolManagement.Core.SchoolAggregate.Users.User;

namespace SchoolManagement.Data.Schools.RegisterSchool
{
    public class RegisterSchoolHandler : IRequestHandler<RegisterSchoolCommand, Result<SchoolCreatedDTO, RequestError>>
    {
        private readonly SchoolContext _schoolContext;
        private readonly IEmailUniquenessChecker _checker;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IMapper _mapper;

        public RegisterSchoolHandler(
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

        public async Task<Result<SchoolCreatedDTO, RequestError>> Handle(RegisterSchoolCommand command, CancellationToken cancellationToken)
        {
            if (command.AuthId != Admin.Id)
                return Result.Failure<SchoolCreatedDTO, RequestError>(SharedErrors.General.Unauthorized(command.AuthId.ToString()));
            //fail fast
            Name schoolName = Name.Create(command.Name).Value;
            FirstName firstName = FirstName.Create(command.HeadmasterFirstName).Value;
            LastName lastName = LastName.Create(command.HeadmasterLastName).Value;
            Email email = Email.Create(command.HeadmasterEmail).Value;
            Gender gender = Gender.Create(command.HeadmasterGender).Value;

            if (!_checker.IsUnique(email))
                return Result.Failure<SchoolCreatedDTO, RequestError>(SharedErrors.User.EmailIsTaken(email.Value));

            var schoolOrError = Admin.RegisterSchool(schoolName, firstName, lastName, email, gender);

            if (schoolOrError.IsFailure)
                return Result.Failure<SchoolCreatedDTO, RequestError>(SharedErrors.General.BusinessRuleViolation(schoolOrError.Error));

            _schoolRepository.Add(schoolOrError.Value);

            await _schoolContext.SaveChangesAsync(cancellationToken);

            SchoolCreatedDTO schoolDto = _mapper.Map<SchoolCreatedDTO>(schoolOrError.Value);
            return Result.Success<SchoolCreatedDTO, RequestError>(schoolDto);
        }
    }
}
