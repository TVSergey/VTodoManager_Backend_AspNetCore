using System.ComponentModel.DataAnnotations;

namespace Vtodo.UseCases.Handlers.Accounts.Dto
{
    public class LogoutDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}