using BookingSystem.BuildingBlocks.Domain;
using BookingSystem.Modules.Reservations.Domain;

namespace BookingSystem.Modules.Reservations.UseCases.Abstractions;

internal interface IReservationRepository
{
    Task<Reservation?> GetById(ReservationId id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Reservation>> GetAll(CancellationToken cancellationToken);
    Task<bool> TryAdd(Reservation reservation, CancellationToken cancellationToken);
    Task<Result> TryChangePeriod(Reservation reservation, ReservationPeriod newPeriod, CancellationToken cancellationToken);
    Task Update(Reservation reservation, CancellationToken cancellationToken);
}
