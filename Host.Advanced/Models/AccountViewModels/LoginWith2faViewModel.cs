using System.ComponentModel.DataAnnotations;

namespace Host.Advanced.Models.AccountViewModels
{
    public class LoginWith2faViewModel
    {
        [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Text)]
        [Display(Name = "Authenticator code")]
        public string TwoFactorCode { get; set; }

        [Display(Name = "App id")]
        public string AppId { get; set; }

        [Display(Name = "Version")]
        public string Version { get; set; }

        [Display(Name = "Device Response")]
        public string DeviceResponse { get; set; }

        [Display(Name = "Challenges")]
        public string Challenges { get; set; }

        [Display(Name = "Challenge")]
        public string Challenge { get; set; }

        [Display(Name = "Remember this machine")]
        public bool RememberMachine { get; set; }

        public bool RememberMe { get; set; }

        public Method2fa Method { get; set; }
    }

    public enum Method2fa
    {
        AuthenticatorCode,
        U2FDevice
    }
}
