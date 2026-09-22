/*
 * Purpose: Booking slot repository interface.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using SmartSolarMicrogrid.Api.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartSolarMicrogrid.Api.Repositories
{
    public interface IEnergyBookingSlotRepository
    {
        Task<EnergyBookingSlot?> GetByIdAsync(string id);
        Task<IEnumerable<EnergyBookingSlot>> GetByStationIdAsync(string stationId);
        Task CreateAsync(EnergyBookingSlot slot);
        Task UpdateAsync(EnergyBookingSlot slot);
        Task<bool> UpdateStatusAtomicAsync(string id, string expectedStatus, string newStatus);
    }
}
