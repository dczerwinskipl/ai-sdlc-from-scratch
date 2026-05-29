using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.UseCases.Abstractions;

namespace BookingSystem.Modules.Reservations.Infrastructure;

internal sealed class InMemoryReservationRepository(InMemoryReservationStore store) : IReservationRepository
{
    public Task<Reservation?> GetById(ReservationId id, CancellationToken cancellationToken)
    {
        var reservation = store.Execute(r => r.FirstOrDefault(x => x.Id == id));
        return Task.FromResult(reservation);
    }

    public Task<IReadOnlyList<Reservation>> GetAll(CancellationToken cancellationToken)
    {
        var reservations = store.Execute(r => (IReadOnlyList<Reservation>)r.ToList());
        return Task.FromResult(reservations);
    }

    public Task<bool> TryAdd(Reservation reservation, CancellationToken cancellationToken)
    {
        var added = store.Execute(reservations =>
        {
            var hasConflict = reservations.Any(r =>
                r.RoomId == reservation.RoomId &&
                r.IsActive() &&
                r.Period.Overlaps(reservation.Period));
            if (hasConflict) return false;
            reservations.Add(reservation);
            return true;
        });
        return Task.FromResult(added);
    }

    public Task<Result> TryChangePeriod(Reservation reservation, ReservationPeriod newPeriod, CancellationToken cancellationToken)
    {
        var result = store.Execute(reservations =>
        {
            var hasConflict = reservations.Any(r =>
                r.Id != reservation.Id &&
                r.RoomId == reservation.RoomId &&
                r.IsActive() &&
                r.Period.Overlaps(newPeriod));

            if (hasConflict)
                return (Result)new ConflictError("The selected period overlaps with an existing reservation.");

            return reservation.ChangePeriod(newPeriod);
        });
        return Task.FromResult(result);
    }

    public Task Update(Reservation reservation, CancellationToken cancellationToken)
    {
        store.Execute(reservations =>
        {
            var index = reservations.FindIndex(x => x.Id == reservation.Id);
            if (index >= 0) reservations[index] = reservation;
        });
        return Task.CompletedTask;
    }
}
