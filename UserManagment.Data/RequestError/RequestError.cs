using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using Fundraiser.SharedKernel.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SchoolManagement.Data.ResultErrors
{
    public sealed class ManagementRequestError : RequestError
    {
        //private const string Separator = "||";

        private ManagementRequestError(string code, dynamic message)
        {
            Code = code;
            Message = message;
        }

        public static class Csv
        {
            public static RequestError DuplicateEmails(IEnumerable<Email> emails) =>
                new ManagementRequestError("invalid.csv.emails", $"Duplicate emails in input file: {string.Join(", ", emails)}");

            public static RequestError InvalidHeader(string message) =>
                 new ManagementRequestError("invalid.csv.header", message);
        }

    }
}
