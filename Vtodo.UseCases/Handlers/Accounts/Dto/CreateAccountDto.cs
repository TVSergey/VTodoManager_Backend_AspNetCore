using System.ComponentModel.DataAnnotations;

namespace Vtodo.UseCases.Handlers.Accounts.Dto
{
    public class CreateAccountDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        public string Username { get; set; } = string.Empty;
        
        public string? Firstname { get; set; }
        
        public string? Surname { get; set; }
        
        [Required]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}