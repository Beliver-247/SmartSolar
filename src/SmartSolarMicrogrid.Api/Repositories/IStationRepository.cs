/*
 * Purpose: Station repository interface.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using SmartSolarMicrogrid.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolarMicrogrid.Api.Repositories
{
    public interface IStationRepository
    {
        Task<SolarStationInfo?> GetByIdAsync(string id);
        Task<IEnumerable<SolarStationInfo>> GetAllAsync(int skip, int limit);
        Task CreateAsync(SolarStationInfo station);
        Task UpdateAsync(SolarStationInfo station);
        Task DeleteAsync(string id);
    }
}
