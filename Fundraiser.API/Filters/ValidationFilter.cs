using Fundraiser.API.Controllers;
using Fundraiser.SharedKernel.ResultErrors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fundraiser.API.Filters
{
    public sealed class ValidationFilter : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (!context.ModelState.IsValid && !(context.Controller is Controller))
            {
                var errorsInModelState = context.ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value.Errors.Select(x => x.ErrorMessage)).ToArray();

                var errorList = new List<dynamic>();
                foreach (var error in errorsInModelState)
                {
                    foreach (var subError in error.Value)
                    {
                        var errorModel = new 
                        {
                            FieldName = error.Key,
                            Message = new List<string>() { subError }
                        };
                        var previousError = errorList.FirstOrDefault(x => x.FieldName == errorModel.FieldName);
                        if (previousError != null)
                            previousError.Message.Add(errorModel.Message.First());
                        else errorList.Add(errorModel);
                    }
                }
                var errorResponse = SharedErrors.General.UnprocessableEntity(errorList);
                context.Result = new UnprocessableEntityObjectResult(Envelope.Error(errorResponse));
                return;
            }

            await next();
        }
    }
}
