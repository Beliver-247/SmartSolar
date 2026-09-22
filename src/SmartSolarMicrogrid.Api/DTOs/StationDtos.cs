/*
 * Purpose: Station DTOs.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using SmartSolarMicrogrid.Api.Models;
using System.Collections.Generic;

namespace SmartSolarMicrogrid.Api.DTOs
{
    public class CreateStationRequest
    {
        public string StationName { get; set; } = string.Empty;
        public GpsLocation GpsLocation { get; set; } = new GpsLocation();
        public double CapacityKwh { get; set; }
        public int BatterySlots { get; set; }
        public List<string> Schedule { get; set; } = new List<string>();
    }

    public class UpdateStationRequest
    {
        public string StationName { get; set; } = string.Empty;
        public GpsLocation GpsLocation { get; set; } = new GpsLocation();
        public double CapacityKwh { get; set; }
        public int BatterySlots { get; set; }
        public List<string> Schedule { get; set; } = new List<string>();
    }

    public class StationResponse
    {
        public string Id { get; set; } = string.Empty;
        public string StationName { get; set; } = string.Empty;
        public GpsLocation GpsLocation { get; set; } = new GpsLocation();
        public double CapacityKwh { get; set; }
        public int BatterySlots { get; set; }
        public List<string> Schedule { get; set; } = new List<string>();
        public bool IsActive { get; set; }
    }
}
