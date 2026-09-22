/*
 * Purpose: Unit tests for Reservation rules (7-day rule, 12-hour rule).
 * Author: Antigravity
 * Date: 2026-09-22
 */
using FluentAssertions;
using Moq;
using SmartSolarMicrogrid.Api.DTOs;
using SmartSolarMicrogrid.Api.Models;
using SmartSolarMicrogrid.Api.Repositories;
using SmartSolarMicrogrid.Api.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace SmartSolarMicrogrid.Tests.Services
{
    public class ReservationServiceTests
    {
        private readonly Mock<IEnergyReservationRepository> _reservationRepoMock;
        private readonly Mock<IEnergyBookingSlotRepository> _slotRepoMock;
        private readonly Mock<IProsumerRepository> _prosumerRepoMock;
        private readonly ReservationService _service;

        public ReservationServiceTests()
        {
            _reservationRepoMock = new Mock<IEnergyReservationRepository>();
            _slotRepoMock = new Mock<IEnergyBookingSlotRepository>();
            _prosumerRepoMock = new Mock<IProsumerRepository>();

            _service = new ReservationService(
                _reservationRepoMock.Object,
                _slotRepoMock.Object,
                _prosumerRepoMock.Object);
        }

        [Fact]
        public async Task Test1_SevenDayReservationRule_Valid_Succeeds()
        {
            // Verify: valid reservation within 7 days -> succeeds
            var now = DateTimeOffset.UtcNow;
            var slotId = "slot1";
            var nic = "123456789V";

            var prosumer = new Prosumer { Nic = nic, IsActive = true };
            _prosumerRepoMock.Setup(x => x.GetByNicAsync(nic)).ReturnsAsync(prosumer);

            // Slot starts in 3 days (within 7 days)
            var slot = new EnergyBookingSlot { Id = slotId, Status = SlotStatus.Open, SlotStart = now.AddDays(3) };
            _slotRepoMock.Setup(x => x.GetByIdAsync(slotId)).ReturnsAsync(slot);
            _slotRepoMock.Setup(x => x.UpdateStatusAtomicAsync(slotId, SlotStatus.Open, SlotStatus.Reserved)).ReturnsAsync(true);

            var request = new CreateReservationRequest { Nic = nic, SlotId = slotId };
            
            var response = await _service.CreateAsync(request);

            response.Should().NotBeNull();
            response.SlotId.Should().Be(slotId);
        }

        [Fact]
        public async Task Test1_SevenDayReservationRule_Invalid_Rejected()
        {
            // Verify: reservation beyond 7 days -> rejected
            var now = DateTimeOffset.UtcNow;
            var slotId = "slot1";
            var nic = "123456789V";

            var prosumer = new Prosumer { Nic = nic, IsActive = true };
            _prosumerRepoMock.Setup(x => x.GetByNicAsync(nic)).ReturnsAsync(prosumer);

            // Slot starts in 8 days (beyond 7 days)
            var slot = new EnergyBookingSlot { Id = slotId, Status = SlotStatus.Open, SlotStart = now.AddDays(8) };
            _slotRepoMock.Setup(x => x.GetByIdAsync(slotId)).ReturnsAsync(slot);

            var request = new CreateReservationRequest { Nic = nic, SlotId = slotId };

            Func<Task> act = async () => await _service.CreateAsync(request);

            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("Reservations cannot be made more than 7 days in advance.");
        }

        [Fact]
        public async Task Test2_TwelveHourRule_Update_Valid_Allowed()
        {
            // Verify: slot starts more than 12 hours from now -> allowed
            var now = DateTimeOffset.UtcNow;
            var reservationId = "res1";
            var oldSlotId = "slot1";
            var newSlotId = "slot2";

            var reservation = new EnergyReservation { Id = reservationId, SlotId = oldSlotId };
            _reservationRepoMock.Setup(x => x.GetByIdAsync(reservationId)).ReturnsAsync(reservation);

            // Old slot starts in 13 hours
            var oldSlot = new EnergyBookingSlot { Id = oldSlotId, SlotStart = now.AddHours(13) };
            _slotRepoMock.Setup(x => x.GetByIdAsync(oldSlotId)).ReturnsAsync(oldSlot);

            // New slot is valid
            var newSlot = new EnergyBookingSlot { Id = newSlotId, Status = SlotStatus.Open, SlotStart = now.AddDays(1) };
            _slotRepoMock.Setup(x => x.GetByIdAsync(newSlotId)).ReturnsAsync(newSlot);
            _slotRepoMock.Setup(x => x.UpdateStatusAtomicAsync(newSlotId, SlotStatus.Open, SlotStatus.Reserved)).ReturnsAsync(true);

            var request = new UpdateReservationRequest { SlotId = newSlotId };

            var response = await _service.UpdateAsync(reservationId, request);

            response.Should().NotBeNull();
            response.SlotId.Should().Be(newSlotId);
        }

        [Fact]
        public async Task Test2_TwelveHourRule_Update_Invalid_Rejected()
        {
            // Verify: slot starts less than 12 hours from now -> rejected
            var now = DateTimeOffset.UtcNow;
            var reservationId = "res1";
            var oldSlotId = "slot1";

            var reservation = new EnergyReservation { Id = reservationId, SlotId = oldSlotId };
            _reservationRepoMock.Setup(x => x.GetByIdAsync(reservationId)).ReturnsAsync(reservation);

            // Old slot starts in 10 hours (less than 12)
            var oldSlot = new EnergyBookingSlot { Id = oldSlotId, SlotStart = now.AddHours(10) };
            _slotRepoMock.Setup(x => x.GetByIdAsync(oldSlotId)).ReturnsAsync(oldSlot);

            var request = new UpdateReservationRequest { SlotId = "any" };

            Func<Task> act = async () => await _service.UpdateAsync(reservationId, request);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Reservations can only be updated with at least 12 hours notice.");
        }
        
        [Fact]
        public async Task Test2_TwelveHourRule_Cancel_Invalid_Rejected()
        {
            var now = DateTimeOffset.UtcNow;
            var reservationId = "res1";
            var slotId = "slot1";

            var reservation = new EnergyReservation { Id = reservationId, SlotId = slotId, Status = ReservationStatus.Pending };
            _reservationRepoMock.Setup(x => x.GetByIdAsync(reservationId)).ReturnsAsync(reservation);

            // Slot starts in 10 hours (less than 12)
            var slot = new EnergyBookingSlot { Id = slotId, SlotStart = now.AddHours(10) };
            _slotRepoMock.Setup(x => x.GetByIdAsync(slotId)).ReturnsAsync(slot);

            Func<Task> act = async () => await _service.DeleteAsync(reservationId);

            await act.Should().ThrowAsync<Exception>()
                .WithMessage("Reservations can only be cancelled with at least 12 hours notice.");
        }
    }
}
