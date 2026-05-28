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
            try
            {
                var command = new AddRoomCommand(request.Name, request.Capacity);
                var response = await handler.Handle(command, cancellationToken);
                return Results.Created($"/api/rooms/{response.RoomId}", response);
            }
            catch (ArgumentException ex)
            {
                return ProblemDetailsExtensions.ValidationProblem(ex.Message);
            }
        });

        return group;
    }
}
