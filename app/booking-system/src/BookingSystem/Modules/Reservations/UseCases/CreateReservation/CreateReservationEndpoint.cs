using BookingSystem.BuildingBlocks.Api;

namespace BookingSystem.Modules.Reservations.UseCases.CreateReservation;

internal static class CreateReservationEndpoint
{
    public static IEndpointRouteBuilder MapCreateReservationEndpoint(this IEndpointRouteBuilder group)
    {
        group.MapPost("/", async (
            CreateReservationRequest request,
            CreateReservationHandler handler,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var command = new CreateReservationCommand(
                    request.RoomId,
                    request.GuestName,
                    request.Start,
                    request.End);
                var response = await handler.Handle(command, cancellationToken);
                return Results.Created($"/api/reservations/{response.ReservationId}", response);
            }
            catch (ArgumentException ex)
            {
                return ProblemDetailsExtensions.ValidationProblem(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return ProblemDetailsExtensions.NotFoundProblem(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return ProblemDetailsExtensions.ConflictProblem(ex.Message);
            }
        });

        return group;
    }
}
