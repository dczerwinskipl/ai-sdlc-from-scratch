using BookingSystem.BuildingBlocks.Application;
using BookingSystem.Modules.RoomManagement.Domain;
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
    public async Task<CreateReservationResponse> Handle(CreateReservationCommand command, CancellationToken cancellationToken)
    {
        var error = validator.Validate(command);
        if (error is not null)
            throw new ArgumentException(error);

        var roomId = RoomId.From(command.RoomId);
        var room = await roomReader.GetById(roomId, cancellationToken)
            ?? throw new KeyNotFoundException($"Room {command.RoomId} not found.");

        if (!room.IsActive)
            throw new InvalidOperationException("Room is not available for reservations.");

        var period = ReservationPeriod.Create(command.Start, command.End);
        var available = await availabilityChecker.IsAvailable(roomId, period, null, cancellationToken);
        if (!available)
            throw new InvalidOperationException("The selected period overlaps with an existing reservation.");

        var reservation = Reservation.Create(
            ReservationId.New(),
            roomId,
            ReservationGuest.Create(command.GuestName),
            period,
            clock.UtcNow);

        await repository.Add(reservation, cancellationToken);

        return new CreateReservationResponse(reservation.Id.Value);
    }
}
