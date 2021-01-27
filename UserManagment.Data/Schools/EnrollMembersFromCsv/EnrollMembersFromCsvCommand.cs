using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using Fundraiser.SharedKernel.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;

namespace SchoolManagement.Data.Schools.EnrollMembersFromCsv
{
    public sealed class EnrollMembersFromCsvCommand : ICommand<Result<IEnumerable<MemberCreatedDTO>, RequestError>> 
    {
        public IFormFile CsvFile { get; }
        public char Delimiter { get; }
        public Guid SchoolId { get; }
        public Guid AuthId { get; }

        public EnrollMembersFromCsvCommand(IFormFile csvFile, Guid schoolId, Guid authId)
        {
            CsvFile = csvFile;
            SchoolId = schoolId;
            AuthId = authId;
        }
    }
}
