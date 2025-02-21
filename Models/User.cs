using System.ComponentModel.DataAnnotations;

namespace BlogWebApp.Models
{
    public class User
    {
        [Key]
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = "User";

        public bool IsApproved { get; set; } = false;
    }
}
