using System.Collections.Generic;
using FluentValidation;

namespace SchoolManagement.Application.Schools.Commands.EnrollMembersFromCsv
{
    internal sealed class RawMembersFromCsvModelValidator : AbstractValidator<IEnumerable<RawMemberFromCsvModel>>
    {
        public RawMembersFromCsvModelValidator()
        {
            RuleForEach(x => x).OverrideIndexer((x, collection, element, index) =>
            {
                return "[" + element.RowNumber + "]";
            }).SetValidator(new RawMemberFromCsvModelValidator()).WithName("Records");
        }
    }
}