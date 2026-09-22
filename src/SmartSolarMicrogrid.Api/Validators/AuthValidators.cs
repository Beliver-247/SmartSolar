/*
 * Purpose: Validation rules for Auth DTOs.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using FluentValidation;
using SmartSolarMicrogrid.Api.DTOs;

namespace SmartSolarMicrogrid.Api.Validators
{
    public class LoginRequestValidator : AbstractValidator<LoginRequest>
    {
        public LoginRequestValidator()
        {
            RuleFor(x => x.UsernameOrNic).NotEmpty().WithMessage("Username or NIC is required.");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
        }
    }

    public class RegisterProsumerRequestValidator : AbstractValidator<RegisterProsumerRequest>
    {
        public RegisterProsumerRequestValidator()
        {
            RuleFor(x => x.Nic).NotEmpty().Matches(@"^(\d{12}|\d{9}[vV])$").WithMessage("NIC must be either 12 numbers or 9 numbers followed by 'v' or 'V'.");
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full Name is required.");
            RuleFor(x => x.Phone).NotEmpty().Matches(@"^\d{10}$").WithMessage("Mobile number must be exactly 10 numbers.");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required.");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Password must be at least 6 characters.");
        }
    }
}
