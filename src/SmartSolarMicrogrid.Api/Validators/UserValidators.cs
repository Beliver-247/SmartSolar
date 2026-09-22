/*
 * Purpose: Validation rules for User DTOs.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using FluentValidation;
using SmartSolarMicrogrid.Api.DTOs;
using SmartSolarMicrogrid.Api.Models;

namespace SmartSolarMicrogrid.Api.Validators
{
    public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
    {
        public CreateUserRequestValidator()
        {
            RuleFor(x => x.Username).NotEmpty().WithMessage("Username is required.");
            RuleFor(x => x.Password).NotEmpty().MinimumLength(6).WithMessage("Password must be at least 6 characters.");
            RuleFor(x => x.Role)
                .Must(role => role == Role.Backoffice || role == Role.GridOperator)
                .WithMessage("Role must be 'Backoffice' or 'GridOperator'.");
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full Name is required.");
        }
    }

    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator()
        {
            RuleFor(x => x.Role)
                .Must(role => role == Role.Backoffice || role == Role.GridOperator)
                .WithMessage("Role must be 'Backoffice' or 'GridOperator'.");
            RuleFor(x => x.FullName).NotEmpty().WithMessage("Full Name is required.");
        }
    }
}
