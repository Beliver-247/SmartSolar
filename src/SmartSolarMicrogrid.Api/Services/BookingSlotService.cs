/*
 * Purpose: Booking slot management.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using SmartSolarMicrogrid.Api.DTOs;
using SmartSolarMicrogrid.Api.Middleware;
using SmartSolarMicrogrid.Api.Models;
using SmartSolarMicrogrid.Api.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSolarMicrogrid.Api.Services
{
    public interface IBookingSlotService
    {
        Task<IEnumerable<SlotResponse>> GetByStationIdAsync(string stationId);
        Task<SlotResponse> CreateAsync(string stationId, CreateSlotRequest request);
    }

    public class BookingSlotService : IBookingSlotService
    {
        private readonly IEnergyBookingSlotRepository _slotRepository;
        private readonly IStationRepository _stationRepository;

        public BookingSlotService(IEnergyBookingSlotRepository slotRepository, IStationRepository stationRepository)
        {
            _slotRepository = slotRepository;
            _stationRepository = stationRepository;
        }

        public async Task<IEnumerable<SlotResponse>> GetByStationIdAsync(string stationId)
        {
            var slots = await _slotRepository.GetByStationIdAsync(stationId);
            return slots.Select(s => new SlotResponse
            {
                Id = s.Id!,
                StationId = s.StationId,
                SlotStart = s.SlotStart,
                SlotEnd = s.SlotEnd,
                Status = s.Status
            });
        }

        public async Task<SlotResponse> CreateAsync(string stationId, CreateSlotRequest request)
        {
            var station = await _stationRepository.GetByIdAsync(stationId);
            if (station == null)
                throw new NotFoundException("Station not found.");

            var slot = new EnergyBookingSlot
            {
                StationId = stationId,
                SlotStart = request.SlotStart,
                SlotEnd = request.SlotEnd,
                Status = SlotStatus.Open
            };

            await _slotRepository.CreateAsync(slot);

            return new SlotResponse
            {
                Id = slot.Id!,
                StationId = slot.StationId,
                SlotStart = slot.SlotStart,
                SlotEnd = slot.SlotEnd,
                Status = slot.Status
            };
        }
    }
}
