/*
 * Purpose: Represents CORS configuration settings from appsettings.
 * Author: Antigravity
 * Date: 2026-09-22
 */
namespace SmartSolarMicrogrid.Api.Configuration
{
    public class CorsSettings
    {
        public string[] AllowedOrigins { get; set; } = [];
    }
}
