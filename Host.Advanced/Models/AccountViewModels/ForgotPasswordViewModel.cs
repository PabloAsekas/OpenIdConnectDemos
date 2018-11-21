using System.ComponentModel.DataAnnotations;

namespace Host.Advanced.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
