using BookingSystem.BuildingBlocks.Application;
using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.RoomManagement.PublicContracts;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.UseCases.Abstractions;

namespace BookingSystem.Modules.Reservations.UseCases.CreateReservation;

internal sealed class CreateReservationHandler(
    IReservationRepository repository,
    IReservationAvailabilityChecker availabilityChecker,
    IRoomReader roomReader,
    CreateReservationValidator validator,
    IClock clock) : ICommandHandler<CreateReservationCommand, CreateReservationResponse>
{
    public async Task<Result<CreateReservationResponse>> Handle(CreateReservationCommand command, CancellationToken cancellationToken)
    {
        var validationError = validator.Validate(command);
        if (validationError is not null)
            return new ValidationError(validationError);

        var room = await roomReader.GetById(command.RoomId, cancellationToken);
        if (room is null)
            return new NotFoundError($"Room {command.RoomId} not found.");

        if (!room.IsActive)
            return new ConflictError("Room is not available for reservations.");

        try
        {
            var roomId = ReservableRoomId.From(command.RoomId);
            var period = ReservationPeriod.Create(command.Start, command.End);
            var available = await availabilityChecker.IsAvailable(roomId, period, null, cancellationToken);
            if (!available)
                return new ConflictError("The selected period overlaps with an existing reservation.");

            var reservation = Reservation.Create(
                ReservationId.New(),
                roomId,
                ReservationGuest.Create(command.GuestName),
                period,
                clock.UtcNow);

            await repository.Add(reservation, cancellationToken);
            return new CreateReservationResponse(reservation.Id.Value);
        }
        catch (DomainException ex)
        {
            return new ConflictError(ex.Message);
        }
    }
}
