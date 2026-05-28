using BookingSystem.BuildingBlocks.Application;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.UseCases.Abstractions;

namespace BookingSystem.Modules.Reservations.UseCases.ChangeReservationPeriod;

internal sealed class ChangeReservationPeriodHandler(
    IReservationRepository repository,
    IReservationAvailabilityChecker availabilityChecker,
    ChangeReservationPeriodValidator validator) : ICommandHandler<ChangeReservationPeriodCommand>
{
    public async Task Handle(ChangeReservationPeriodCommand command, CancellationToken cancellationToken)
    {
        var error = validator.Validate(command);
        if (error is not null)
            throw new ArgumentException(error);

        var reservationId = ReservationId.From(command.ReservationId);
        var reservation = await repository.GetById(reservationId, cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation {command.ReservationId} not found.");

        var newPeriod = ReservationPeriod.Create(command.Start, command.End);
        var available = await availabilityChecker.IsAvailable(
            reservation.RoomId,
            newPeriod,
            reservationId,
            cancellationToken);

        if (!available)
            throw new InvalidOperationException("The selected period overlaps with an existing reservation.");

        reservation.ChangePeriod(newPeriod);
        await repository.Update(reservation, cancellationToken);
    }
}
