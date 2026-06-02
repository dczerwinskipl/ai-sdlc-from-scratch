using BookingSystem.BuildingBlocks.Application;
using BookingSystem.Modules.Reservations.Domain;
using BookingSystem.Modules.Reservations.UseCases.Abstractions;

namespace BookingSystem.Modules.Reservations.UseCases.GetReservation;

internal sealed class GetReservationHandler(
    IReservationRepository repository) : IQueryHandler<GetReservationQuery, GetReservationResponse?>
{
    public async Task<GetReservationResponse?> Handle(GetReservationQuery query, CancellationToken cancellationToken)
    {
        var reservation = await repository.GetById(ReservationId.From(query.ReservationId), cancellationToken);
        if (reservation is null) return null;

        return new GetReservationResponse(
            reservation.Id.Value,
            reservation.RoomId.Value,
            reservation.Guest.Name,
            reservation.Period.Start,
            reservation.Period.End,
            reservation.Status.ToString(),
            reservation.CreatedAt);
    }
}
