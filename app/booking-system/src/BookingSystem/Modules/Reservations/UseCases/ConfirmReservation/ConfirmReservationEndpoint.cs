using BookingSystem.BuildingBlocks.Api;

namespace BookingSystem.Modules.Reservations.UseCases.ConfirmReservation;

internal static class ConfirmReservationEndpoint
{
    public static IEndpointRouteBuilder MapConfirmReservationEndpoint(this IEndpointRouteBuilder group)
    {
        group.MapPost("/{reservationId:guid}/confirm", async (
            Guid reservationId,
            ConfirmReservationHandler handler,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var command = new ConfirmReservationCommand(reservationId);
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
