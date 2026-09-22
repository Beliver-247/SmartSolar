/*
 * Purpose: Transfer management endpoints.
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
    [Authorize(Roles = Role.GridOperator)]
    public class TransfersController : ControllerBase
    {
        private readonly ITransferService _transferService;

        public TransfersController(ITransferService transferService)
        {
            _transferService = transferService;
        }

        [HttpPost("verify")]
        public async Task<IActionResult> Verify([FromBody] TransferVerifyRequest request)
        {
            var result = await _transferService.VerifyTransferAsync(request);
            if (!result.IsValid)
            {
                return BadRequest(result);
            }
            return Ok(result);
        }

        [HttpPut("{reservationId}/complete")]
        public async Task<IActionResult> Complete(string reservationId)
        {
            await _transferService.CompleteTransferAsync(reservationId);
            return NoContent();
        }
    }
}
