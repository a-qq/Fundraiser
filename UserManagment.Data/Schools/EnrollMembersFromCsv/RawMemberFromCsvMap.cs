using CsvHelper.Configuration;
using System.Globalization;

namespace SchoolManagement.Data.Schools.EnrollMembersFromCsv
{
    public sealed class RawMemberFromCsvMap : ClassMap<RawMemberFromCsvModel>
    {
        public RawMemberFromCsvMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.Group).Name("Group", "Class").Optional();
            Map(m => m.RowNumber).Convert(row => row.Context.Parser.Row).Ignore();
        }
    }
}
