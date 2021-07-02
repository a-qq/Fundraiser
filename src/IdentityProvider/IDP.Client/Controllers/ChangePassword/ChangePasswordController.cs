using IDP.Application.Users.Commands.ChangePassword;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Infrastructure.Options;
using System.Threading.Tasks;

namespace IDP.Client.Controllers.ChangePassword
{
    public class ChangePasswordController : Controller
    {
        private readonly UrlsOptions _urls;
        private ISender _mediator;
        private ISender Mediator => _mediator ??= HttpContext.RequestServices.GetService<ISender>();

        public ChangePasswordController(UrlsOptions urlsOptions)
        {
            _urls = urlsOptions;
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

                var result = await Mediator.Send(
                    new ChangePasswordCommand(User.Identity.Name, model.OldPassword, model.NewPassword));

                if (result.IsSuccess)
                    return Redirect(_urls.Client);

                ViewData["Error"] = result.Error;
            }

            return View(model);
        }
    }
}