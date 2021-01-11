using IdentityServer4.Services;
using IDP.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace IdentityServerHost.Quickstart.UI
{
    [SecurityHeaders]
    public class PasswordResetController : Controller
    {
        private readonly ILocalUserService _localUserService;

        public PasswordResetController(ILocalUserService localUserService)
        {
            _localUserService = localUserService;
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
                await _localUserService.SendResetPasswordEmail(model.Email);
                return View("PasswordResetRequestSent");
               // ViewData["Success"] = "If your email is in our system, password reset email has been sent!" + Environment.NewLine
               //     + "Beware that you need to complete registration first!";
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
                    return Redirect(Environment.GetEnvironmentVariable("FrontendSettings__ClientUrl"));

                ViewData["Error"] = result.Error;
            }
            return View(model);
        }
    }
}
