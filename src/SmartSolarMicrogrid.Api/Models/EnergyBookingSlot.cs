/*
 * Purpose: Energy booking slot for a station.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SmartSolarMicrogrid.Api.Models
{
    public class EnergyBookingSlot
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        public string StationId { get; set; } = string.Empty;

        public DateTimeOffset SlotStart { get; set; }
        public DateTimeOffset SlotEnd { get; set; }
        public string Status { get; set; } = SlotStatus.Open;
    }
}
