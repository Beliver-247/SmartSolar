/*
 * Purpose: Auth DTOs.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using System;

namespace SmartSolarMicrogrid.Api.DTOs
{
    public class LoginRequest
    {
        public string UsernameOrNic { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string UsernameOrNic { get; set; } = string.Empty;
        public DateTimeOffset ExpiresAt { get; set; }
    }

    public class RegisterProsumerRequest
    {
        public string Nic { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
