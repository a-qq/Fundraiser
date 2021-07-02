using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using SchoolManagement.Domain.Common.Models;
using SchoolManagement.Domain.SchoolAggregate.Groups;
using SchoolManagement.Domain.SchoolAggregate.Members;
using SharedKernel.Domain.ValueObjects;

namespace SchoolManagement.Application.Common.Mappings.CsvHelper
{
    internal class MemberEnrollmentAssignmentDataMap : ClassMap<MemberEnrollmentAssignmentData>
    {
        public MemberEnrollmentAssignmentDataMap()
        {
            Map(m => m.FirstName).TypeConverter<FirstNameConverter>();
            Map(m => m.LastName).TypeConverter<LastNameConverter>();
            Map(m => m.Email).TypeConverter<EmailConverter>();
            Map(m => m.Role).TypeConverter<RoleConverter>();
            Map(m => m.Gender).TypeConverter<GenderConverter>();
            Map(m => m.GroupCode).TypeConverter<CodeConverter>().Name("Group", "Class").Optional();
        }

        private class FirstNameConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                return FirstName.Create(text).Value;
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return (FirstName) value;
            }
        }

        private class LastNameConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                return LastName.Create(text).Value;
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return (LastName) value;
            }
        }

        private class EmailConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                return Email.Create(text).Value;
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return (Email) value;
            }
        }

        private class RoleConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                return Role.Create(text).Value;
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return (Role) value;
            }
        }

        private class GenderConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                return Gender.Create(text).Value;
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return (Gender) value;
            }
        }

        private class CodeConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                if (!string.IsNullOrWhiteSpace(text))
                {
                    var number = Number.Create(int.Parse(text.Substring(0, 1))).Value;
                    var sign = Sign.Create(text[1..]).Value;
                    return Maybe<Code>.From(new Code(number, sign));
                }

                return Maybe<Code>.None;
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                var code = value as Maybe<Code>?;
                return (code.HasValue && code.Value.HasValue) ? code.Value.Value : string.Empty;
            }
        }
    }
}