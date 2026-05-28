using BookingSystem.BuildingBlocks.Application;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.UseCases.Abstractions;

namespace BookingSystem.Modules.Reservations.UseCases.ConfirmReservation;

internal sealed class ConfirmReservationHandler(
    IReservationRepository repository) : ICommandHandler<ConfirmReservationCommand>
{
    public async Task Handle(ConfirmReservationCommand command, CancellationToken cancellationToken)
    {
        var reservation = await repository.GetById(ReservationId.From(command.ReservationId), cancellationToken)
            ?? throw new KeyNotFoundException($"Reservation {command.ReservationId} not found.");

        reservation.Confirm();
        await repository.Update(reservation, cancellationToken);
    }
}
