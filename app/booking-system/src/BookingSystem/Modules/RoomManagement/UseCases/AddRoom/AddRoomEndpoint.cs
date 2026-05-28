using BookingSystem.BuildingBlocks.Api;

namespace BookingSystem.Modules.RoomManagement.UseCases.AddRoom;

internal static class AddRoomEndpoint
{
    public static IEndpointRouteBuilder MapAddRoomEndpoint(this IEndpointRouteBuilder group)
    {
        group.MapPost("/", async (
            AddRoomRequest request,
            AddRoomHandler handler,
            CancellationToken cancellationToken) =>
        {
            var command = new AddRoomCommand(request.Name, request.Capacity);
            var result = await handler.Handle(command, cancellationToken);
            return result.ToHttpResult(r => Results.Created($"/api/rooms/{r.RoomId}", r));
        });

        return group;
    }
}
