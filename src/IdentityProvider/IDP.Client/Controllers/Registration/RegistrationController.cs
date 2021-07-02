using IDP.Application.Users.Commands.CompleteRegistration;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Infrastructure.Options;
using System.Threading.Tasks;

namespace IDP.Client.Controllers.Registration
{
    [SecurityHeaders]
    public class RegistrationController : Controller
    {
        private readonly UrlsOptions _urls;
        private ISender _mediator;
        private ISender Mediator => _mediator ??= HttpContext.RequestServices.GetService<ISender>();

        public RegistrationController(UrlsOptions urlsOptions)
        {
            _urls = urlsOptions;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register(string securityCode, string returnUrl)
        {
            var vm = new RegisterViewModel
            {
                SecurityCode = securityCode,
                RedirectUrl = returnUrl
            };
            return View(vm);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await Mediator.Send(
                    new CompleteRegistrationCommand(model.SecurityCode, model.Password));

                if (result.IsSuccess)
                    return Redirect(_urls.Client);

                ViewData["Error"] = result.Error;
            }

            return View(model);
        }
    }
}