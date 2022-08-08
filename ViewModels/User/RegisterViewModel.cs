using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace uhrenWelt.ViewModels.User
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required]
        [DataType(dataType: DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(dataType: DataType.Password)]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [DisplayName("Confirm password")]
        public string ConfirmPassword { get; set; }

        public string Title { get; set; }

        [Required]
        [DisplayName("First name")]
        public string FirstName { get; set; }

        [Required]
        [DisplayName("Last name")]
        public string LastName { get; set; }

        [Required]
        public string Street { get; set; }

        [Required]
        [DataType(dataType: DataType.PostalCode)]
        [Range(1010, 9992, ErrorMessage = "Please input a valid Austrian Postal code.")]
        public string Zip { get; set; }

        [Required]
        public string City { get; set; }
        [DisplayName("Accept Terms and Conditions")]
        public bool AcceptTerms { get; set; } = true;
    }
}