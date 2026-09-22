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
            RuleFor(x => x.Nic).NotEmpty().WithMessage("NIC is required.");
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full Name is required.");
            RuleFor(x => x.Phone).NotEmpty().WithMessage("Phone is required.");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Address is required.");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Password must be at least 6 characters.");
        }
    }
}
