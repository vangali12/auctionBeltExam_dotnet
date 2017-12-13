using System.ComponentModel.DataAnnotations;

namespace beltexam.Models {
    public class UserViewModel {
        [Required(ErrorMessage = "First Name is Required.")]
        [MinLength(2, ErrorMessage = "First Name must be at least 2 characters long.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "First Name is Required.")]
        [MinLength(2, ErrorMessage = "Last Name must be at least 2 characters long.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email is Required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is requried.")]
        [MinLength(6, ErrorMessage = "Password is not long enough. Must be at least 6 characters.")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm Password is requried.")]
        [MinLength(6)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string cPassword { get; set; }
    }
}