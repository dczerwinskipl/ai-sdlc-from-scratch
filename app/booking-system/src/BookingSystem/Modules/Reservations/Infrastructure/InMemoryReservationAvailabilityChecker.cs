using BookingSystem.Modules.RoomManagement.Domain;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.UseCases.Abstractions;

namespace BookingSystem.Modules.Reservations.Infrastructure;

internal sealed class InMemoryReservationAvailabilityChecker(
    InMemoryReservationStore store) : IReservationAvailabilityChecker
{
    public Task<bool> IsAvailable(
        RoomId roomId,
        ReservationPeriod period,
        ReservationId? excludeReservationId,
        CancellationToken cancellationToken)
    {
        var hasConflict = store.Execute(reservations =>
            reservations.Any(r =>
                r.RoomId == roomId &&
                r.IsActive() &&
                (excludeReservationId is null || r.Id != excludeReservationId) &&
                r.Period.Overlaps(period)));

        return Task.FromResult(!hasConflict);
    }
}
