using BookingSystem.BuildingBlocks.Application;

namespace BookingSystem.Modules.Reservations.UseCases.CancelReservation;

internal sealed record CancelReservationCommand(Guid ReservationId) : ICommand;
