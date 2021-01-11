using IDP.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace IdentityServerHost.Quickstart.UI
{
    public class ChangePasswordController : Controller
    {
        private readonly ILocalUserService _localUserService;

        public ChangePasswordController(ILocalUserService localUserService)
        {
            _localUserService = localUserService;
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            if (User == null || User?.Identity.IsAuthenticated == false)
                return Redirect("~/Account/Login?ReturnUrl=/ChangePassword/ChangePassword");

            var vm = new ChangePasswordViewModel();
            return View(vm);
        }


        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (User == null || User?.Identity.IsAuthenticated == false)
                    return Redirect("~/Account/Login?ReturnUrl=/ChangePassword/ChangePassword");

                var result = await _localUserService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                if (result.IsSuccess)
                    return Redirect(Environment.GetEnvironmentVariable("FrontendSettings__ClientUrl"));
                ViewData["Error"] = result.Error;
            }
            return View(model);
        }
    }
}
