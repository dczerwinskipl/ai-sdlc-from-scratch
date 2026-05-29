using BookingSystem.BuildingBlocks.Application;
using BookingSystem.Modules.Reservations.Infrastructure;
using BookingSystem.Modules.Reservations.UseCases.Abstractions;
using BookingSystem.Modules.Reservations.UseCases.CancelReservation;
using BookingSystem.Modules.Reservations.UseCases.ChangeReservationPeriod;
using BookingSystem.Modules.Reservations.UseCases.ConfirmReservation;
using BookingSystem.Modules.Reservations.UseCases.CreateReservation;
using BookingSystem.Modules.Reservations.UseCases.GetReservation;
using BookingSystem.Modules.Reservations.UseCases.ListReservations;

namespace BookingSystem.Modules.Reservations;

public static class ReservationsModule
{
    public static IServiceCollection AddReservationsModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<InMemoryReservationStore>();

        services.AddScoped<IReservationRepository, InMemoryReservationRepository>();

        services.AddSingleton<IClock, SystemClock>();

        services.AddScoped<CreateReservationValidator>();
        services.AddScoped<CreateReservationHandler>();

        services.AddScoped<ConfirmReservationHandler>();
        services.AddScoped<CancelReservationHandler>();

        services.AddScoped<ChangeReservationPeriodValidator>();
        services.AddScoped<ChangeReservationPeriodHandler>();

        services.AddScoped<GetReservationHandler>();
        services.AddScoped<ListReservationsHandler>();

        return services;
    }

    public static IEndpointRouteBuilder MapReservationsModule(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/reservations")
            .WithTags("Reservations");

        group.MapCreateReservationEndpoint();
        group.MapConfirmReservationEndpoint();
        group.MapCancelReservationEndpoint();
        group.MapChangeReservationPeriodEndpoint();
        group.MapGetReservationEndpoint();
        group.MapListReservationsEndpoint();

        return app;
    }
}
