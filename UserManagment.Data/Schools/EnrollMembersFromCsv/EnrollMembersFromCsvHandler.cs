using AutoMapper;
using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using Fundraiser.SharedKernel.RequestErrors;
using Fundraiser.SharedKernel.Utils;
using MediatR;
using SchoolManagement.Core.Interfaces;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Members;
using SchoolManagement.Core.SchoolAggregate.Schools;
using SchoolManagement.Data.Database;
using SchoolManagement.Data.ResultErrors;
using SchoolManagement.Data.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagement.Data.Schools.EnrollMembersFromCsv
{
    public sealed class EnrollMembersFromCsvHandler : IRequestHandler<EnrollMembersFromCsvCommand, Result<IEnumerable<MemberCreatedDTO>, RequestError>>
    {
        private readonly SchoolContext _schoolContext;
        private readonly ISchoolRepository _schoolRepository;
        private readonly IEmailUniquenessChecker _checker;
        private readonly IAuthorizationService _authService;
        private readonly IMapper _mapper;

        public EnrollMembersFromCsvHandler(
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

        public async Task<Result<IEnumerable<MemberCreatedDTO>, RequestError>> Handle(EnrollMembersFromCsvCommand request, CancellationToken cancellationToken)
        {
            await _authService.VerifyAuthorizationAsync(request.SchoolId, request.AuthId, Role.Headmaster);

            Maybe<School> schoolOrNone = await _schoolRepository.GetByIdAsync(request.SchoolId);
            if (schoolOrNone.HasNoValue)
                return Result.Failure<IEnumerable<MemberCreatedDTO>, RequestError>(SharedRequestError.General.NotFound(request.SchoolId, nameof(School)));

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ";",
                PrepareHeaderForMatch = (string header, int index) => header.ToLower()
            };

            ILookup<bool, MemberFromCsvModel> candidates = null;
            using (var reader = new StreamReader(request.CsvFile.OpenReadStream()))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<MemberFromCsvMap>();
                candidates = csv.GetRecords<MemberFromCsvModel>().ToLookup(c => c.GroupNumber.HasValue && c.GroupSign.HasValue);
            }

            //check if works
            var emails = candidates.SelectMany(c => c).Select(c => c.Email).AsEnumerable();

            var duplicates = emails.GroupBy(e => e).SelectMany(g => g.Skip(1)).Distinct();
            if (duplicates.Any())
                return Result.Failure<IEnumerable<MemberCreatedDTO>, RequestError>(ManagementRequestError.Csv.DuplicateEmails(duplicates));

            //since duplicate check have passed, there are no duplicates in input file
            Tuple<bool, IEnumerable<Email>> result = await _checker.AreUnique(emails);
            if (!result.Item1)
                return Result.Failure<IEnumerable<MemberCreatedDTO>, RequestError>(SharedRequestError.User.EmailsAreTaken(result.Item2));

            Result<bool, Error> candidatesEnrollment = Result.Success<bool, Error>(true);
            List<Member> members = new List<Member>();

            if (candidates.Contains(true))
            {
                var studentsGroups = candidates[true]
                    .GroupBy(c => c.GroupNumber.Value + c.GroupSign.Value, StringComparer.OrdinalIgnoreCase);

                var notFoundGroupCodes = studentsGroups.Select(x => x.Key)
                    .Except(schoolOrNone.Value.Groups.Select(g => g.Code));

                if (notFoundGroupCodes.Count() > 0)
                    return Result.Failure<IEnumerable<MemberCreatedDTO>, RequestError>(SharedRequestError.General.NotFound(notFoundGroupCodes, nameof(Group), nameof(Group.Code)));

                foreach (var studentGroup in studentsGroups)
                {
                    var enrollment = EnrollCandidates(studentGroup.AsEnumerable(), schoolOrNone.Value, out var newMembers);
                    var assigment = Result.Success<bool, Error>(true);
                    if (enrollment.IsSuccess)
                    {
                        var group = schoolOrNone.Value.Groups.FirstOrDefault(
                            g => string.Equals(studentGroup.Key, g.Code, StringComparison.OrdinalIgnoreCase));
                        assigment = schoolOrNone.Value.AssignMembersToGroup(group, newMembers);
                        members.AddRange(newMembers);
                    }

                    candidatesEnrollment = Result.Combine(candidatesEnrollment, enrollment, assigment);
                }
            }

            if (candidates.Contains(false))
            {
                var enrollment = EnrollCandidates(candidates[false], schoolOrNone.Value, out var newMembers);
                if (enrollment.IsSuccess)
                    members.AddRange(newMembers);

                candidatesEnrollment = Result.Combine(candidatesEnrollment, enrollment);
            }

            if (candidatesEnrollment.IsFailure)
                return Result.Failure<IEnumerable<MemberCreatedDTO>, RequestError>(SharedRequestError.General.BusinessRuleViolation(candidatesEnrollment.Error));

            schoolOrNone.Value.MergeEnrollmentEvents();

            await _schoolContext.SaveChangesAsync(cancellationToken);
            var memberDTOs = _mapper.Map<IEnumerable<MemberCreatedDTO>>(members);

            return Result.Success<IEnumerable<MemberCreatedDTO>, RequestError>(memberDTOs);
        }

        private Result<bool, Error> EnrollCandidates(IEnumerable<MemberFromCsvModel> candidates, School school, out List<Member> membersToReturn)
        {
            Result<bool, Error> candidatesEnrollment = Result.Success<bool, Error>(true);
            List<Member> members = new List<Member>();
            foreach (var candidate in candidates)
            {
                var result = school.EnrollCandidate(candidate.FirstName, candidate.LastName, candidate.Email, candidate.Role, candidate.Gender);
                if (candidatesEnrollment.IsFailure || result.IsFailure)
                    candidatesEnrollment = Result.Combine(candidatesEnrollment,
                        Result.Failure<bool, Error>(new Error(result.Error)));
                else
                    members.Add(result.Value);
            }
            membersToReturn = candidatesEnrollment.IsSuccess ? members : new List<Member>();

            return candidatesEnrollment;
        }
    }
}


