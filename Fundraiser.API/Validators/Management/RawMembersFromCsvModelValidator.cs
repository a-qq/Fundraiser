using FluentValidation;
using SchoolManagement.Data.Schools.EnrollMembersFromCsv;
using System.Collections.Generic;

namespace Fundraiser.API.Validators.Management
{
    public class RawMembersFromCsvModelValidator : AbstractValidator<IEnumerable<RawMemberFromCsvModel>>
    {
        public RawMembersFromCsvModelValidator()
        {
            RuleForEach(x => x).OverrideIndexer((x, collection, element, index) =>
            {
                return "[" + element.RowNumber.ToString() + "]";
            }).SetValidator(new RawMemberFromCsvModelValidator()).WithName("Records");
        }
    }
}
