/*
 * Purpose: Slot DTOs.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using System;

namespace SmartSolarMicrogrid.Api.DTOs
{
    public class CreateSlotRequest
    {
        public DateTimeOffset SlotStart { get; set; }
        public DateTimeOffset SlotEnd { get; set; }
    }

    public class SlotResponse
    {
        public string Id { get; set; } = string.Empty;
        public string StationId { get; set; } = string.Empty;
        public DateTimeOffset SlotStart { get; set; }
        public DateTimeOffset SlotEnd { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
