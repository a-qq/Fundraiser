using CSharpFunctionalExtensions;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;
using Fundraiser.SharedKernel.Utils;
using SchoolManagement.Core.SchoolAggregate.Groups;
using SchoolManagement.Core.SchoolAggregate.Members;

namespace SchoolManagement.Data.Schools.EnrollMembersFromCsv
{
    public sealed class MemberFromCsvMap : ClassMap<MemberFromCsvModel>
    {
        public MemberFromCsvMap()
        {
            Map(m => m.FirstName).TypeConverter<FirstNameConverter>();
            Map(m => m.LastName).TypeConverter<LastNameConverter>();
            Map(m => m.Email).TypeConverter<EmailConverter>();
            Map(m => m.Role).TypeConverter<RoleConverter>();
            Map(m => m.Gender).TypeConverter<GenderConverter>();
            Map(m => m.GroupNumber).TypeConverter<NumberConverter>().Name("Group", "Class");
            Map(m => m.GroupSign).TypeConverter<SignConverter>().Name("Group", "Class");
        }

        public class FirstNameConverter : DefaultTypeConverter 
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                return FirstName.Create(text).Value;
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return (FirstName)value;
            }
        }

        public class LastNameConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                return LastName.Create(text).Value;
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return (LastName)value;
            }
        }

        public class EmailConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                return Email.Create(text).Value;
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return (Email)value;
            }
        }

        public class RoleConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                return Role.Create(text).Value;
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return (Role)value;
            }
        }

        public class GenderConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                return Gender.Create(text).Value;
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                return (Gender)value;
            }
        }

        public class NumberConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                
                return string.IsNullOrEmpty(text) ? Maybe<Number>.None : Maybe<Number>.From(Number.Create(int.Parse(text.Substring(0,1))).Value);
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                var number = value as Maybe<Number>?;
                return number.HasValue && number.Value.HasValue ? number.Value.Value : string.Empty;
            }
        }

        public class SignConverter : DefaultTypeConverter
        {
            public override object ConvertFromString(string text, IReaderRow row, MemberMapData memberMapData)
            {
                return string.IsNullOrEmpty(text) ? Maybe<Sign>.None : Maybe<Sign>.From(Sign.Create(text.Substring(1)).Value);
            }

            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                var sign = value as Maybe<Sign>?;
                return sign.HasValue && sign.Value.HasValue ? sign.Value.Value : string.Empty;
            }
        }

    }
}
