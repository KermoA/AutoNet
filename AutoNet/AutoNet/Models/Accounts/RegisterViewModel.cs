using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AutoNet.Models.Accounts
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        //[ValidEmailDomain(allowedDomain: ".com", ErrorMessage = "Email domain must be .com")]
        public string Email { get; set; }

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [DisplayName("Confirm Password")]
        [Compare("Password", ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}
