using BookingSystem.BuildingBlocks.Application;
using BookingSystem.BuildingBlocks.Domain;

namespace BookingSystem.Modules.Reservations.UseCases.CreateReservation;

internal sealed class CreateReservationValidator(IClock clock)
{
    public Error? Validate(CreateReservationCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.GuestName))
            return new ValidationError("Guest name is required.");

        if (command.Start >= command.End)
            return new ValidationError("Start must be before end.");

        if (command.Start < clock.UtcNow)
            return new ValidationError("Start must be in the future.");

        return null;
    }
}
