using BookingSystem.Modules.RoomManagement.Domain;
using BookingSystem.Modules.Reservations.Domain;

namespace BookingSystem.Modules.Reservations.UseCases.Abstractions;

internal interface IReservationAvailabilityChecker
{
    Task<bool> IsAvailable(
        RoomId roomId,
        ReservationPeriod period,
        ReservationId? excludeReservationId,
        CancellationToken cancellationToken);
}
