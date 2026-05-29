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
            var command = new ConfirmReservationCommand(reservationId);
            var result = await handler.Handle(command, cancellationToken);
            return result.ToHttpResult();
        });

        return group;
    }
}
