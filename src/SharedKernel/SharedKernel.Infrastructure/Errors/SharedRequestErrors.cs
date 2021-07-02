using System;
using System.Collections.Generic;
using System.Linq;
using SharedKernel.Domain.Errors;
using SharedKernel.Domain.ValueObjects;

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
            {
                return new SharedRequestError("internal.server.error", message ?? "Internal server error.");
            }

            public static RequestError UnauthorizedAccess()
            {
                return new SharedRequestError("unauthorized", string.Empty);
            }

            public static RequestError ForbiddenAccess()
            {
                return new SharedRequestError("forbidden", string.Empty);
            }

            public static RequestError BusinessRuleViolation(string message)
            {
                return new SharedRequestError("business.rule.violation", message);
            }

            public static RequestError BusinessRuleViolation(Error message)
            {
                if (message.Errors.Count == 0)
                    return BusinessRuleViolation(string.Empty);

                if (message.Errors.Count == 1)
                    return BusinessRuleViolation(message.Errors.First());

                return new SharedRequestError("business.rule.violation", message.Errors);
            }

            public static RequestError UnprocessableEntity(IEnumerable<BodyFieldErrorModel> errors)
            {
                return new SharedRequestError("invalid.body.input.values", errors);
            }

            public static RequestError UnprocessableEntity(IEnumerable<RouteValueErrorModel> errors)
            {
                return new SharedRequestError("invalid.route.input.values", errors);
            }

            public static RequestError NotFound(string entityName = "Record", string id = null)
            {
                var forId = id == null ? "" : $" for Id '{id}'";
                return new SharedRequestError("record.not.found", $"{entityName} not found{forId}!");
            }

            public static RequestError NotFound(Guid id, string entityName = "Record")
            {
                return new SharedRequestError("record.not.found", $"{entityName} not found for Id '{id}'");
            }

            public static RequestError NotFound(IEnumerable<Guid> ids, string entityName = "Record")
            {
                return new SharedRequestError("record.not.found", NotFoundMessages(ids, entityName));
            }

            public static RequestError NotFound(IEnumerable<string> uniqueProperties, string entityName = "Record",
                string propertyName = "Identifier")
            {
                return new SharedRequestError("record.not.found",
                    NotFoundMessages(uniqueProperties, entityName, propertyName));
            }


            //helper processing methods
            private static IEnumerable<string> NotFoundMessages(IEnumerable<Guid> ids, string entityName)
            {
                var messages = new List<string>();

                foreach (var id in ids)
                {
                    var message = $"{entityName} not found for Id '{id}'";
                    messages.Add(message);
                }

                return messages;
            }

            private static IEnumerable<string> NotFoundMessages(IEnumerable<string> uniqueProperties, string entityName,
                string propertyName)
            {
                var messages = new List<string>();

                foreach (var prop in uniqueProperties)
                {
                    var message = $"{entityName} not found for {propertyName} '{prop}'";
                    messages.Add(message);
                }

                return messages;
            }
        }

        public static class User
        {
            public static RequestError EmailIsTaken(Email email)
            {
                return new SharedRequestError("user.email.is.taken", $"User email '{email}' is already taken!");
            }

            public static RequestError EmailsAreTaken(IEnumerable<Email> emails)
            {
                return new SharedRequestError("user.email.is.taken", EmailsAreTakenMessages(emails));
            }

            //helper processing methods
            private static IEnumerable<string> EmailsAreTakenMessages(IEnumerable<Email> emails)
            {
                var messages = new List<string>();

                foreach (var email in emails)
                {
                    var message = $"User email '{email}' is already taken!";
                    messages.Add(message);
                }

                return messages;
            }
        }
    }
}