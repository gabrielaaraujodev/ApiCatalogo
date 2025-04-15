using System.ComponentModel.DataAnnotations;

namespace ApiCatalogo.DTOs.Token
{
    public class RegisterModelDTO
    {
        [Required(ErrorMessage = "UserName is required")]
        public string? UserName { get; set; }
  
        [Required(ErrorMessage = "Password is required")]
        public string? Password { get; set; }

        [EmailAddress]
        [Required(ErrorMessage = "Email is required")]
        public string? Email { get; set; }
    }
}
