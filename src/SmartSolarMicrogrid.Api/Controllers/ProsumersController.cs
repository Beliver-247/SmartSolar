/*
 * Purpose: Prosumer management endpoints.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSolarMicrogrid.Api.DTOs;
using SmartSolarMicrogrid.Api.Models;
using SmartSolarMicrogrid.Api.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartSolarMicrogrid.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProsumersController : ControllerBase
    {
        private readonly IProsumerService _prosumerService;

        public ProsumersController(IProsumerService prosumerService)
        {
            _prosumerService = prosumerService;
        }

        [HttpGet]
        [Authorize(Roles = Role.Backoffice)]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            // Gets paginated list of prosumers
            var prosumers = await _prosumerService.GetAllAsync(page, pageSize);
            return Ok(prosumers);
        }

        [HttpGet("{nic}")]
        [Authorize(Roles = Role.Backoffice + "," + Role.Prosumer)]
        public async Task<IActionResult> GetByNic(string nic)
        {
            // Validates ownership for prosumer role
            if (User.IsInRole(Role.Prosumer) && User.FindFirstValue(ClaimTypes.NameIdentifier) != nic)
            {
                return Forbid();
            }

            var prosumer = await _prosumerService.GetByNicAsync(nic);
            return Ok(prosumer);
        }

        [HttpPut("{nic}")]
        [Authorize(Roles = Role.Backoffice + "," + Role.Prosumer)]
        public async Task<IActionResult> Update(string nic, [FromBody] UpdateProsumerRequest request)
        {
            // Validates ownership for prosumer role
            if (User.IsInRole(Role.Prosumer) && User.FindFirstValue(ClaimTypes.NameIdentifier) != nic)
            {
                return Forbid();
            }

            var prosumer = await _prosumerService.UpdateAsync(nic, request);
            return Ok(prosumer);
        }

        [HttpPut("{nic}/deactivate")]
        [Authorize(Roles = Role.Backoffice + "," + Role.Prosumer)]
        public async Task<IActionResult> Deactivate(string nic)
        {
            // Validates ownership for prosumer role
            if (User.IsInRole(Role.Prosumer) && User.FindFirstValue(ClaimTypes.NameIdentifier) != nic)
            {
                return Forbid();
            }

            await _prosumerService.DeactivateAsync(nic);
            return NoContent();
        }

        [HttpPut("{nic}/reactivate")]
        [Authorize(Roles = Role.Backoffice)]
        public async Task<IActionResult> Reactivate(string nic)
        {
            // Only Backoffice can reactivate
            await _prosumerService.ReactivateAsync(nic);
            return NoContent();
        }
    }
}
