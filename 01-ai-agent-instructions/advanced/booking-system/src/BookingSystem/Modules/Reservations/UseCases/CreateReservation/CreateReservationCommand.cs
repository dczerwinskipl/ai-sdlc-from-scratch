using BookingSystem.BuildingBlocks.Application;

namespace BookingSystem.Modules.Reservations.UseCases.CreateReservation;

internal sealed record CreateReservationCommand(
    Guid RoomId,
    string GuestName,
    DateTimeOffset Start,
    DateTimeOffset End) : ICommand<CreateReservationResponse>;
