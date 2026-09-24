using System.ComponentModel.DataAnnotations;

namespace ECommerce.APIs.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string DisplayName { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Phone {  get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}

