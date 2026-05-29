using BookingSystem.BuildingBlocks.Application;
using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.RoomManagement.PublicContracts;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.UseCases.Abstractions;

namespace BookingSystem.Modules.Reservations.UseCases.CreateReservation;

internal sealed class CreateReservationHandler(
    IReservationRepository repository,
    IRoomReader roomReader,
    CreateReservationValidator validator,
    IClock clock) : ICommandHandler<CreateReservationCommand, CreateReservationResponse>
{
    public async Task<Result<CreateReservationResponse>> Handle(CreateReservationCommand command, CancellationToken cancellationToken)
    {
        var error = validator.Validate(command);
        if (error is not null)
            return error;

        var room = await roomReader.GetById(command.RoomId, cancellationToken);
        if (room is null)
            return new NotFoundError($"Room {command.RoomId} not found.");

        if (!room.IsActive)
            return new ConflictError("Room is not available for reservations.");

        var periodResult = ReservationPeriod.Create(command.Start, command.End);
        if (periodResult.IsFailure) return periodResult.Error!;

        var reservation = Reservation.Create(
            ReservationId.New(),
            ReservableRoomId.From(command.RoomId),
            ReservationGuest.Create(command.GuestName),
            periodResult.Value!,
            clock.UtcNow);

        var added = await repository.TryAdd(reservation, cancellationToken);
        if (!added)
            return new ConflictError("The selected period overlaps with an existing reservation.");

        return new CreateReservationResponse(reservation.Id.Value);
    }
}
