/*
 * Purpose: Solar station information node.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace SmartSolarMicrogrid.Api.Models
{
    public class SolarStationInfo
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string StationName { get; set; } = string.Empty;
        public GpsLocation GpsLocation { get; set; } = new GpsLocation();
        public double CapacityKwh { get; set; }
        public int BatterySlots { get; set; }
        public List<string> Schedule { get; set; } = new List<string>();
        public bool IsActive { get; set; } = true;
    }
}
