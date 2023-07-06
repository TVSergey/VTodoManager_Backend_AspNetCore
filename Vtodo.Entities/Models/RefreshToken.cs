using System;
using System.ComponentModel.DataAnnotations;

namespace Vtodo.Entities.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        
        [Required]
        public int AccountId { get; set; }
        
        [Required]
        public string Token { get; set; } = string.Empty;
        
        [Required]
        public DateTime ExpireAt { get; set; }

        [Required]
        public string Ip { get; set; } = string.Empty;
        
        [Required]
        public string Device { get; set; } = string.Empty;
    }
}