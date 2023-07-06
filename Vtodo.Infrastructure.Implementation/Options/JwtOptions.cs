namespace Vtodo.Infrastructure.Implementation.Options
{
    internal class JwtOptions
    {
        public string Key { get; set; } = string.Empty;
        
        public string RefreshKey { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        
        public int AccessTokenLifeTimeInMinutes { get; set; }
        
        public int RefreshTokenLifeTimeInDays { get; set; }
        
    }
}