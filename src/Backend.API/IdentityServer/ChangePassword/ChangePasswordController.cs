using System.Threading.Tasks;
using IDP.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SharedKernel.Infrastructure.Options;

namespace Backend.API.IdentityServer.ChangePassword
{
    public class ChangePasswordController : Controller
    {
        private readonly ILocalUserService _localUserService;
        private readonly UrlsOptions _urls;

        public ChangePasswordController(
            ILocalUserService localUserService,
            IOptions<UrlsOptions> urlsOptions)
        {
            _localUserService = localUserService;
            _urls = urlsOptions.Value;
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

                var result =
                    await _localUserService.ChangePassword(User.Identity.Name, model.OldPassword, model.NewPassword);
                if (result.IsSuccess)
                    return Redirect(_urls.Client);

                ViewData["Error"] = "<p> " + string.Join("<br />", result.Error.Errors) + "</p>";
            }

            return View(model);
        }
    }
}