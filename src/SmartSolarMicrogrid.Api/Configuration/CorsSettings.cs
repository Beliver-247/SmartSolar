/*
 * Purpose: Represents CORS configuration settings from appsettings.
 * Author: Rathnayaka D.M.B.H. (IT22149930)
 * Date: 2026-09-22
 */
namespace SmartSolarMicrogrid.Api.Configuration
{
    public class CorsSettings
    {
        public string[] AllowedOrigins { get; set; } = [];
    }
}
