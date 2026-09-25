/*
 * Purpose: Transfer and QR management.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using QRCoder;
using SmartSolarMicrogrid.Api.DTOs;
using SmartSolarMicrogrid.Api.Middleware;
using SmartSolarMicrogrid.Api.Models;
using SmartSolarMicrogrid.Api.Repositories;
using System;
using System.Text;
using System.Threading.Tasks;

namespace SmartSolarMicrogrid.Api.Services
{
    public interface ITransferService
    {
        Task<QrResponse> GenerateQrAsync(string reservationId, string nic);
        Task<TransferVerifyResponse> VerifyTransferAsync(TransferVerifyRequest request);
        Task CompleteTransferAsync(string reservationId);
    }

    public class TransferService : ITransferService
    {
        private readonly IEnergyReservationRepository _reservationRepository;

        public TransferService(IEnergyReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        public async Task<QrResponse> GenerateQrAsync(string reservationId, string nic)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            if (reservation == null || reservation.Nic != nic)
                throw new NotFoundException("Reservation not found.");

            if (reservation.Status != ReservationStatus.Approved)
                throw new BusinessRuleException("QR code is only available for Approved reservations.");

            // Generate payload: e.g., "SmartSolarTransfer|{ReservationId}|{Nic}"
            string payload = $"SmartSolarTransfer|{reservation.Id}|{reservation.Nic}";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            
            byte[] qrCodeImage = qrCode.GetGraphic(20);
            string base64Image = Convert.ToBase64String(qrCodeImage);

            // Persist payload or hash to reservation? The spec says the server must verify the transaction.
            // Storing the payload or relying on the ID in the payload is fine since we re-fetch from DB.
            if (string.IsNullOrEmpty(reservation.QrCode))
            {
                reservation.QrCode = payload;
                await _reservationRepository.UpdateAsync(reservation);
            }

            return new QrResponse { QrCode = $"data:image/png;base64,{base64Image}" };
        }

        public async Task<TransferVerifyResponse> VerifyTransferAsync(TransferVerifyRequest request)
        {
            // Simple parsing for this implementation
            var parts = request.QrPayload.Split('|');
            if (parts.Length != 3 || parts[0] != "SmartSolarTransfer")
            {
                return new TransferVerifyResponse { IsValid = false, Message = "Invalid QR format." };
            }

            var reservationId = parts[1];
            var nic = parts[2];

            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            if (reservation == null || reservation.Nic != nic)
                return new TransferVerifyResponse { IsValid = false, Message = "Reservation not found or NIC mismatch." };

            if (reservation.Status == ReservationStatus.Completed)
                return new TransferVerifyResponse { IsValid = false, Message = "Transaction has already been completed." };
            
            if (reservation.Status == ReservationStatus.Cancelled)
                return new TransferVerifyResponse { IsValid = false, Message = "Transaction is cancelled." };

            if (reservation.Status != ReservationStatus.Approved)
                return new TransferVerifyResponse { IsValid = false, Message = "Reservation is not approved." };

            return new TransferVerifyResponse
            {
                IsValid = true,
                Message = "QR is valid and ready for completion.",
                ReservationDetails = new ReservationResponse
                {
                    Id = reservation.Id!,
                    Nic = reservation.Nic,
                    StationId = reservation.StationId,
                    BookingDate = reservation.BookingDate,
                    TimeSlot = reservation.TimeSlot,
                    ReservedAt = reservation.ReservedAt,
                    Status = reservation.Status,
                    LastModified = reservation.LastModified
                }
            };
        }

        public async Task CompleteTransferAsync(string reservationId)
        {
            var reservation = await _reservationRepository.GetByIdAsync(reservationId);
            if (reservation == null)
                throw new NotFoundException("Reservation not found.");

            if (reservation.Status == ReservationStatus.Completed)
                throw new BusinessRuleException("Transaction has already been completed."); // Rule 24: Don't allow Completed -> Completed

            if (reservation.Status != ReservationStatus.Approved)
                throw new BusinessRuleException("Only approved reservations can be completed.");

            reservation.Status = ReservationStatus.Completed;
            reservation.LastModified = DateTimeOffset.UtcNow;

            await _reservationRepository.UpdateAsync(reservation);
        }
    }
}
