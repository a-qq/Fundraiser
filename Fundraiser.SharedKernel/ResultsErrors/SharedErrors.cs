using Fundraiser.SharedKernel.Utils;
using System;
using System.Collections.Generic;

namespace Fundraiser.SharedKernel.ResultErrors
{
    public static class SharedErrors
    {
        public static class User
        {
            public static RequestError EmailIsTaken(string email) =>
                new RequestError("user.email.is.taken", $"User email '{email}' is taken");

            /* other errors specific to users go here */
        }

        public static class General
        {
            public static RequestError InternalServerError(string message = null)
                => new RequestError("internal.server.error", message ?? "Something gone wrong!:( Try again later!");

            public static RequestError BusinessRuleViolation(string message)
                => new RequestError("business.rule.violation", message);
            public static RequestError BusinessRuleViolation(Error message)
            {
                if (message.Errors.Count == 0)
                    return BusinessRuleViolation("");

                if (message.Errors.Count == 1)
                    return BusinessRuleViolation(message.Errors.GetEnumerator().Current);

                return new RequestError("business.rule.violation", message.Errors);
            }

            public static RequestError UnprocessableEntity(dynamic message)
                => new RequestError("invalid.input.values", message);

            public static RequestError NotFound(string entityName = "Record", string id = null)
            {
                string forId = id == null ? "" : $" for Id '{id}'";
                return new RequestError("record.not.found", $"{entityName} not found{forId}");
            }
            public static RequestError NotFound(Guid id, string entityName = "Record")
                => new RequestError("record.not.found", $"{entityName} not found for Id '{id}'");

            public static RequestError NotFound(IEnumerable<Guid> ids, string entityName = "Record")
                => new RequestError("record.not.found", NotFoundMessages(ids, entityName));

            public static RequestError NotFound(long id, string entityName = "Record")
                => new RequestError("record.not.found", $"{entityName} not found for Id '{id}'");

            /* other general errors go here */

            private static IEnumerable<string> NotFoundMessages<T>(IEnumerable<T> ids, string entityName)
            {
                List<string> messages = new List<string>();

                foreach (var id in ids)
                {
                    string message = $"{entityName} not found for Id '{id}'";
                    messages.Add(message);
                }

                return messages;
            }
        }
    }
}
