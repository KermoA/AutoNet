using System.ComponentModel.DataAnnotations;

namespace AutoNet.Models.Accounts
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
