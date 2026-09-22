/*
 * Purpose: Authentication endpoints.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartSolarMicrogrid.Api.DTOs;
using SmartSolarMicrogrid.Api.Services;
using System.Threading.Tasks;

namespace SmartSolarMicrogrid.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Authenticates user and returns JWT
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterProsumerRequest request)
        {
            // Registers a new prosumer
            var response = await _authService.RegisterProsumerAsync(request);
            return CreatedAtAction(nameof(Register), new { nic = response.Nic }, response);
        }
    }
}
