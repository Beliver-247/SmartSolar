/*
 * Purpose: Validation rules for Slot DTOs.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using FluentValidation;
using SmartSolarMicrogrid.Api.DTOs;

namespace SmartSolarMicrogrid.Api.Validators
{
    public class CreateSlotRequestValidator : AbstractValidator<CreateSlotRequest>
    {
        public CreateSlotRequestValidator()
        {
            RuleFor(x => x.SlotStart).NotEmpty().WithMessage("Slot Start is required.");
            RuleFor(x => x.SlotEnd).NotEmpty().WithMessage("Slot End is required.");
            RuleFor(x => x.SlotEnd).GreaterThan(x => x.SlotStart).WithMessage("Slot End must be after Slot Start.");
        }
    }
}
