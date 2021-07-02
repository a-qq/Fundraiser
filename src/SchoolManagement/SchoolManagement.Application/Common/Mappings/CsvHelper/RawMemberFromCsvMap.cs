using System.Globalization;
using CsvHelper.Configuration;
using SchoolManagement.Application.Common.Models;
using SchoolManagement.Application.Schools.Commands.EnrollMembersFromCsv;

namespace SchoolManagement.Application.Common.Mappings.CsvHelper
{
    internal sealed class RawMemberFromCsvMap : ClassMap<RawMemberFromCsvModel>
    {
        public RawMemberFromCsvMap()
        {
            AutoMap(CultureInfo.InvariantCulture);
            Map(m => m.Group).Name("Group", "Class").Optional();
            Map(m => m.RowNumber).Convert(row => row.Context.Parser.Row).Ignore();
        }
    }
}