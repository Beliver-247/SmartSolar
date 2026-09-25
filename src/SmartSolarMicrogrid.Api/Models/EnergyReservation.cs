/*
 * Purpose: Energy reservation.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace SmartSolarMicrogrid.Api.Models
{
    public class EnergyReservation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        public string Nic { get; set; } = string.Empty;

        [BsonRepresentation(BsonType.ObjectId)]
        public string? SlotId { get; set; } // Kept for backwards compatibility

        [BsonRepresentation(BsonType.ObjectId)]
        public string StationId { get; set; } = string.Empty;

        public string BookingDate { get; set; } = string.Empty;
        public string TimeSlot { get; set; } = string.Empty;

        public DateTimeOffset ReservedAt { get; set; }
        public string Status { get; set; } = ReservationStatus.Pending;
        public string QrCode { get; set; } = string.Empty;
        public DateTimeOffset LastModified { get; set; }
    }
}
