/*
 * Purpose: Authentication service logic.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SmartSolarMicrogrid.Api.Configuration;
using SmartSolarMicrogrid.Api.DTOs;
using SmartSolarMicrogrid.Api.Middleware;
using SmartSolarMicrogrid.Api.Models;
using SmartSolarMicrogrid.Api.Repositories;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolarMicrogrid.Api.Services
{
    public interface IAuthService
    {
        Task<LoginResponse> LoginAsync(LoginRequest request);
        Task<ProsumerResponse> RegisterProsumerAsync(RegisterProsumerRequest request);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IProsumerRepository _prosumerRepository;
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
            IUserRepository userRepository,
            IProsumerRepository prosumerRepository,
            IOptions<JwtSettings> jwtSettings,
            ILogger<AuthService> logger)
        {
            _userRepository = userRepository;
            _prosumerRepository = prosumerRepository;
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public async Task<LoginResponse> LoginAsync(LoginRequest request)
        {
            // Login logic for Prosumers, Backoffice, and GridOperator
            _logger.LogInformation("Login attempt for {UsernameOrNic}", request.UsernameOrNic);

            string role = string.Empty;
            string identifier = string.Empty;

            // Try user first
            var user = await _userRepository.GetByUsernameAsync(request.UsernameOrNic);
            if (user != null)
            {
                if (!user.IsActive)
                    throw new UnauthorizedAccessException("Account is deactivated.");

                if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                    throw new UnauthorizedAccessException("Invalid credentials.");

                role = user.Role;
                identifier = user.Username;
            }
            else
            {
                // Try prosumer
                var prosumer = await _prosumerRepository.GetByNicAsync(request.UsernameOrNic);
                if (prosumer != null)
                {
                    if (!prosumer.IsActive)
                        throw new UnauthorizedAccessException("Account is deactivated.");

                    if (!BCrypt.Net.BCrypt.Verify(request.Password, prosumer.PasswordHash))
                        throw new UnauthorizedAccessException("Invalid credentials.");

                    role = Role.Prosumer;
                    identifier = prosumer.Nic;
                }
                else
                {
                    throw new UnauthorizedAccessException("Invalid credentials.");
                }
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.Key);
            var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, identifier),
                new Claim(ClaimTypes.Role, role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            _logger.LogInformation("Login successful for {UsernameOrNic} with role {Role}", identifier, role);

            return new LoginResponse
            {
                Token = tokenHandler.WriteToken(token),
                Role = role,
                UsernameOrNic = identifier,
                ExpiresAt = new DateTimeOffset(expires)
            };
        }

        public async Task<ProsumerResponse> RegisterProsumerAsync(RegisterProsumerRequest request)
        {
            // Self-registration for Prosumers
            var existing = await _prosumerRepository.GetByNicAsync(request.Nic);
            if (existing != null)
                throw new BusinessRuleException("NIC already exists.");

            var prosumer = new Prosumer
            {
                Nic = request.Nic,
                FullName = request.FullName,
                Phone = request.Phone,
                Address = request.Address,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _prosumerRepository.CreateAsync(prosumer);
            _logger.LogInformation("Prosumer registered with NIC: {Nic}", request.Nic);

            return new ProsumerResponse
            {
                Nic = prosumer.Nic,
                FullName = prosumer.FullName,
                Phone = prosumer.Phone,
                Address = prosumer.Address,
                IsActive = prosumer.IsActive,
                CreatedAt = prosumer.CreatedAt
            };
        }
    }
}
