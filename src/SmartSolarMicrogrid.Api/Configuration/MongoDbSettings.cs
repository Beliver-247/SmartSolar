/*
 * Purpose: Represents MongoDB configuration settings from appsettings.
 * Author: Rathnayaka D.M.B.H. (IT22149930)
 * Date: 2026-09-22
 */
namespace SmartSolarMicrogrid.Api.Configuration
{
    public class MongoDbSettings
    {
        public string ConnectionString { get; set; } = string.Empty;
        public string DatabaseName { get; set; } = string.Empty;
    }
}
