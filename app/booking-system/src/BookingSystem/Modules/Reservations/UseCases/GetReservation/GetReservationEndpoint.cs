using BookingSystem.BuildingBlocks.Api;

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
                ? ProblemDetailsExtensions.NotFoundProblem($"Reservation {reservationId} not found.")
                : Results.Ok(response);
        });

        return group;
    }
}
