using System;
using System.Linq;
using System.Threading.Tasks;
using eSchool.API.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedKernel.Infrastructure.Errors;

namespace eSchool.API.Filters
{
    public sealed class RouteIdsValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!(context.Controller is Controller))
                if (context.HttpContext.Request.RouteValues.Keys.Any(k =>
                    k.Contains("id", StringComparison.OrdinalIgnoreCase)))
                {
                    var idKeyValuePairs = context.HttpContext.Request.RouteValues
                        .Where(rv => rv.Key.Contains("id", StringComparison.OrdinalIgnoreCase)
                                     && (Guid.TryParse(rv.Value as string, out var IdAsGuid) && IdAsGuid == Guid.Empty
                                         || long.TryParse(rv.Value as string, out var IdAsLong) && IdAsLong < 1));

                    if (idKeyValuePairs.Any())
                    {
                        var errors = idKeyValuePairs
                            .Select(kvp =>
                                new RouteValueErrorModel(kvp.Key, $"'{kvp.Key}' value is invalid ('{kvp.Value}')!"))
                            .ToArray();

                        var errorResponse = SharedRequestError.General.UnprocessableEntity(errors);
                        context.Result = new UnprocessableEntityObjectResult(Envelope.Error(errorResponse));

                        return;
                    }
                }

            await next();
            /* previous validation of request body kept for lookup purposes 
            if (!context.ModelState.IsValid)
            {
                var bodyErrorList = new List<dynamic>();
                var errorsInModelState = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(x => x.ErrorMessage)).ToArray();

                foreach (var error in errorsInModelState)
                {
                    foreach (var subError in error.Value)
                    {
                        var errorModel = new
                        {
                            FieldName = error.Key,
                            Message = new List<string>() { subError }
                        };
                        var previousError = bodyErrorList.FirstOrDefault(x => x.FieldName == errorModel.FieldName);
                        if (previousError != null)
                            previousError.Message.Add(errorModel.Message.First());
                        else bodyErrorList.Add(errorModel);
                    }
                }

                errorList.AddRange(bodyErrorList);
            }

            if (errorList.Count != 0)
            {
                var errorResponse = SharedRequestError.General.UnprocessableEntity(errorList);
                context.Result = new UnprocessableEntityObjectResult(Envelope.Error(errorResponse));
                return;
            }
        }
        await next(); */
        }
    }
}