/*
 * Purpose: Reservation DTOs.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using System;

namespace SmartSolarMicrogrid.Api.DTOs
{
    public class CreateReservationRequest
    {
        public string Nic { get; set; } = string.Empty;
        public string SlotId { get; set; } = string.Empty;
    }

    public class UpdateReservationRequest
    {
        public string SlotId { get; set; } = string.Empty;
    }

    public class ReservationResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Nic { get; set; } = string.Empty;
        public string SlotId { get; set; } = string.Empty;
        public string StationId { get; set; } = string.Empty;
        public DateTimeOffset ReservedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTimeOffset LastModified { get; set; }
    }
}
