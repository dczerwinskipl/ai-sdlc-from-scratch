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
            var command = new CancelReservationCommand(reservationId);
            var result = await handler.Handle(command, cancellationToken);
            return result.ToHttpResult();
        });

        return group;
    }
}
