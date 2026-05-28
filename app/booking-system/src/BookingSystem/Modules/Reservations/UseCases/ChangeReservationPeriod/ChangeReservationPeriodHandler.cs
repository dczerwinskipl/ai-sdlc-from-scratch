using BookingSystem.BuildingBlocks.Application;
using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.UseCases.Abstractions;

namespace BookingSystem.Modules.Reservations.UseCases.ChangeReservationPeriod;

internal sealed class ChangeReservationPeriodHandler(
    IReservationRepository repository,
    IReservationAvailabilityChecker availabilityChecker,
    ChangeReservationPeriodValidator validator) : ICommandHandler<ChangeReservationPeriodCommand>
{
    public async Task<Result> Handle(ChangeReservationPeriodCommand command, CancellationToken cancellationToken)
    {
        var validationError = validator.Validate(command);
        if (validationError is not null)
            return new ValidationError(validationError);

        var reservationId = ReservationId.From(command.ReservationId);
        var reservation = await repository.GetById(reservationId, cancellationToken);
        if (reservation is null)
            return new NotFoundError($"Reservation {command.ReservationId} not found.");

        try
        {
            var newPeriod = ReservationPeriod.Create(command.Start, command.End);
            var available = await availabilityChecker.IsAvailable(
                reservation.RoomId, newPeriod, reservationId, cancellationToken);

            if (!available)
                return new ConflictError("The selected period overlaps with an existing reservation.");

            reservation.ChangePeriod(newPeriod);
            await repository.Update(reservation, cancellationToken);
            return Result.Success();
        }
        catch (DomainException ex)
        {
            return new ConflictError(ex.Message);
        }
    }
}
