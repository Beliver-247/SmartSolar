/*
 * Purpose: Validation rules for Station DTOs.
 * Author: Antigravity
 * Date: 2026-09-22
 */
using FluentValidation;
using SmartSolarMicrogrid.Api.DTOs;

namespace SmartSolarMicrogrid.Api.Validators
{
    public class CreateStationRequestValidator : AbstractValidator<CreateStationRequest>
    {
        public CreateStationRequestValidator()
        {
            RuleFor(x => x.StationName).NotEmpty().WithMessage("Station Name is required.");
            RuleFor(x => x.CapacityKwh).GreaterThan(0).WithMessage("Capacity must be positive.");
            RuleFor(x => x.BatterySlots).GreaterThan(0).WithMessage("Battery Slots must be positive.");
            RuleFor(x => x.GpsLocation).NotNull().WithMessage("GPS Location is required.");
            RuleFor(x => x.GpsLocation.Lat).InclusiveBetween(-90, 90).WithMessage("Invalid Latitude.");
            RuleFor(x => x.GpsLocation.Lng).InclusiveBetween(-180, 180).WithMessage("Invalid Longitude.");
        }
    }

    public class UpdateStationRequestValidator : AbstractValidator<UpdateStationRequest>
    {
        public UpdateStationRequestValidator()
        {
            RuleFor(x => x.StationName).NotEmpty().WithMessage("Station Name is required.");
            RuleFor(x => x.CapacityKwh).GreaterThan(0).WithMessage("Capacity must be positive.");
            RuleFor(x => x.BatterySlots).GreaterThan(0).WithMessage("Battery Slots must be positive.");
            RuleFor(x => x.GpsLocation).NotNull().WithMessage("GPS Location is required.");
            RuleFor(x => x.GpsLocation.Lat).InclusiveBetween(-90, 90).WithMessage("Invalid Latitude.");
            RuleFor(x => x.GpsLocation.Lng).InclusiveBetween(-180, 180).WithMessage("Invalid Longitude.");
        }
    }
}
