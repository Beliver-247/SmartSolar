/*
 * Purpose: Web User management endpoints.
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
    [Authorize(Roles = Role.Backoffice)]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            // Gets paginated list of users
            var users = await _userService.GetAllAsync(page, pageSize);
            return Ok(users);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserRequest request)
        {
            // Creates a new user
            var user = await _userService.CreateAsync(request);
            return CreatedAtAction(nameof(GetAll), new { id = user.Id }, user);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateUserRequest request)
        {
            // Updates a user
            var user = await _userService.UpdateAsync(id, request);
            return Ok(user);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            // Deletes a user
            await _userService.DeleteAsync(id);
            return NoContent();
        }
    }
}
