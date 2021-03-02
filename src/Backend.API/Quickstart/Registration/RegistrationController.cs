using IDP.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedKernel.Infrastructure.Options;
using System;
using System.Threading.Tasks;

namespace IdentityServerHost.Quickstart.UI
{
    [SecurityHeaders]
    public class RegistrationController : Controller
    {
        private readonly ILocalUserService _localUserService;
        private readonly UrlsOptions _urls;

        public RegistrationController(
            ILocalUserService localUserService,
            IOptions<UrlsOptions> urlsOptions)
        {
            _localUserService = localUserService;
            _urls = urlsOptions.Value;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register(string securityCode, string returnUrl)
        {
            var vm = new RegisterViewModel()
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
                var result = await _localUserService.CompleteRegistration(model.SecurityCode, model.Password);
                if (result.IsSuccess)
                   return Redirect(_urls.Client);

                ViewData["Error"] = "<p> " + string.Join("<br />", result.Error.Errors) + "</p>";
            }
            return View(model);
        }
    }
}
