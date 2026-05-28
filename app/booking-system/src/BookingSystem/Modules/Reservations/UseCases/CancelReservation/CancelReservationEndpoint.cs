using BookingSystem.BuildingBlocks.Api;

namespace BookingSystem.Modules.Reservations.UseCases.CancelReservation;

internal static class CancelReservationEndpoint
{
    public static IEndpointRouteBuilder MapCancelReservationEndpoint(this IEndpointRouteBuilder group)
    {
        group.MapPost("/{reservationId:guid}/cancel", async (
            Guid reservationId,
            CancelReservationHandler handler,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var command = new CancelReservationCommand(reservationId);
                await handler.Handle(command, cancellationToken);
                return Results.NoContent();
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
