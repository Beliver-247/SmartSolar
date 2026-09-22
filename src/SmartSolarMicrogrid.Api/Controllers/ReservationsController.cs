/*
 * Purpose: Reservation management endpoints.
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
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationService _reservationService;

        public ReservationsController(IReservationService reservationService)
        {
            _reservationService = reservationService;
        }

        [HttpGet]
        [Authorize(Roles = Role.Backoffice + "," + Role.GridOperator + "," + Role.Prosumer)]
        public async Task<IActionResult> Get([FromQuery] string? nic, [FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            if (User.IsInRole(Role.Prosumer))
            {
                var ownNic = User.FindFirstValue(ClaimTypes.NameIdentifier);
                // Prosumers can only view their own
                return Ok(await _reservationService.GetByNicAsync(ownNic!, page, pageSize));
            }

            if (!string.IsNullOrEmpty(nic))
            {
                return Ok(await _reservationService.GetByNicAsync(nic, page, pageSize));
            }

            return Ok(await _reservationService.GetAllAsync(page, pageSize));
        }

        [HttpGet("pending")]
        [Authorize(Roles = Role.Backoffice + "," + Role.GridOperator)]
        public async Task<IActionResult> GetPending([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            return Ok(await _reservationService.GetPendingAsync(page, pageSize));
        }

        [HttpGet("approved-future")]
        [Authorize(Roles = Role.Backoffice + "," + Role.GridOperator)]
        public async Task<IActionResult> GetApprovedFuture([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            return Ok(await _reservationService.GetApprovedFutureAsync(page, pageSize));
        }

        [HttpPost]
        [Authorize(Roles = Role.GridOperator + "," + Role.Prosumer)]
        public async Task<IActionResult> Create([FromBody] CreateReservationRequest request)
        {
            if (User.IsInRole(Role.Prosumer))
            {
                var ownNic = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (request.Nic != ownNic)
                {
                    return Forbid("You can only create reservations for your own account.");
                }
            }

            var reservation = await _reservationService.CreateAsync(request);
            return Ok(reservation);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = Role.GridOperator + "," + Role.Prosumer)]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateReservationRequest request)
        {
            // Validating ownership for prosumer would require reading the reservation first,
            // or letting the service handle it, but the spec says the controller should obtain authenticated user.
            // For simplicity, we can try to fetch it first, or let service do it.
            // But wait, the service doesn't have the user context. Let's do it here.

            var res = await _reservationService.GetByNicAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!, 1, int.MaxValue);
            
            if (User.IsInRole(Role.Prosumer))
            {
                bool ownsReservation = false;
                foreach (var r in res)
                {
                    if (r.Id == id)
                    {
                        ownsReservation = true;
                        break;
                    }
                }

                if (!ownsReservation)
                    return Forbid();
            }

            var updated = await _reservationService.UpdateAsync(id, request);
            return Ok(updated);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = Role.GridOperator + "," + Role.Prosumer)]
        public async Task<IActionResult> Delete(string id)
        {
            if (User.IsInRole(Role.Prosumer))
            {
                var res = await _reservationService.GetByNicAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!, 1, int.MaxValue);
                bool ownsReservation = false;
                foreach (var r in res)
                {
                    if (r.Id == id)
                    {
                        ownsReservation = true;
                        break;
                    }
                }

                if (!ownsReservation)
                    return Forbid();
            }

            await _reservationService.DeleteAsync(id);
            return NoContent();
        }

        [HttpPut("{id}/approve")]
        [Authorize(Roles = Role.GridOperator)] // Or Backoffice based on specific need, spec mentions GridOperator
        public async Task<IActionResult> Approve(string id)
        {
            var approved = await _reservationService.ApproveAsync(id);
            return Ok(approved);
        }

        [HttpGet("{id}/qr")]
        [Authorize(Roles = Role.Prosumer)]
        public async Task<IActionResult> GetQr(string id)
        {
            var nic = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var qr = await _reservationService.GetByNicAsync(nic!, 1, int.MaxValue);
            bool owns = false;
            foreach (var r in qr)
            {
                if (r.Id == id)
                {
                    owns = true;
                    break;
                }
            }

            if (!owns)
                return Forbid();

            var transferService = HttpContext.RequestServices.GetService(typeof(ITransferService)) as ITransferService;
            var qrData = await transferService!.GenerateQrAsync(id, nic!);

            return Ok(qrData);
        }
    }
}
