using System.ComponentModel.DataAnnotations;

namespace WebApi_ClimaTracker.Models.DTOs
{
    public class LoginSuccessDTO
    {
        [Required]
        public string Token { get; set; } = "";
        public string Username { get; set; } = "";
    }
}
