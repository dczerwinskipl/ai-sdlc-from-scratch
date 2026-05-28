namespace BookingSystem.Modules.Reservations.UseCases.ChangeReservationPeriod;

internal sealed class ChangeReservationPeriodValidator
{
    public string? Validate(ChangeReservationPeriodCommand command)
    {
        if (command.Start >= command.End)
            return "Start must be before end.";

        return null;
    }
}
