using BookingSystem.BuildingBlocks.Api;
using BookingSystem.BuildingBlocks.Domain;

namespace BookingSystem.Modules.Reservations.UseCases.GetReservation;

internal static class GetReservationEndpoint
{
    public static IEndpointRouteBuilder MapGetReservationEndpoint(this IEndpointRouteBuilder group)
    {
        group.MapGet("/{reservationId:guid}", async (
            Guid reservationId,
            GetReservationHandler handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetReservationQuery(reservationId);
            var response = await handler.Handle(query, cancellationToken);
            return response is null
                ? new NotFoundError($"Reservation {reservationId} not found.").ToHttpResult()
                : Results.Ok(response);
        });

        return group;
    }
}
