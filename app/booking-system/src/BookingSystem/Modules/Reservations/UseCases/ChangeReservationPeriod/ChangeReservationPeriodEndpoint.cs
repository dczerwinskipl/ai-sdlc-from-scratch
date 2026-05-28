using BookingSystem.BuildingBlocks.Api;

namespace BookingSystem.Modules.Reservations.UseCases.ChangeReservationPeriod;

internal static class ChangeReservationPeriodEndpoint
{
    public static IEndpointRouteBuilder MapChangeReservationPeriodEndpoint(this IEndpointRouteBuilder group)
    {
        group.MapPut("/{reservationId:guid}/period", async (
            Guid reservationId,
            ChangeReservationPeriodRequest request,
            ChangeReservationPeriodHandler handler,
            CancellationToken cancellationToken) =>
        {
            try
            {
                var command = new ChangeReservationPeriodCommand(reservationId, request.Start, request.End);
                await handler.Handle(command, cancellationToken);
                return Results.NoContent();
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
