/*
 * Purpose: Validation rules for Prosumer DTOs.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using FluentValidation;
using SmartSolarMicrogrid.Api.DTOs;

namespace SmartSolarMicrogrid.Api.Validators
{
    public class UpdateProsumerRequestValidator : AbstractValidator<UpdateProsumerRequest>
    {
        public UpdateProsumerRequestValidator()
        {
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full Name is required.");
            RuleFor(x => x.Phone).NotEmpty().Matches(@"^\d{10}$").WithMessage("Mobile number must be exactly 10 numbers.");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required.");
        }
    }
}
