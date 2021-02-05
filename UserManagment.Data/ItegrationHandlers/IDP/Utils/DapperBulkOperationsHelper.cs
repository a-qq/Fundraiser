using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;

namespace SchoolManagement.Data.ItegrationHandlers.IDP
{
    public static class DapperBulkOperationsHelper
    {
        public static DataTable GetUsersInsertTable(IEnumerable<MemberAuthInsertDTO> usersToInsert, DateTime now)
        {
            var output = new DataTable();
           
            output.Columns.Add("Subject", typeof(string));
            output.Columns.Add("Email", typeof(string));
            output.Columns.Add("SecurityCode", typeof(string));
            output.Columns.Add("SecurityCodeIssuedAt", typeof(DateTime));

            foreach (var user in usersToInsert)
                output.Rows.Add(user.Id.ToString(), user.Email, user.SecurityCode, now);

            return output;
        }

        public static DataTable GetClaimsInsertTable(MemberAuthInsertDTO userToInsert)
        {
            var output = new DataTable();
            output.Columns.Add("UserSubject", typeof(string));
            output.Columns.Add("Type", typeof(string));
            output.Columns.Add("Value", typeof(string));

            var subject = userToInsert.Id.ToString();
            output.Rows.Add(subject, "given_name", userToInsert.FirstName);
            output.Rows.Add(subject, "family_name", userToInsert.FirstName);
            output.Rows.Add(subject, "role", userToInsert.Role.ToString());
            output.Rows.Add(subject, "gender", userToInsert.Gender.ToString());
            output.Rows.Add(subject, "school_id", userToInsert.SchoolId.ToString());

            return output;
        }

        public static DataTable GetClaimsInsertTable(IEnumerable<MemberAuthInsertDTO> usersToInsert)
        {
            var output = new DataTable();
            output.Columns.Add("UserSubject", typeof(string));
            output.Columns.Add("Type", typeof(string));
            output.Columns.Add("Value", typeof(string));

            foreach (var user in usersToInsert)
            {
                var subject = user.Id.ToString();
                output.Rows.Add(subject, "given_name", user.FirstName);
                output.Rows.Add(subject, "family_name", user.LastName);
                output.Rows.Add(subject, "role", user.Role.ToString());
                output.Rows.Add(subject, "gender", user.Gender.ToString());
                output.Rows.Add(subject, "school_id", user.SchoolId.ToString());
            }

            return output;
        }

        public static void GenereteSecurityCodes(this IEnumerable<MemberAuthInsertDTO> users)
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                foreach (var user in users)
                {
                    var securityCodeData = new byte[128];
                    randomNumberGenerator.GetBytes(securityCodeData);
                    user.SecurityCode = Convert.ToBase64String(securityCodeData);
                }
            }
        }

        public static void GenereteSecurityCode(this MemberAuthInsertDTO user)
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var securityCodeData = new byte[128];
                randomNumberGenerator.GetBytes(securityCodeData);
                user.SecurityCode = Convert.ToBase64String(securityCodeData);
            }
        }
    }
}
