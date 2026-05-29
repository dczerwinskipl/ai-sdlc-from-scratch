using BookingSystem.BuildingBlocks.Application;
using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.RoomManagement.PublicContracts;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.UseCases.Abstractions;

namespace BookingSystem.Modules.Reservations.UseCases.ChangeReservationPeriod;

internal sealed class ChangeReservationPeriodHandler(
    IReservationRepository repository,
    IRoomReader roomReader,
    ChangeReservationPeriodValidator validator) : ICommandHandler<ChangeReservationPeriodCommand>
{
    public async Task<Result> Handle(ChangeReservationPeriodCommand command, CancellationToken cancellationToken)
    {
        var error = validator.Validate(command);
        if (error is not null)
            return error;

        var reservationId = ReservationId.From(command.ReservationId);
        var reservation = await repository.GetById(reservationId, cancellationToken);
        if (reservation is null)
            return new NotFoundError($"Reservation {command.ReservationId} not found.");

        var room = await roomReader.GetById(reservation.RoomId.Value, cancellationToken);
        if (room is null)
            return new NotFoundError($"Room {reservation.RoomId.Value} not found.");
        if (!room.IsActive)
            return new ConflictError("Room is not available for reservations.");

        var periodResult = ReservationPeriod.Create(command.Start, command.End);
        if (periodResult.IsFailure) return periodResult.Error!;

        return await repository.TryChangePeriod(reservation, periodResult.Value!, cancellationToken);
    }
}
