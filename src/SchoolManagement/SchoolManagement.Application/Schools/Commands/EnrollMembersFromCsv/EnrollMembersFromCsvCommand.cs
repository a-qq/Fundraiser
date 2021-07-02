using Ardalis.GuardClauses;
using AutoMapper;
using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Microsoft.AspNetCore.Http;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Mappings.CsvHelper;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.Common.Models;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Domain.Errors;
using SharedKernel.Domain.ValueObjects;
using SharedKernel.Infrastructure.Abstractions.Requests;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Application.Schools.Commands.EnrollMembersFromCsv
{
    [Authorize(Policy = PolicyNames.MustBeAtLeastHeadmaster)]
    public sealed class EnrollMembersFromCsvCommand : IUserCommand<IEnumerable<MemberDTO>>, ISchoolAuthorizationRequest
    {
        public EnrollMembersFromCsvCommand(IFormFile csvFile, DelimiterEnum delimiter, Guid schoolId)
        {
            File = csvFile;
            Delimiter = delimiter;
            SchoolId = schoolId;
        }

        public IFormFile File { get; }
        public DelimiterEnum Delimiter { get; }
        public Guid SchoolId { get; }
    }

    internal sealed class EnrollMembersFromCsvCommandHandler : IRequestHandler<EnrollMembersFromCsvCommand,
        Result<IEnumerable<MemberDTO>, RequestError>>
    {
        private readonly IEmailUniquenessChecker _checker;
        private readonly IMapper _mapper;
        private readonly ISchoolRepository _schoolRepository;

        public EnrollMembersFromCsvCommandHandler(
            ISchoolRepository schoolRepository,
            IEmailUniquenessChecker emailUniquenessChecker,
            IMapper mapper)
        {
            _schoolRepository = Guard.Against.Null(schoolRepository, nameof(schoolRepository));
            _checker = Guard.Against.Null(emailUniquenessChecker, nameof(emailUniquenessChecker));
            _mapper = Guard.Against.Null(mapper, nameof(mapper));
        }

        public async Task<Result<IEnumerable<MemberDTO>, RequestError>> Handle(EnrollMembersFromCsvCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);

            var schoolOrNone = await _schoolRepository.GetByIdWithMembersAsync(schoolId, cancellationToken);
            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ((char)request.Delimiter).ToString(),
                PrepareHeaderForMatch = (header, index) => header.ToLower()
            };

            using var reader = new StreamReader(request.File.OpenReadStream());
            using var csv = new CsvReader(reader, config);

            csv.Context.RegisterClassMap<MemberEnrollmentAssignmentDataMap>();

            var candidates = csv.GetRecords<MemberEnrollmentAssignmentData>();
            var emails = new HashSet<Email>();
            var notFoundGroupCodes = new HashSet<Code>();
            var enrolledMembers = new List<Member>();
            var fullGroups = new Dictionary<Code, int>();

            var enrollmentResult = Result.Success<bool, Error>(true);
            var studentRole = Role.Student;
            foreach (var candidate in candidates)
            {
                if (emails.Contains(candidate.Email))
                    throw new ApplicationException($"Duplicate email '{candidate.Email}' in input file!");

                emails.Add(candidate.Email);

                if (candidate.GroupCode.HasValue)
                {
                    if (candidate.Role != studentRole)
                        throw new ApplicationException($"Attempted to assign candidate with role '{candidate.Role}' to a group!");

                    if (!notFoundGroupCodes.Contains(candidate.GroupCode.Value) && !schoolOrNone.Value.Groups.Any(g => g.Code == candidate.GroupCode.Value && !g.IsArchived))
                        notFoundGroupCodes.Add(candidate.GroupCode.Value);
                }

                if (notFoundGroupCodes.Any())
                    continue;

                if (enrollmentResult.IsFailure)
                    continue;

                var enrollment = schoolOrNone.Value.EnrollCandidate(candidate.FirstName, candidate.LastName, candidate.Email,
                    candidate.Role, candidate.Gender);

                if (enrollment.IsFailure)
                {
                    enrollmentResult = enrollment.ConvertFailure<bool>();
                    continue;
                }

                enrolledMembers.Add(enrollment.Value);

                if (candidate.GroupCode.HasNoValue) 
                    continue;

                if (fullGroups.ContainsKey(candidate.GroupCode.Value))
                {
                    fullGroups[candidate.GroupCode.Value] += 1;
                    continue;
                }

                var result = schoolOrNone.Value.AssignStudentToGroup(enrollment.Value.Id, candidate.GroupCode.Value);

                if (result.IsFailure)
                    fullGroups.Add(candidate.GroupCode.Value, 1);
            }

            if (notFoundGroupCodes.Any())
                return SharedRequestError.General.NotFound(
                    notFoundGroupCodes.Select(x => x.Value), nameof(Group), nameof(Group.Code));

            var (areUnique, duplicateEmails) = await _checker.AreUnique(emails);

            if (!areUnique)
                return SharedRequestError.User.EmailsAreTaken(duplicateEmails);

            if (fullGroups.Any())
            {
                ICombine error = new Error(string.Empty); 
                foreach (var group in fullGroups)
                    error = error.Combine(new Error($"Member limit for group '{group.Key}' exceeded by {group.Value}!"));

                return SharedRequestError.General.BusinessRuleViolation(error as Error);
            }

            var memberDtos = _mapper.Map<IEnumerable<MemberDTO>>(enrolledMembers);

            return Result.Success<IEnumerable<MemberDTO>, RequestError>(memberDtos);
        }
    }
}