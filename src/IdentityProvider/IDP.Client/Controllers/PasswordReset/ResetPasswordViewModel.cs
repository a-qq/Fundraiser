using System.ComponentModel.DataAnnotations;

namespace IDP.Client.Controllers.PasswordReset
{
    public class ResetPasswordViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        public string ConfirmPassword { get; set; }

        public string SecurityCode { get; set; }
        public string RedirectUrl { get; set; }
    }
}