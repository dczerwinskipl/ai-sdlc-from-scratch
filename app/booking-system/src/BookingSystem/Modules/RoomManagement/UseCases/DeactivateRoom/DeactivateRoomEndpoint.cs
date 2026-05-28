using BookingSystem.BuildingBlocks.Api;

namespace BookingSystem.Modules.RoomManagement.UseCases.DeactivateRoom;

internal static class DeactivateRoomEndpoint
{
    public static IEndpointRouteBuilder MapDeactivateRoomEndpoint(this IEndpointRouteBuilder group)
    {
        group.MapDelete("/{roomId:guid}", async (
            Guid roomId,
            DeactivateRoomHandler handler,
            CancellationToken cancellationToken) =>
        {
            var command = new DeactivateRoomCommand(roomId);
            var result = await handler.Handle(command, cancellationToken);
            return result.ToHttpResult();
        });

        return group;
    }
}
