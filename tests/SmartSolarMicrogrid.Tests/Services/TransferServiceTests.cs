/*
 * Purpose: Unit tests for Transfer rules (QR generation).
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
    public class TransferServiceTests
    {
        private readonly Mock<IEnergyReservationRepository> _reservationRepoMock;
        private readonly TransferService _service;

        public TransferServiceTests()
        {
            _reservationRepoMock = new Mock<IEnergyReservationRepository>();
            _service = new TransferService(_reservationRepoMock.Object);
        }

        [Fact]
        public async Task Test6_QRGeneration_ApprovedStatus_Succeeds()
        {
            var id = "res1";
            var nic = "123456789V";
            var reservation = new EnergyReservation { Id = id, Nic = nic, Status = ReservationStatus.Approved };

            _reservationRepoMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(reservation);

            var response = await _service.GenerateQrAsync(id, nic);

            response.Should().NotBeNull();
            response.QrCode.Should().StartWith("data:image/png;base64,");
        }

        [Theory]
        [InlineData(ReservationStatus.Pending)]
        [InlineData(ReservationStatus.Cancelled)]
        [InlineData(ReservationStatus.Completed)]
        public async Task Test6_QRGeneration_OtherStatus_Rejected(string status)
        {
            var id = "res1";
            var nic = "123456789V";
            var reservation = new EnergyReservation { Id = id, Nic = nic, Status = status };

            _reservationRepoMock.Setup(x => x.GetByIdAsync(id)).ReturnsAsync(reservation);

            Func<Task> act = async () => await _service.GenerateQrAsync(id, nic);

            await act.Should().ThrowAsync<BusinessRuleException>()
                .WithMessage("QR code is only available for Approved reservations.");
        }
    }
}
