/*
 * Purpose: Unit tests for Prosumer rules (NIC uniqueness).
 * Author: Antigravity
 * Date: 2026-09-22
 */
using FluentAssertions;
using Moq;
using SmartSolarMicrogrid.Api.DTOs;
using SmartSolarMicrogrid.Api.Middleware;
using SmartSolarMicrogrid.Api.Models;
using SmartSolarMicrogrid.Api.Repositories;
using SmartSolarMicrogrid.Api.Services;
using System;
using System.Threading.Tasks;
using Xunit;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SmartSolarMicrogrid.Api.Configuration;

namespace SmartSolarMicrogrid.Tests.Services
{
    public class ProsumerAuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;
        private readonly Mock<IProsumerRepository> _prosumerRepoMock;
        private readonly Mock<IOptions<JwtSettings>> _jwtSettingsMock;
        private readonly Mock<ILogger<AuthService>> _loggerMock;
        private readonly AuthService _service;

        public ProsumerAuthServiceTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
            _prosumerRepoMock = new Mock<IProsumerRepository>();
            _jwtSettingsMock = new Mock<IOptions<JwtSettings>>();
            _jwtSettingsMock.Setup(x => x.Value).Returns(new JwtSettings { Key = "SOME_SUPER_SECRET_KEY_FOR_TESTS_1234567890", ExpirationMinutes = 60, Issuer = "Test", Audience = "Test" });
            _loggerMock = new Mock<ILogger<AuthService>>();

            _service = new AuthService(_userRepoMock.Object, _prosumerRepoMock.Object, _jwtSettingsMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Test5_NICUniqueness_ExistingNIC_Rejected()
        {
            var nic = "123456789V";
            var existingProsumer = new Prosumer { Nic = nic };
            _prosumerRepoMock.Setup(x => x.GetByNicAsync(nic)).ReturnsAsync(existingProsumer);

            var request = new RegisterProsumerRequest { Nic = nic, Password = "password" };

            Func<Task> act = async () => await _service.RegisterProsumerAsync(request);

            await act.Should().ThrowAsync<BusinessRuleException>()
                .WithMessage("NIC already exists.");
        }

        [Fact]
        public async Task Test5_NICUniqueness_NewNIC_Succeeds()
        {
            var nic = "123456789V";
            _prosumerRepoMock.Setup(x => x.GetByNicAsync(nic)).ReturnsAsync((Prosumer)null!);

            var request = new RegisterProsumerRequest { Nic = nic, Password = "password" };

            var response = await _service.RegisterProsumerAsync(request);

            response.Should().NotBeNull();
            response.Nic.Should().Be(nic);
        }
    }
}
