using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;
using SchoolManagement.Data.Schools.EnrollMembersFromCsv;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace Fundraiser.API.Validators.Management
{
    public sealed class EnrollMembersFromCsvRequestValidator : AbstractValidator<EnrollMembersFromCsvRequest>
    {
        public EnrollMembersFromCsvRequestValidator()
        {
            RuleFor(p => p.File).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} is required!")
                .Must(p => p.Length < 3145728).WithMessage("{PropertyName} must be under 3 MB!")
                .Must(p => (p.ContentType == "text/csv" || p.ContentType == "text/plain" || p.ContentType == "application/vnd.ms-excel")
                 && new List<string>() { ".csv", ".txt" }.Contains(Path.GetExtension(p.FileName)))
                  .WithMessage("{PropertyName} must be in '.csv' or '.txt' format!")
                    .DependentRules(() =>
                    {
                        When(x => Enum.IsDefined(typeof(DelimiterEnum), Convert.ToInt32(x.Delimiter)), () =>
                        RuleFor(p => p.File).Custom((file, context) =>
                        {
                            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                            {
                                PrepareHeaderForMatch = (string header, int index) => header.ToLower(),
                                Delimiter = this.GetValueofDelimiter(context.ParentContext.InstanceToValidate as EnrollMembersFromCsvRequest)
                            };
                            using (var reader = new StreamReader(file.OpenReadStream()))
                            using (var csv = new CsvReader(reader, config))
                            {
                                csv.Context.RegisterClassMap<RawMemberFromCsvMap>();
                                try
                                {
                                    var records = csv.GetRecords<RawMemberFromCsvModel>();
                                    var validationContext = new ValidationContext<IEnumerable<RawMemberFromCsvModel>>(records);
                                    var validator = new RawMembersFromCsvModelValidator();
                                    var result = validator.Validate(validationContext);
                                    if (!result.IsValid)
                                        foreach (var error in result.Errors)
                                        {
                                            context.AddFailure(error);
                                        }
                                }
                                catch (HeaderValidationException ex)
                                {
                                    int index = ex.Message.IndexOf("\r\nIf");
                                    var message = index > 0 ? ex.Message.Remove(index) : ex.Message;
                                    string[] messages = message.Replace("[0]", "").Split("\r\n");
                                    foreach (var error in messages)
                                        context.AddFailure(error);
                                }
                            }
                        }));
                    });

            RuleFor(p => p.Delimiter).Must(x => Enum.IsDefined(typeof(DelimiterEnum), Convert.ToInt32(x)))
                .WithMessage("Delimiter must be Comma, Semicolon, Tab or Space!");
        }

        private string GetValueofDelimiter(EnrollMembersFromCsvRequest request)
        {
            var delimiter =  ((char)request.Delimiter).ToString();
            return delimiter;
        }
    }
}
