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
            var command = new ChangeReservationPeriodCommand(reservationId, request.Start, request.End);
            var result = await handler.Handle(command, cancellationToken);
            return result.ToHttpResult();
        });

        return group;
    }
}
