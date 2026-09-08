using System.ComponentModel.DataAnnotations;

namespace WebApi_ClimaTracker.Models.DTOs
{
    public class LoginDTO
    {
        [Required]
        public string Username { get; set; } = "";
        [Required]
        public string Password { get; set; } = "";
    }
}
