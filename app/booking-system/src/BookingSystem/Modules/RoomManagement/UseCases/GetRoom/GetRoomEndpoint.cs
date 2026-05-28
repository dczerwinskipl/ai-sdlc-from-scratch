using BookingSystem.BuildingBlocks.Api;

namespace BookingSystem.Modules.RoomManagement.UseCases.GetRoom;

internal static class GetRoomEndpoint
{
    public static IEndpointRouteBuilder MapGetRoomEndpoint(this IEndpointRouteBuilder group)
    {
        group.MapGet("/{roomId:guid}", async (
            Guid roomId,
            GetRoomHandler handler,
            CancellationToken cancellationToken) =>
        {
            var query = new GetRoomQuery(roomId);
            var response = await handler.Handle(query, cancellationToken);
            return response is null
                ? ProblemDetailsExtensions.NotFoundProblem($"Room {roomId} not found.")
                : Results.Ok(response);
        });

        return group;
    }
}
