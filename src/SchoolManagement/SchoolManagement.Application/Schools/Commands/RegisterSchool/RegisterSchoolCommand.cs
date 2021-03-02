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
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.RegisterSchool
{
    [Authorize(Policy = "MustBeAdmin")]
    public sealed class RegisterSchoolCommand : CommandRequest<SchoolCreatedDTO>
    {
        public string Name { get; }
        public int YearsOfEducation { get; }
        public string HeadmasterFirstName { get; }
        public string HeadmasterLastName { get; }
        public string HeadmasterEmail { get; }
        public string HeadmasterGender { get; }

        public RegisterSchoolCommand(string name, int yearsOfEducation, string firstName, string lastName, string email, string gender)
        {
            Name = name;
            YearsOfEducation = yearsOfEducation;
            HeadmasterFirstName = firstName;
            HeadmasterLastName = lastName;
            HeadmasterEmail = email;
            HeadmasterGender = gender;
        }
    }

    internal sealed class RegisterSchoolHandler : IRequestHandler<RegisterSchoolCommand, Result<SchoolCreatedDTO, RequestError>>
    {
        private readonly ISchoolContext _context;
        private readonly IEmailUniquenessChecker _checker;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IMapper _mapper;

        public RegisterSchoolHandler(
            ISchoolContext schoolContext,
            IEmailUniquenessChecker checker,
            ISchoolRepository schoolRepository,
            IMapper mapper)
        {
            _context = schoolContext;
            _checker = checker;
            _schoolRepository = schoolRepository;
            _mapper = mapper;
        }

        public async Task<Result<SchoolCreatedDTO, RequestError>> Handle(RegisterSchoolCommand command, CancellationToken cancellationToken)
        {
            //fail fast
            Name schoolName = Name.Create(command.Name).Value;
            YearsOfEducation years = YearsOfEducation.Create(command.YearsOfEducation).Value;
            FirstName firstName = FirstName.Create(command.HeadmasterFirstName).Value;
            LastName lastName = LastName.Create(command.HeadmasterLastName).Value;
            Email email = Email.Create(command.HeadmasterEmail).Value;
            Gender gender = Gender.Create(command.HeadmasterGender).Value;

            if (!await _checker.IsUnique(email))
                return Result.Failure<SchoolCreatedDTO, RequestError>(SharedRequestError.User.EmailIsTaken(email));

            School school = new School(schoolName, years, firstName, lastName, email, gender);

            _schoolRepository.Add(school);

            await _context.SaveChangesAsync(cancellationToken);

            SchoolCreatedDTO schoolDto = _mapper.Map<SchoolCreatedDTO>(school);
            return Result.Success<SchoolCreatedDTO, RequestError>(schoolDto);
        }
    }
}
