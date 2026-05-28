namespace BookingSystem.Modules.Reservations.UseCases.CreateReservation;

internal sealed class CreateReservationValidator
{
    public string? Validate(CreateReservationCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.GuestName))
            return "Guest name is required.";

        if (command.Start >= command.End)
            return "Start must be before end.";

        return null;
    }
}
