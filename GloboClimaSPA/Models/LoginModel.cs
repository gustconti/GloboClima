using System.ComponentModel.DataAnnotations;

namespace GloboClimaSPA.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long.")]
        public string Password { get; set; }
        
        public LoginModel ()
        { 
            Email = string.Empty;
            Password = string.Empty;
        }
    }
}