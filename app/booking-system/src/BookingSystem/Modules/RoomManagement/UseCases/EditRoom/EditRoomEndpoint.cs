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
            try
            {
                var command = new EditRoomCommand(roomId, request.Name, request.Capacity);
                var response = await handler.Handle(command, cancellationToken);
                return Results.Ok(response);
            }
            catch (ArgumentException ex)
            {
                return ProblemDetailsExtensions.ValidationProblem(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return ProblemDetailsExtensions.NotFoundProblem(ex.Message);
            }
        });

        return group;
    }
}
