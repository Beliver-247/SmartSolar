/*
 * Purpose: Station management endpoints.
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
    [Route("api/[controller]")]
    public class StationsController : ControllerBase
    {
        private readonly IStationService _stationService;

        public StationsController(IStationService stationService)
        {
            _stationService = stationService;
        }

        [HttpGet]
        [Authorize] // All authenticated users can list
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            var stations = await _stationService.GetAllAsync(page, pageSize);
            return Ok(stations);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetById(string id)
        {
            var station = await _stationService.GetByIdAsync(id);
            return Ok(station);
        }

        [HttpPost]
        [Authorize(Roles = Role.Backoffice)]
        public async Task<IActionResult> Create([FromBody] CreateStationRequest request)
        {
            var station = await _stationService.CreateAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = station.Id }, station);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Role.Backoffice)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateStationRequest request)
        {
            var station = await _stationService.UpdateAsync(id, request);
            return Ok(station);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.Backoffice)]
        public async Task<IActionResult> Delete(string id)
        {
            await _stationService.DeleteAsync(id);
            return NoContent();
        }
    }
}
