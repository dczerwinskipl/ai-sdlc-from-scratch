using BookingSystem.BuildingBlocks.Application;
using BookingSystem.BuildingBlocks.Domain;

namespace BookingSystem.Modules.Reservations.UseCases.ChangeReservationPeriod;

internal sealed class ChangeReservationPeriodValidator(IClock clock)
{
    public Error? Validate(ChangeReservationPeriodCommand command)
    {
        if (command.Start >= command.End)
            return new ValidationError("Start must be before end.");

        if (command.Start < clock.UtcNow)
            return new ValidationError("Start must be in the future.");

        return null;
    }
}
