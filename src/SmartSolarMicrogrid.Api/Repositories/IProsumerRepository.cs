/*
 * Purpose: Prosumer repository interface.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using SmartSolarMicrogrid.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolarMicrogrid.Api.Repositories
{
    public interface IProsumerRepository
    {
        Task<Prosumer?> GetByNicAsync(string nic);
        Task<IEnumerable<Prosumer>> GetAllAsync(int skip, int limit);
        Task CreateAsync(Prosumer prosumer);
        Task UpdateAsync(Prosumer prosumer);
    }
}
