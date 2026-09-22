/*
 * Purpose: Validation rules for Transfer DTOs.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using FluentValidation;
using SmartSolarMicrogrid.Api.DTOs;

namespace SmartSolarMicrogrid.Api.Validators
{
    public class TransferVerifyRequestValidator : AbstractValidator<TransferVerifyRequest>
    {
        public TransferVerifyRequestValidator()
        {
            RuleFor(x => x.QrPayload).NotEmpty().WithMessage("QR Payload is required.");
        }
    }
}
