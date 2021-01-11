using IdentityServer4.Services;
using IDP.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace IdentityServerHost.Quickstart.UI
{
    [SecurityHeaders]
    public class RegistrationController : Controller
    {
        private readonly ILocalUserService _localUserService;

        public RegistrationController(ILocalUserService localUserService)
        {
            _localUserService = localUserService;
        }

        //[HttpGet]
        //public async Task<IActionResult> ActivateUser(string securityCode)
        //{
        //    if (await _localUserService.ActivateUser(securityCode))
        //    {
        //        ViewData["Message"] = "Your account was successfully activated.  " +
        //            "Navigate to your client application to log in.";
        //    }
        //    else
        //    {
        //        ViewData["Message"] = "Your account couldn't be activated, " +
        //            "please contact your administrator.";
        //    }

        //    await _localUserService.SaveChangesAsync();

        //    return View();
        //}

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
                   return Redirect(Environment.GetEnvironmentVariable("FrontendSettings__ClientUrl"));

                ViewData["Error"] = result.Error;
            }
            return View(model);
        }
    }
}
