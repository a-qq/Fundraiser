﻿using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Utils;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.RegisterSchool
{
    [Authorize(Policy = PolicyNames.MustBeAdmin)]
    public sealed class RegisterSchoolCommand : IUserCommand<SchoolCreatedDTO>
    {
        public RegisterSchoolCommand(string name, int yearsOfEducation, string firstName, string lastName, string email,
            string gender)
        {
            Name = name;
            YearsOfEducation = yearsOfEducation;
            HeadmasterFirstName = firstName;
            HeadmasterLastName = lastName;
            HeadmasterEmail = email;
            HeadmasterGender = gender;
        }

        public string Name { get; }
        public int YearsOfEducation { get; }
        public string HeadmasterFirstName { get; }
        public string HeadmasterLastName { get; }
        public string HeadmasterEmail { get; }
        public string HeadmasterGender { get; }
    }

    internal sealed class RegisterSchoolCommandHandler : IRequestHandler<RegisterSchoolCommand, Result<SchoolCreatedDTO, RequestError>>
    {
        private readonly IEmailUniquenessChecker _checker;
        private readonly IMapper _mapper;
        private readonly ISchoolRepository _schoolRepository;

        public RegisterSchoolCommandHandler(
            IEmailUniquenessChecker checker,
            ISchoolRepository schoolRepository,
            IMapper mapper)
        {
            _checker = checker;
            _schoolRepository = schoolRepository;
            _mapper = mapper;
        }

        public async Task<Result<SchoolCreatedDTO, RequestError>> Handle(RegisterSchoolCommand command,
            CancellationToken cancellationToken)
        {
            var schoolName = Name.Create(command.Name).Value;
            var years = YearsOfEducation.Create(command.YearsOfEducation).Value;
            var firstName = FirstName.Create(command.HeadmasterFirstName).Value;
            var lastName = LastName.Create(command.HeadmasterLastName).Value;
            var email = Email.Create(command.HeadmasterEmail).Value;
            var gender = Gender.Create(command.HeadmasterGender).Value;

            if (!await _checker.IsUnique(email))
                return Result.Failure<SchoolCreatedDTO, RequestError>(SharedRequestError.User.EmailIsTaken(email));

            var school = new School(schoolName, years, firstName, lastName, email, gender);

            _schoolRepository.Add(school);

            var schoolDto = _mapper.Map<SchoolCreatedDTO>(school);
            return Result.Success<SchoolCreatedDTO, RequestError>(schoolDto);
        }
    }
}