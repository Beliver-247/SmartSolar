/*
 * Purpose: Validation rules for Reservation DTOs.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using FluentValidation;
using SmartSolarMicrogrid.Api.DTOs;

namespace SmartSolarMicrogrid.Api.Validators
{
    public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
    {
        public CreateReservationRequestValidator()
        {
            RuleFor(x => x.Nic).NotEmpty().Matches(@"^(\d{12}|\d{9}[vV])$").WithMessage("NIC must be either 12 numbers or 9 numbers followed by 'v' or 'V'.");
            RuleFor(x => x.SlotId).NotEmpty().WithMessage("Slot ID is required.");
        }
    }

    public class UpdateReservationRequestValidator : AbstractValidator<UpdateReservationRequest>
    {
        public UpdateReservationRequestValidator()
        {
            RuleFor(x => x.SlotId).NotEmpty().WithMessage("Slot ID is required.");
        }
    }
}
