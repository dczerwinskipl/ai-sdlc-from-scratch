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
            try
            {
                var command = new DeactivateRoomCommand(roomId);
                await handler.Handle(command, cancellationToken);
                return Results.NoContent();
            }
            catch (KeyNotFoundException ex)
            {
                return ProblemDetailsExtensions.NotFoundProblem(ex.Message);
            }
        });

        return group;
    }
}
