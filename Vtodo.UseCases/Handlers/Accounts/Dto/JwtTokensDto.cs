using System.ComponentModel.DataAnnotations;

namespace Vtodo.UseCases.Handlers.Accounts.Dto
{
    public class JwtTokensDto
    {
        [Required]
        public string AccessToken { get; set; } = string.Empty;
        
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}