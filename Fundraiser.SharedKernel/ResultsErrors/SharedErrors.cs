using Microsoft.AspNetCore.Mvc.ModelBinding;
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

            /* other errors specific to students go here */
        }

        public static class General
        {
            public static RequestError InternalServerError(string message = null)
                => new RequestError("internal.server.error", message ?? "Something gone wrong!:( Try again later!");
            public static RequestError BusinessRuleViolation(string message)
                => new RequestError("business.rule.violation", message);
            public static RequestError UnprocessableEntity(dynamic message) =>
                new RequestError("invalid.property.values", message);
            /*KeyValuePair<string, IEnumerable<string>>[]*/
            public static RequestError NotFound(string entityName = "Record", string id = null)
            {
                string forId = id == null ? "" : $" for Id '{id}'";
                return new RequestError("record.not.found", $"{entityName} not found{forId}");
            }

            public static RequestError Unauthorized(string id) 
            {
                string withId = id == null ? "" : $"with id '{id}' ";
                return new RequestError("currentuser.unauthorized", $"Current user '{withId}'unauthorized");
            }
            /* other general errors go here */
        }
    }
}
