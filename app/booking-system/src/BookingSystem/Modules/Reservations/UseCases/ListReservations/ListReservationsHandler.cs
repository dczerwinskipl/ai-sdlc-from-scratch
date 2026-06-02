using BookingSystem.BuildingBlocks.Application;
using BookingSystem.Modules.Reservations.UseCases.Abstractions;

namespace BookingSystem.Modules.Reservations.UseCases.ListReservations;

internal sealed class ListReservationsHandler(
    IReservationRepository repository) : IQueryHandler<ListReservationsQuery, IReadOnlyList<ReservationListItem>>
{
    public async Task<IReadOnlyList<ReservationListItem>> Handle(ListReservationsQuery query, CancellationToken cancellationToken)
    {
        var reservations = await repository.GetAll(cancellationToken);
        return reservations
            .Select(r => new ReservationListItem(
                r.Id.Value,
                r.RoomId.Value,
                r.Guest.Name,
                r.Period.Start,
                r.Period.End,
                r.Status.ToString()))
            .ToList();
    }
}
