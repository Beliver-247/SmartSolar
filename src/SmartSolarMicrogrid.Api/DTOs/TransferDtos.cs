/*
 * Purpose: Transfer and QR DTOs.
 * Author: Antigravity
 * Date: 2026-09-22
 */
namespace SmartSolarMicrogrid.Api.DTOs
{
    public class QrResponse
    {
        public string QrCode { get; set; } = string.Empty;
    }

    public class TransferVerifyRequest
    {
        public string QrPayload { get; set; } = string.Empty;
    }

    public class TransferVerifyResponse
    {
        public bool IsValid { get; set; }
        public string Message { get; set; } = string.Empty;
        public ReservationResponse? ReservationDetails { get; set; }
    }
}
