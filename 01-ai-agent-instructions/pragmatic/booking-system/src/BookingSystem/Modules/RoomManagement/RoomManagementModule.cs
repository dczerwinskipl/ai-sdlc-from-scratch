using BookingSystem.Modules.RoomManagement.Infrastructure;
using BookingSystem.Modules.RoomManagement.PublicContracts;
using BookingSystem.Modules.RoomManagement.UseCases.Abstractions;
using BookingSystem.Modules.RoomManagement.UseCases.AddRoom;
using BookingSystem.Modules.RoomManagement.UseCases.DeactivateRoom;
using BookingSystem.Modules.RoomManagement.UseCases.EditRoom;
using BookingSystem.Modules.RoomManagement.UseCases.GetRoom;
using BookingSystem.Modules.RoomManagement.UseCases.ListRooms;

namespace BookingSystem.Modules.RoomManagement;

public static class RoomManagementModule
{
    public static IServiceCollection AddRoomManagementModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<InMemoryRoomStore>();

        services.AddScoped<IRoomRepository, InMemoryRoomRepository>();
        services.AddScoped<IRoomReader, InMemoryRoomReader>();

        services.AddScoped<AddRoomValidator>();
        services.AddScoped<AddRoomHandler>();

        services.AddScoped<EditRoomValidator>();
        services.AddScoped<EditRoomHandler>();

        services.AddScoped<DeactivateRoomHandler>();
        services.AddScoped<GetRoomHandler>();
        services.AddScoped<ListRoomsHandler>();

        return services;
    }

    public static IEndpointRouteBuilder MapRoomManagementModule(
        this IEndpointRouteBuilder app)
    {
        var group = app
            .MapGroup("/api/rooms")
            .WithTags("Rooms");

        group.MapAddRoomEndpoint();
        group.MapEditRoomEndpoint();
        group.MapDeactivateRoomEndpoint();
        group.MapGetRoomEndpoint();
        group.MapListRoomsEndpoint();

        return app;
    }
}
