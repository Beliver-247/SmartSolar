/*
 * Purpose: Energy reservation repository interface.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using SmartSolarMicrogrid.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolarMicrogrid.Api.Repositories
{
    public interface IEnergyReservationRepository
    {
        Task<EnergyReservation?> GetByIdAsync(string id);
        Task<IEnumerable<EnergyReservation>> GetAllAsync(int skip, int limit);
        Task<IEnumerable<EnergyReservation>> GetByNicAsync(string nic, int skip, int limit);
        Task<IEnumerable<EnergyReservation>> GetByStatusAsync(string status, int skip, int limit);
        Task<bool> HasActiveReservationsForStationAsync(string stationId);
        Task CreateAsync(EnergyReservation reservation);
        Task UpdateAsync(EnergyReservation reservation);
        Task DeleteAsync(string id);
    }
}
