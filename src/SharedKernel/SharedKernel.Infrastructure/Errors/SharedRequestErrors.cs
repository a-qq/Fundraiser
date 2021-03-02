using SharedKernel.Domain.Errors;
using SharedKernel.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SharedKernel.Infrastructure.Errors
{
    public sealed class SharedRequestError : RequestError
    {
        private SharedRequestError(string code, dynamic message)
        {
            Code = code;
            Message = message;
        }

        public static class General
        {
            public static RequestError InternalServerError(string message = null)
                => new SharedRequestError("internal.server.error", message ?? "Internal server error.");

            public static RequestError UnauthorizedAccess()
                => new SharedRequestError("unauthorized", string.Empty);

            public static RequestError ForbiddenAccess()
                => new SharedRequestError("forbidden", string.Empty);

            public static RequestError BusinessRuleViolation(string message)
                => new SharedRequestError("business.rule.violation", message);

            public static RequestError BusinessRuleViolation(Error message)
            {
                if (message.Errors.Count == 0)
                    return BusinessRuleViolation(string.Empty);

                if (message.Errors.Count == 1)
                    return BusinessRuleViolation(message.Errors.First());

                return new SharedRequestError("business.rule.violation", message.Errors);
            }

            public static RequestError UnprocessableEntity(IEnumerable<BodyFieldErrorModel> errors)
                => new SharedRequestError("invalid.body.input.values", errors);

            public static RequestError UnprocessableEntity(IEnumerable<RouteValueErrorModel> errors)
                => new SharedRequestError("invalid.route.input.values", errors);

            public static RequestError NotFound(string id = null, string entityName = "Record")
            {
                string forId = id == null ? "" : $" for Id '{id}'";
                return new SharedRequestError("record.not.found", $"{entityName} not found{forId}!");
            }

            public static RequestError NotFound(Guid id, string entityName = "Record")
                => new SharedRequestError("record.not.found", $"{entityName} not found for Id '{id}'");

            public static RequestError NotFound(IEnumerable<Guid> ids, string entityName = "Record")
                => new SharedRequestError("record.not.found", NotFoundMessages(ids, entityName));

            public static RequestError NotFound(long id, string entityName = "Record")
                => new SharedRequestError("record.not.found", $"{entityName} not found for Id '{id}'");

            public static RequestError NotFound(IEnumerable<string> uniqueProperties, string entityName = "Record", string propertyName = "Identifier")
                => new SharedRequestError("record.not.found", NotFoundMessages(uniqueProperties, entityName, propertyName));


            //helper processing methods
            private static IEnumerable<string> NotFoundMessages(IEnumerable<Guid> ids, string entityName)
            {
                List<string> messages = new List<string>();

                foreach (var id in ids)
                {
                    string message = $"{entityName} not found for Id '{id}'";
                    messages.Add(message);
                }

                return messages;
            }

            private static IEnumerable<string> NotFoundMessages(IEnumerable<string> uniqueProperties, string entityName, string propertyName)
            {
                List<string> messages = new List<string>();

                foreach (var prop in uniqueProperties)
                {
                    string message = $"{entityName} not found for {propertyName} '{prop}'";
                    messages.Add(message);
                }

                return messages;
            }
        }

        public static class User
        {
            public static RequestError EmailIsTaken(Email email) =>
                new SharedRequestError("user.email.is.taken", $"User email '{email}' is already taken!");

            public static RequestError EmailsAreTaken(IEnumerable<Email> emails) =>
                 new SharedRequestError("user.email.is.taken", EmailsAreTakenMessages(emails));

            //helper processing methods
            private static IEnumerable<string> EmailsAreTakenMessages(IEnumerable<Email> emails)
            {
                List<string> messages = new List<string>();

                foreach (var email in emails)
                {
                    string message = $"User email '{email}' is already taken!";
                    messages.Add(message);
                }

                return messages;
            }
        }
    }
}
