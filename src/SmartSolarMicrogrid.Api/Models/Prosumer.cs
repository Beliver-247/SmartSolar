/*
 * Purpose: Solar Prosumer model, using NIC as primary key.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SmartSolarMicrogrid.Api.Models
{
    public class Prosumer
    {
        [BsonId]
        public string Nic { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public DateTimeOffset CreatedAt { get; set; }
    }
}
