using Fundraiser.API.Controllers;
using Fundraiser.SharedKernel.ResultErrors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fundraiser.API.Filters
{
    public sealed class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {

            if (!(context.Controller is Controller))
            {
                var errorList = new List<dynamic>();

                if (context.HttpContext.Request.RouteValues.Keys.Any(k => k.Contains("id", StringComparison.OrdinalIgnoreCase)))
                {
                    var idKeyValuePairs = context.HttpContext.Request.RouteValues
                        .Where(rv => rv.Key.Contains("id", StringComparison.OrdinalIgnoreCase)
                            && (Guid.TryParse(rv.Value as string, out Guid IdAsGuid) && (IdAsGuid == Guid.Empty))
                             || (long.TryParse(rv.Value as string, out long IdAsLong) && (IdAsLong == 0)));
                        

                    foreach (var pair in idKeyValuePairs)
                    {
                        var errorModel = new
                        {
                            RouteKey = pair.Key,
                            Message = $"{pair.Key} cannot be equal {pair.Value}!"
                        };

                        errorList.Add(errorModel);
                    }
                }

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
                    var errorResponse = SharedErrors.General.UnprocessableEntity(errorList);
                    context.Result = new UnprocessableEntityObjectResult(Envelope.Error(errorResponse));
                    return;
                }
            }
            await next();
        }
    }
}
