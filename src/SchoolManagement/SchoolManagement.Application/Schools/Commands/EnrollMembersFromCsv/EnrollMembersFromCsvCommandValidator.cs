using CsvHelper;
using CsvHelper.Configuration;
using FluentValidation;
using SchoolManagement.Application.Common.Mappings.CsvHelper;
using SharedKernel.Infrastructure.Extensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using SchoolManagement.Application.Common.Models;

namespace SchoolManagement.Application.Schools.Commands.EnrollMembersFromCsv
{
    public sealed class EnrollMembersFromCsvCommandValidator : AbstractValidator<EnrollMembersFromCsvCommand>
    {
        public EnrollMembersFromCsvCommandValidator()
        {
            RuleFor(p => p.File).Cascade(CascadeMode.Stop)
                .NotEmpty().WithMessage("{PropertyName} is required!")
                .Must(p => p.Length < 3145728).WithMessage("{PropertyName} must be under 3 MB!")
                .Must(p => (p.ContentType == "text/csv" || p.ContentType == "text/plain" ||
                            p.ContentType == "application/vnd.ms-excel")
                           && new List<string> { ".csv", ".txt" }.Contains(Path.GetExtension(p.FileName)))
                .WithMessage("{PropertyName} must be in '.csv' or '.txt' format!")
                .DependentRules(() =>
                {
                    When(x => Enum.IsDefined(typeof(DelimiterEnum), Convert.ToInt32(x.Delimiter)), () =>
                        RuleFor(p => p.File).Custom((file, context) =>
                        {
                            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
                            {
                                PrepareHeaderForMatch = (header, index) => header.ToLower(),
                                Delimiter = GetValueOfDelimiter(
                                    context.ParentContext.InstanceToValidate as EnrollMembersFromCsvCommand)
                            };

                            try
                            {
                                using (var reader = new StreamReader(file.OpenReadStream()))
                                using (var csv = new CsvReader(reader, config))
                                {
                                    csv.Context.RegisterClassMap<RawMemberFromCsvMap>();
                                    var records = csv.GetRecords<RawMemberFromCsvModel>();
                                    var duplicateEmails = records
                                        .Select(c => c.Email)
                                        .GroupBy(e => e)
                                        .SelectMany(g => g.Skip(1))
                                        .ToHashSet();

                                    if (duplicateEmails.Any())
                                        context.AddFailure(context.PropertyName + ".Records",
                                            $"Duplicate emails in input file: {string.Join("\n", duplicateEmails)}");
                                }
                                using (var reader = new StreamReader(file.OpenReadStream()))
                                using (var csv = new CsvReader(reader, config))
                                {
                                    var recordValidator = new RawMemberFromCsvModelValidator();
                                    csv.Context.RegisterClassMap<RawMemberFromCsvMap>();
                                    foreach (var record in csv.GetRecords<RawMemberFromCsvModel>())
                                    {
                                        var validationContext = new ValidationContext<RawMemberFromCsvModel>(record);
                                        var result = recordValidator.Validate(validationContext);
                                        if (result.IsValid)
                                            continue;

                                        foreach (var error in result.Errors)
                                            context.AddFailure(context.PropertyName
                                                 + $".Records[{record.RowNumber}]." + error.PropertyName, error.ErrorMessage);
                                    }
                                }
                            }
                            catch (HeaderValidationException ex)
                            {
                                var index = ex.Message.IndexOf("\r\nIf", StringComparison.Ordinal);
                                var message = index > 0 ? ex.Message.Remove(index) : ex.Message;
                                var messages = message.Replace("[0]", "").Split("\r\n");
                                foreach (var error in messages)
                                    context.AddFailure(error);
                            }

                        }));
                });

            RuleFor(p => p.Delimiter).Must(x => Enum.IsDefined(typeof(DelimiterEnum), Convert.ToInt32(x)))
                .WithMessage("Delimiter must be Comma, Semicolon, Tab or Space!");

            RuleFor(p => p.SchoolId).GuidIdMustBeValid();
        }

        private string GetValueOfDelimiter(EnrollMembersFromCsvCommand request)
        {
            var delimiter = ((char)request.Delimiter).ToString();
            return delimiter;
        }
    }
}