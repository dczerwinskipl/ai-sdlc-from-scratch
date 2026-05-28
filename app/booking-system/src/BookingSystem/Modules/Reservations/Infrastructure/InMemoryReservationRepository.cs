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

    public Task Add(Reservation reservation, CancellationToken cancellationToken)
    {
        store.Execute(r => r.Add(reservation));
        return Task.CompletedTask;
    }

    public Task Update(Reservation reservation, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
