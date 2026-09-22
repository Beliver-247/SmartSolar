/*
 * Purpose: Solar station management.
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
    public interface IStationService
    {
        Task<IEnumerable<StationResponse>> GetAllAsync(int page, int pageSize);
        Task<StationResponse> GetByIdAsync(string id);
        Task<StationResponse> CreateAsync(CreateStationRequest request);
        Task<StationResponse> UpdateAsync(string id, UpdateStationRequest request);
        Task DeleteAsync(string id);
    }

    public class StationService : IStationService
    {
        private readonly IStationRepository _stationRepository;
        private readonly IEnergyReservationRepository _reservationRepository;

        public StationService(IStationRepository stationRepository, IEnergyReservationRepository reservationRepository)
        {
            _stationRepository = stationRepository;
            _reservationRepository = reservationRepository;
        }

        public async Task<IEnumerable<StationResponse>> GetAllAsync(int page, int pageSize)
        {
            // Paginated list of stations
            var skip = (page - 1) * pageSize;
            var stations = await _stationRepository.GetAllAsync(skip, pageSize);
            return stations.Select(s => new StationResponse
            {
                Id = s.Id!,
                StationName = s.StationName,
                GpsLocation = s.GpsLocation,
                CapacityKwh = s.CapacityKwh,
                BatterySlots = s.BatterySlots,
                Schedule = s.Schedule,
                IsActive = s.IsActive
            });
        }

        public async Task<StationResponse> GetByIdAsync(string id)
        {
            // Gets station by Id
            var s = await _stationRepository.GetByIdAsync(id);
            if (s == null)
                throw new NotFoundException("Station not found.");

            return new StationResponse
            {
                Id = s.Id!,
                StationName = s.StationName,
                GpsLocation = s.GpsLocation,
                CapacityKwh = s.CapacityKwh,
                BatterySlots = s.BatterySlots,
                Schedule = s.Schedule,
                IsActive = s.IsActive
            };
        }

        public async Task<StationResponse> CreateAsync(CreateStationRequest request)
        {
            // Creates a new station
            var station = new SolarStationInfo
            {
                StationName = request.StationName,
                GpsLocation = request.GpsLocation,
                CapacityKwh = request.CapacityKwh,
                BatterySlots = request.BatterySlots,
                Schedule = request.Schedule,
                IsActive = true
            };

            await _stationRepository.CreateAsync(station);

            return new StationResponse
            {
                Id = station.Id!,
                StationName = station.StationName,
                GpsLocation = station.GpsLocation,
                CapacityKwh = station.CapacityKwh,
                BatterySlots = station.BatterySlots,
                Schedule = station.Schedule,
                IsActive = station.IsActive
            };
        }

        public async Task<StationResponse> UpdateAsync(string id, UpdateStationRequest request)
        {
            // Updates station info
            var station = await _stationRepository.GetByIdAsync(id);
            if (station == null)
                throw new NotFoundException("Station not found.");

            station.StationName = request.StationName;
            station.GpsLocation = request.GpsLocation;
            station.CapacityKwh = request.CapacityKwh;
            station.BatterySlots = request.BatterySlots;
            station.Schedule = request.Schedule;

            await _stationRepository.UpdateAsync(station);

            return new StationResponse
            {
                Id = station.Id!,
                StationName = station.StationName,
                GpsLocation = station.GpsLocation,
                CapacityKwh = station.CapacityKwh,
                BatterySlots = station.BatterySlots,
                Schedule = station.Schedule,
                IsActive = station.IsActive
            };
        }

        public async Task DeleteAsync(string id)
        {
            // Deletes (or deactivates) a station.
            // Rule 3: Deleting/deactivating a station must fail if it has active reservations (Pending or Approved).
            var station = await _stationRepository.GetByIdAsync(id);
            if (station == null)
                throw new NotFoundException("Station not found.");

            var hasActiveReservations = await _reservationRepository.HasActiveReservationsForStationAsync(id);
            if (hasActiveReservations)
            {
                throw new BusinessRuleException("Cannot delete/deactivate a station that has active reservations.");
            }

            // Using soft delete for deactivation or physical delete as specified. 
            // The instructions say "Deleting/deactivating a station must fail", I'll physically delete it to match the REST DELETE verb semantics cleanly, 
            // or we could mark IsActive = false. The specification allows either as long as the rule is checked.
            await _stationRepository.DeleteAsync(id);
        }
    }
}
