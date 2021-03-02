using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using MediatR;
using Microsoft.AspNetCore.Http;
using SchoolManagement.Application.Common.Interfaces;
using SchoolManagement.Application.Common.Mappings.CsvHelper;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.Common.Security;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SchoolManagement.Domain.SchoolAggregate.Schools;
using SharedKernel.Infrastructure.Errors;
using SharedKernel.Infrastructure.Interfaces;

namespace SchoolManagement.Application.Schools.Commands.EnrollMembersFromCsv
{
    [Authorize(Policy = "MustBeAtLeastHeadmaster")]
    public sealed class EnrollMembersFromCsvCommand : CommandRequest<IEnumerable<MemberDTO>>
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

    internal sealed class EnrollMembersFromCsvHandler : IRequestHandler<EnrollMembersFromCsvCommand,
        Result<IEnumerable<MemberDTO>, RequestError>>
    {
        private readonly ISchoolContext _context;
        private readonly IEmailUniquenessChecker _emailUniquenessChecker;
        private readonly IMapper _mapper;
        private readonly ISchoolRepository _schoolRepository;

        public EnrollMembersFromCsvHandler(
            ISchoolContext schoolContext,
            ISchoolRepository schoolRepository,
            IEmailUniquenessChecker checker,
            IMapper mapper)
        {
            _context = schoolContext;
            _schoolRepository = schoolRepository;
            _emailUniquenessChecker = checker;
            _mapper = mapper;
        }

        public async Task<Result<IEnumerable<MemberDTO>, RequestError>> Handle(EnrollMembersFromCsvCommand request,
            CancellationToken cancellationToken)
        {
            var schoolId = new SchoolId(request.SchoolId);

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ((char) request.Delimiter).ToString(),
                PrepareHeaderForMatch = (header, index) => header.ToLower()
            };

            IEnumerable<MemberEnrollmentAssignmentData> candidates = null;
            using (var reader = new StreamReader(request.File.OpenReadStream()))
            using (var csv = new CsvReader(reader, config))
            {
                csv.Context.RegisterClassMap<MemberEnrollmentAssignmentDataMap>();
                candidates = csv.GetRecords<MemberEnrollmentAssignmentData>().AsEnumerable();
            }

            var emails = candidates.Select(c => c.Email);

            var duplicates = emails.GroupBy(e => e).SelectMany(g => g.Skip(1)).Distinct();
            if (duplicates.Any())
                return ManagementRequestError.Csv.DuplicateEmails(duplicates.AsEnumerable());

            var schoolOrNone = await _schoolRepository.GetByIdWithMembersAsync(schoolId, cancellationToken);
            if (schoolOrNone.HasNoValue)
                return SharedRequestError.General.NotFound(schoolId, nameof(School));

            //since duplicate check have passed, there are no duplicates in input file
            var (areUnique, duplicateEmails) = await _emailUniquenessChecker.AreUnique(emails /*.AsEnumerable()*/);

            if (!areUnique) //if not unique, return invalid emails
                return SharedRequestError.User.EmailsAreTaken(duplicateEmails);

            var groupedCandidates = candidates.ToLookup(c => c.GroupCode.HasValue);
            if (groupedCandidates.Contains(true))
            {
                var studentGroups = groupedCandidates[true].GroupBy(c => c.GroupCode.Value);

                var notFoundGroupCodes = studentGroups.Select(x => x.Key)
                    .Except(schoolOrNone.Value.Groups.Select(g => g.Code));

                if (notFoundGroupCodes.Count() > 0)
                    return SharedRequestError.General.NotFound(notFoundGroupCodes.Select(x => x.Value), nameof(Group),
                        nameof(Group.Code));
            }

            var membersOrError = schoolOrNone.Value.EnrollCandidates(candidates);

            if (membersOrError.IsFailure)
                return SharedRequestError.General.BusinessRuleViolation(membersOrError.Error);

            await _context.SaveChangesAsync(cancellationToken);

            var memberDTOs = _mapper.Map<IEnumerable<MemberDTO>>(membersOrError.Value);

            return Result.Success<IEnumerable<MemberDTO>, RequestError>(memberDTOs);
        }
    }
}