using System.ComponentModel.DataAnnotations;

namespace Host.Advanced.Models.AccountViewModels
{
    public class ExternalLoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
