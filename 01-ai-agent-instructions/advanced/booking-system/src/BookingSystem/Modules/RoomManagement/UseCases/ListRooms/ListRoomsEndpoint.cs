namespace BookingSystem.Modules.RoomManagement.UseCases.ListRooms;

internal static class ListRoomsEndpoint
{
    public static IEndpointRouteBuilder MapListRoomsEndpoint(this IEndpointRouteBuilder group)
    {
        group.MapGet("/", async (
            ListRoomsHandler handler,
            CancellationToken cancellationToken) =>
        {
            var query = new ListRoomsQuery();
            var response = await handler.Handle(query, cancellationToken);
            return Results.Ok(response);
        });

        return group;
    }
}
