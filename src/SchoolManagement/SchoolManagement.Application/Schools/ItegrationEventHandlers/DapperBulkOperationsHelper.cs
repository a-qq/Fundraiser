using System;
using System.Collections.Generic;
using System.Data;
using System.Security.Cryptography;
using IdentityModel;
using SchoolManagement.Application.Common.Models;
using SharedKernel.Domain.Utils;

namespace SchoolManagement.Application.Schools.ItegrationEventHandlers
{
    internal static class DapperBulkOperationsHelper
    {
        public static DataTable GetUsersInsertTable(IEnumerable<MemberAuthInsertModel> usersToInsert, DateTime now)
        {
            var output = CreateUsersInsertTable();

            foreach (var user in usersToInsert)
                output.Rows.Add(user.Id.ToString(), user.Email, user.SecurityCode, now);

            return output;
        }

        public static DataTable GetClaimsInsertTable(MemberAuthInsertModel userToInsert)
        {
            var output = CreateClaimsInsertTable();

            var subject = userToInsert.Id.ToString();

            output.Rows.Add(subject, JwtClaimTypes.GivenName, userToInsert.FirstName);
            output.Rows.Add(subject, JwtClaimTypes.FamilyName, userToInsert.LastName);
            output.Rows.Add(subject, JwtClaimTypes.Role, userToInsert.Role.ToString());
            output.Rows.Add(subject, JwtClaimTypes.Gender, userToInsert.Gender.ToString());
            output.Rows.Add(subject, CustomClaimTypes.SchoolId, userToInsert.SchoolId.ToString());

            return output;
        }

        public static DataTable GetClaimsInsertTable(IEnumerable<MemberAuthInsertModel> usersToInsert)
        {
            var output = CreateClaimsInsertTable();

            foreach (var user in usersToInsert)
            {
                var subject = user.Id.ToString();
                output.Rows.Add(subject, JwtClaimTypes.GivenName, user.FirstName);
                output.Rows.Add(subject, JwtClaimTypes.FamilyName, user.LastName);
                output.Rows.Add(subject, JwtClaimTypes.Role, user.Role.ToString());
                output.Rows.Add(subject, JwtClaimTypes.Gender, user.Gender.ToString());
                output.Rows.Add(subject, CustomClaimTypes.SchoolId, user.SchoolId.ToString());

                if (user.GroupId.HasValue)
                    output.Rows.Add(subject, CustomClaimTypes.GroupId, user.GroupId.ToString());
            }

            return output;
        }

        public static void GenereteSecurityCodes(this IEnumerable<MemberAuthInsertModel> users)
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

        public static void GenereteSecurityCode(this MemberAuthInsertModel user)
        {
            using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            {
                var securityCodeData = new byte[128];
                randomNumberGenerator.GetBytes(securityCodeData);
                user.SecurityCode = Convert.ToBase64String(securityCodeData);
            }
        }

        public static DataTable CreateClaimsInsertTable()
        {
            var output = new DataTable();

            output.Columns.Add("UserSubject", typeof(string));
            output.Columns.Add("Type", typeof(string));
            output.Columns.Add("Value", typeof(string));

            return output;
        }

        private static DataTable CreateUsersInsertTable()
        {
            var output = new DataTable();

            output.Columns.Add("Subject", typeof(string));
            output.Columns.Add("Email", typeof(string));
            output.Columns.Add("SecurityCode", typeof(string));
            output.Columns.Add("SecurityCodeIssuedAt", typeof(DateTime));

            return output;
        }
    }
}