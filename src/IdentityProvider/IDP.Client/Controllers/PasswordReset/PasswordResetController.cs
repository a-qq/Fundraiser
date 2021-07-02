using IDP.Application.Users.Commands;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SharedKernel.Infrastructure.Options;
using System.Threading.Tasks;
using IDP.Application.Users.Commands.RequestPasswordReset;
using IDP.Application.Users.Commands.ResetPassword;

namespace IDP.Client.Controllers.PasswordReset
{
    [SecurityHeaders]
    public class PasswordResetController : Controller
    {
        private readonly UrlsOptions _urls;
        private ISender _mediator;
        private ISender Mediator => _mediator ??= HttpContext.RequestServices.GetService<ISender>();

        public PasswordResetController(
            UrlsOptions urlsOptions)
        {
            _urls = urlsOptions;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult RequestPasswordReset()
        {
            var vm = new RequestPasswordResetViewModel();
            return View(vm);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestPasswordReset(RequestPasswordResetViewModel model)
        {
            if (ModelState.IsValid)
            {
                await Mediator.Send(new RequestPasswordResetCommand(model.Email));

                return View("PasswordResetRequestSent");
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string securityCode)
        {
            var vm = new ResetPasswordViewModel {SecurityCode = securityCode};
            return View(vm);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Mediator.Send(new ResetPasswordCommand(model.SecurityCode, model.Password));
                if (result.IsSuccess)
                    this.LoadingPage("Redirect", _urls.Client);

                ViewData["Error"] = result.Error;
            }

            return View(model);
        }
    }
}