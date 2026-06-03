using BookingSystem.BuildingBlocks.Application;

namespace BookingSystem.Modules.Reservations.UseCases.ChangeReservationPeriod;

internal sealed record ChangeReservationPeriodCommand(
    Guid ReservationId,
    DateTimeOffset Start,
    DateTimeOffset End) : ICommand;
