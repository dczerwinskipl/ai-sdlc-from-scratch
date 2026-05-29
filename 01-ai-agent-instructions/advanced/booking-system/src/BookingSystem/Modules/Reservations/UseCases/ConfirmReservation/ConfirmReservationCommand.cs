using BookingSystem.BuildingBlocks.Application;

namespace BookingSystem.Modules.Reservations.UseCases.ConfirmReservation;

internal sealed record ConfirmReservationCommand(Guid ReservationId) : ICommand;
