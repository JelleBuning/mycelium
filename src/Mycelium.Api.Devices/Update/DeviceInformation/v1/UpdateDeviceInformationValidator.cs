using FluentValidation;

namespace Mycelium.Api.Devices.Update.DeviceInformation.v1;

public sealed class UpdateDeviceInformationValidator : AbstractValidator<UpdateDeviceInformationCommand>
{
    public UpdateDeviceInformationValidator()
    {
        RuleFor(x => x.DeviceId)
            .GreaterThan(0).WithMessage("DeviceId must be greater than 0");

        RuleFor(x => x.DeviceInfo)
            .NotNull().WithMessage("DeviceInfo is required");
    }
}
