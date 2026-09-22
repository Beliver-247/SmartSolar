/*
 * Purpose: Booking Slot management endpoints.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSolarMicrogrid.Api.DTOs;
using SmartSolarMicrogrid.Api.Models;
using SmartSolarMicrogrid.Api.Services;
using System.Threading.Tasks;

namespace SmartSolarMicrogrid.Api.Controllers
{
    [ApiController]
    [Route("api/stations/{stationId}/slots")]
    public class BookingSlotsController : ControllerBase
    {
        private readonly IBookingSlotService _slotService;

        public BookingSlotsController(IBookingSlotService slotService)
        {
            _slotService = slotService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetByStationId(string stationId)
        {
            var slots = await _slotService.GetByStationIdAsync(stationId);
            return Ok(slots);
        }

        [HttpPost]
        [Authorize(Roles = Role.Backoffice)]
        public async Task<IActionResult> Create(string stationId, [FromBody] CreateSlotRequest request)
        {
            var slot = await _slotService.CreateAsync(stationId, request);
            return CreatedAtAction(nameof(GetByStationId), new { stationId = stationId }, slot);
        }
    }
}
