using BookingSystem.BuildingBlocks.Api;

namespace BookingSystem.Modules.RoomManagement.UseCases.EditRoom;

internal static class EditRoomEndpoint
{
    public static IEndpointRouteBuilder MapEditRoomEndpoint(this IEndpointRouteBuilder group)
    {
        group.MapPut("/{roomId:guid}", async (
            Guid roomId,
            EditRoomRequest request,
            EditRoomHandler handler,
            CancellationToken cancellationToken) =>
        {
            var command = new EditRoomCommand(roomId, request.Name, request.Capacity);
            var result = await handler.Handle(command, cancellationToken);
            return result.ToHttpResult(r => Results.Ok(r));
        });

        return group;
    }
}
