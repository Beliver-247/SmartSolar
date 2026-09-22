/*
 * Purpose: Unit tests for Station rules (active reservations block deactivation).
 * Author: Antigravity
 * Date: 2026-09-22
 */
using FluentAssertions;
using Moq;
using SmartSolarMicrogrid.Api.Middleware;
using SmartSolarMicrogrid.Api.Models;
using SmartSolarMicrogrid.Api.Repositories;
using SmartSolarMicrogrid.Api.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SmartSolarMicrogrid.Tests.Services
{
    public class StationServiceTests
    {
        private readonly Mock<IStationRepository> _stationRepoMock;
        private readonly Mock<IEnergyReservationRepository> _reservationRepoMock;
        private readonly StationService _service;

        public StationServiceTests()
        {
            _stationRepoMock = new Mock<IStationRepository>();
            _reservationRepoMock = new Mock<IEnergyReservationRepository>();
            _service = new StationService(_stationRepoMock.Object, _reservationRepoMock.Object);
        }

        [Fact]
        public async Task Test3_StationDeactivation_WithActiveReservations_Rejected()
        {
            var stationId = "station1";
            var station = new SolarStationInfo { Id = stationId };
            _stationRepoMock.Setup(x => x.GetByIdAsync(stationId)).ReturnsAsync(station);
            
            // Has active reservations
            _reservationRepoMock.Setup(x => x.HasActiveReservationsForStationAsync(stationId)).ReturnsAsync(true);

            Func<Task> act = async () => await _service.DeleteAsync(stationId);

            await act.Should().ThrowAsync<BusinessRuleException>()
                .WithMessage("Cannot delete/deactivate a station that has active reservations.");
        }

        [Fact]
        public async Task Test3_StationDeactivation_WithoutActiveReservations_Allowed()
        {
            var stationId = "station1";
            var station = new SolarStationInfo { Id = stationId };
            _stationRepoMock.Setup(x => x.GetByIdAsync(stationId)).ReturnsAsync(station);
            
            // NO active reservations
            _reservationRepoMock.Setup(x => x.HasActiveReservationsForStationAsync(stationId)).ReturnsAsync(false);

            Func<Task> act = async () => await _service.DeleteAsync(stationId);

            await act.Should().NotThrowAsync();
            _stationRepoMock.Verify(x => x.DeleteAsync(stationId), Times.Once);
        }
    }
}
