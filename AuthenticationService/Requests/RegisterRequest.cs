using System.ComponentModel.DataAnnotations;

namespace FileService.Requests
{
    public class RegisterRequest
    {
//        [Required]
//        [EmailAddress]
//        [Display(Name = "Email")]
        public string Email { get; set; }
        
//        [Required]
//        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
//        [DataType(DataType.Password)]
//        [Display(Name = "Password")]
        public string Password { get; set; }
        
//        [Required]
//        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
//        [DataType(DataType.Password)]
//        [Display(Name = "PasswordConfirmation")]
//        [Compare("Password", ErrorMessage = "Confirm password doesn't match, Type again !")]
        public string PasswordConfirmation { get; set; }
    }
}