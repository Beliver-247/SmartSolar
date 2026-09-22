/*
 * Purpose: Web User management.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using SmartSolarMicrogrid.Api.DTOs;
using SmartSolarMicrogrid.Api.Middleware;
using SmartSolarMicrogrid.Api.Models;
using SmartSolarMicrogrid.Api.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSolarMicrogrid.Api.Services
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetAllAsync(int page, int pageSize);
        Task<UserResponse> CreateAsync(CreateUserRequest request);
        Task<UserResponse> UpdateAsync(string id, UpdateUserRequest request);
        Task DeleteAsync(string id);
    }

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserResponse>> GetAllAsync(int page, int pageSize)
        {
            // Paginated listing of users
            var skip = (page - 1) * pageSize;
            var users = await _userRepository.GetAllAsync(skip, pageSize);
            
            return users.Select(u => new UserResponse
            {
                Id = u.Id!,
                Username = u.Username,
                Role = u.Role,
                FullName = u.FullName,
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt
            });
        }

        public async Task<UserResponse> CreateAsync(CreateUserRequest request)
        {
            // Creates a user checking for unique username
            var existing = await _userRepository.GetByUsernameAsync(request.Username);
            if (existing != null)
                throw new BusinessRuleException("Username already exists.");

            var user = new User
            {
                Username = request.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = request.Role,
                FullName = request.FullName,
                IsActive = true,
                CreatedAt = DateTimeOffset.UtcNow
            };

            await _userRepository.CreateAsync(user);

            return new UserResponse
            {
                Id = user.Id!,
                Username = user.Username,
                Role = user.Role,
                FullName = user.FullName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task<UserResponse> UpdateAsync(string id, UpdateUserRequest request)
        {
            // Updates user details
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new NotFoundException("User not found.");

            user.Role = request.Role;
            user.FullName = request.FullName;
            user.IsActive = request.IsActive;

            await _userRepository.UpdateAsync(user);

            return new UserResponse
            {
                Id = user.Id!,
                Username = user.Username,
                Role = user.Role,
                FullName = user.FullName,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
        }

        public async Task DeleteAsync(string id)
        {
            // Deletes a user
            var user = await _userRepository.GetByIdAsync(id);
            if (user == null)
                throw new NotFoundException("User not found.");

            await _userRepository.DeleteAsync(id);
        }
    }
}
