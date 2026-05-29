namespace BookingSystem.Modules.Reservations.UseCases.ListReservations;

internal static class ListReservationsEndpoint
{
    public static IEndpointRouteBuilder MapListReservationsEndpoint(this IEndpointRouteBuilder group)
    {
        group.MapGet("/", async (
            ListReservationsHandler handler,
            CancellationToken cancellationToken) =>
        {
            var query = new ListReservationsQuery();
            var response = await handler.Handle(query, cancellationToken);
            return Results.Ok(response);
        });

        return group;
    }
}
