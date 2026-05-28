using BookingSystem.BuildingBlocks.Application;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.UseCases.Abstractions;

namespace BookingSystem.Modules.Reservations.UseCases.CancelReservation;

internal sealed class CancelReservationHandler(
    IReservationRepository repository) : ICommandHandler<CancelReservationCommand>
{
    public async Task Handle(CancelReservationCommand command, CancellationToken cancellationToken)
    {
        var reservation = await repository.GetById(ReservationId.From(command.ReservationId), cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation {command.ReservationId} not found.");

        reservation.Cancel();
        await repository.Update(reservation, cancellationToken);
    }
}
