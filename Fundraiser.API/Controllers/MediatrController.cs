using CSharpFunctionalExtensions;
using Fundraiser.SharedKernel.RequestErrors;
using IdentityModel;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Fundraiser.API.Controllers
{
    public class MediatrController : ControllerBase
    {
        private readonly IMediator _mediator;
        protected Guid AuthId => string.IsNullOrWhiteSpace(User.FindFirstValue(JwtClaimTypes.Subject))
            ? Guid.Empty : Guid.Parse(User.FindFirstValue(JwtClaimTypes.Subject));
        protected Guid SchoolId => string.IsNullOrWhiteSpace(User.FindFirstValue("school_id"))
            ? Guid.Empty : Guid.Parse(User.FindFirstValue("school_id"));


        protected MediatrController(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected async Task<T> Handle<T>(IRequest<T> request)
        {
            return await _mediator.Send(request);
        }

        protected IActionResult FromResultNoContent<T>(Result<T, RequestError> result)
        {
            var errorResultOrNone = ErrorFromResult(result);
            if (errorResultOrNone.HasValue)
                return errorResultOrNone.Value;

            return base.Ok(Envelope.Ok());
        }

        protected IActionResult FromResultCreatedAt<T>(Result<T, RequestError> result, string routeName, object routeValues = null)
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
