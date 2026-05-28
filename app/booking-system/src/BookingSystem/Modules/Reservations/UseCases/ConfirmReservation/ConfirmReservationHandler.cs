using BookingSystem.BuildingBlocks.Application;
using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.UseCases.Abstractions;

namespace BookingSystem.Modules.Reservations.UseCases.ConfirmReservation;

internal sealed class ConfirmReservationHandler(
    IReservationRepository repository) : ICommandHandler<ConfirmReservationCommand>
{
    public async Task<Result> Handle(ConfirmReservationCommand command, CancellationToken cancellationToken)
    {
        var reservation = await repository.GetById(ReservationId.From(command.ReservationId), cancellationToken);
        if (reservation is null)
            return new NotFoundError($"Reservation {command.ReservationId} not found.");

        try
        {
            reservation.Confirm();
            await repository.Update(reservation, cancellationToken);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return new ConflictError(ex.Message);
        }
    }
}
