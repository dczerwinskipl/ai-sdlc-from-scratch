using BookingSystem.BuildingBlocks.Application;
using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.UseCases.Abstractions;

namespace BookingSystem.Modules.Reservations.UseCases.CancelReservation;

internal sealed class CancelReservationHandler(
    IReservationRepository repository) : ICommandHandler<CancelReservationCommand>
{
    public async Task<Result> Handle(CancelReservationCommand command, CancellationToken cancellationToken)
    {
        var reservation = await repository.GetById(ReservationId.From(command.ReservationId), cancellationToken);
        if (reservation is null)
            return new NotFoundError($"Reservation {command.ReservationId} not found.");

        var result = reservation.Cancel();
        if (result.IsFailure) return result;

        await repository.Update(reservation, cancellationToken);
        return Result.Success();
    }
}
