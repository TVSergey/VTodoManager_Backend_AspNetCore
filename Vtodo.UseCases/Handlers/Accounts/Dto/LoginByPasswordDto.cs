using System.ComponentModel.DataAnnotations;

namespace Vtodo.UseCases.Handlers.Accounts.Dto
{
    public class LoginByPasswordDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}