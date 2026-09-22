/*
 * Purpose: Prosumer DTOs.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using System;

namespace SmartSolarMicrogrid.Api.DTOs
{
    public class ProsumerResponse
    {
        public string Nic { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
    }

    public class UpdateProsumerRequest
    {
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
