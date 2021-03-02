using IDP.Application.Common.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedKernel.Infrastructure.Options;
using System.Threading.Tasks;

namespace IdentityServerHost.Quickstart.UI
{
    [SecurityHeaders]
    public class PasswordResetController : Controller
    {
        private readonly ILocalUserService _localUserService;
        private readonly UrlsOptions _urls;

        public PasswordResetController(
            ILocalUserService localUserService,
            IOptions<UrlsOptions> urlsOptions)
        {
            _localUserService = localUserService;
            _urls = urlsOptions.Value;
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
                await _localUserService.RequestPasswordReset(model.Email);

                return View("PasswordResetRequestSent");
            }

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult ResetPassword(string securityCode)
        {
            var vm = new ResetPasswordViewModel() { SecurityCode = securityCode };
            return View(vm);
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _localUserService.ResetPassword(model.SecurityCode, model.Password);
                if (result.IsSuccess)
                    this.LoadingPage("Redirect", _urls.Client);

                ViewData["Error"] = "<p> " + string.Join("<br />", result.Error.Errors) + "</p>";
            }
            return View(model);
        }
    }
}
