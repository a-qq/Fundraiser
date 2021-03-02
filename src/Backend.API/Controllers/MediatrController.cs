using System;
using System.Security.Claims;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Domain.Utils;
using SharedKernel.Infrastructure.Errors;

namespace Backend.API.Controllers
{
    public class MediatrController : ControllerBase
    {
        private ISender _mediator;

        //mediator is a transient service, so new instance is needed for each request
        private ISender Mediator => _mediator ??= HttpContext.RequestServices.GetService<ISender>();
        protected Guid SchoolId => Guid.Parse(User.FindFirstValue(CustomClaimTypes.SchoolId));

        protected async Task<T> Handle<T>(IRequest<T> request)
        {
            return await Mediator.Send(request);
        }

        protected IActionResult FromResultNoContent<T>(Result<T, RequestError> result)
        {
            var errorResultOrNone = ErrorFromResult(result);
            if (errorResultOrNone.HasValue)
                return errorResultOrNone.Value;

            return base.Ok(Envelope.Ok());
        }

        protected IActionResult FromResultCreatedAt<T>(Result<T, RequestError> result, string routeName,
            object routeValues = null)
        {
            var errorResultOrNone = ErrorFromResult(result);
            if (errorResultOrNone.HasValue)
                return errorResultOrNone.Value;

            if (routeValues != null)
                return base.CreatedAtRoute(routeName, routeValues, Envelope.Ok(result.Value));

            return base.CreatedAtRoute(routeName, Envelope.Ok(result.Value));
        }

        protected IActionResult FromResultOk<T>(Result<T, RequestError> result)
        {
            var errorResultOrNone = ErrorFromResult(result);
            if (errorResultOrNone.HasValue)
                return errorResultOrNone.Value;

            return base.Ok(Envelope.Ok(result.Value));
        }

        private Maybe<IActionResult> ErrorFromResult<T>(IResult<T, RequestError> result)
        {
            if (result.IsSuccess)
                return Maybe<IActionResult>.None;

            if (result.Error == SharedRequestError.General.NotFound(string.Empty, string.Empty))
                return NotFound(Envelope.Error(result.Error));

            return BadRequest(Envelope.Error(result.Error));
        }
    }
}