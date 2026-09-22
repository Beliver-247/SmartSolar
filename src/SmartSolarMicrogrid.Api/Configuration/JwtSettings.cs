/*
 * Purpose: Represents JWT configuration settings from appsettings.
 * Author: Antigravity
 * Date: 2026-09-22
 */
namespace SmartSolarMicrogrid.Api.Configuration
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; }
    }
}
