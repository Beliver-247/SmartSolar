/*
 * Purpose: Prosumer management.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using SmartSolarMicrogrid.Api.DTOs;
using SmartSolarMicrogrid.Api.Middleware;
using SmartSolarMicrogrid.Api.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSolarMicrogrid.Api.Services
{
    public interface IProsumerService
    {
        Task<IEnumerable<ProsumerResponse>> GetAllAsync(int page, int pageSize);
        Task<ProsumerResponse> GetByNicAsync(string nic);
        Task<ProsumerResponse> UpdateAsync(string nic, UpdateProsumerRequest request);
        Task DeactivateAsync(string nic);
        Task ReactivateAsync(string nic);
    }

    public class ProsumerService : IProsumerService
    {
        private readonly IProsumerRepository _prosumerRepository;

        public ProsumerService(IProsumerRepository prosumerRepository)
        {
            _prosumerRepository = prosumerRepository;
        }

        public async Task<IEnumerable<ProsumerResponse>> GetAllAsync(int page, int pageSize)
        {
            // Paginated list of prosumers
            var skip = (page - 1) * pageSize;
            var prosumers = await _prosumerRepository.GetAllAsync(skip, pageSize);
            return prosumers.Select(p => new ProsumerResponse
            {
                Nic = p.Nic,
                FullName = p.FullName,
                Phone = p.Phone,
                Address = p.Address,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            });
        }

        public async Task<ProsumerResponse> GetByNicAsync(string nic)
        {
            // Gets prosumer by NIC
            var p = await _prosumerRepository.GetByNicAsync(nic);
            if (p == null)
                throw new NotFoundException("Prosumer not found.");

            return new ProsumerResponse
            {
                Nic = p.Nic,
                FullName = p.FullName,
                Phone = p.Phone,
                Address = p.Address,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            };
        }

        public async Task<ProsumerResponse> UpdateAsync(string nic, UpdateProsumerRequest request)
        {
            // Updates prosumer profile
            var p = await _prosumerRepository.GetByNicAsync(nic);
            if (p == null)
                throw new NotFoundException("Prosumer not found.");

            p.FullName = request.FullName;
            p.Phone = request.Phone;
            p.Address = request.Address;

            await _prosumerRepository.UpdateAsync(p);

            return new ProsumerResponse
            {
                Nic = p.Nic,
                FullName = p.FullName,
                Phone = p.Phone,
                Address = p.Address,
                IsActive = p.IsActive,
                CreatedAt = p.CreatedAt
            };
        }

        public async Task DeactivateAsync(string nic)
        {
            // Deactivates a prosumer
            var p = await _prosumerRepository.GetByNicAsync(nic);
            if (p == null)
                throw new NotFoundException("Prosumer not found.");

            p.IsActive = false;
            await _prosumerRepository.UpdateAsync(p);
        }

        public async Task ReactivateAsync(string nic)
        {
            // Reactivates a prosumer. Only Backoffice can call this (enforced at controller). Rule 4.
            var p = await _prosumerRepository.GetByNicAsync(nic);
            if (p == null)
                throw new NotFoundException("Prosumer not found.");

            p.IsActive = true;
            await _prosumerRepository.UpdateAsync(p);
        }
    }
}
