using FluentValidation;
using System.Collections.Generic;

namespace SchoolManagement.Application.Schools.Commands.EnrollMembersFromCsv
{
    internal sealed class RawMembersFromCsvModelValidator : AbstractValidator<IEnumerable<RawMemberFromCsvModel>>
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
