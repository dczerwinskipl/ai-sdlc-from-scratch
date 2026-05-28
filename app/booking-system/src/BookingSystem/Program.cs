using BookingSystem.Modules.Reservations;
using BookingSystem.Modules.RoomManagement;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddProblemDetails();

builder.Services.AddRoomManagementModule(builder.Configuration);
builder.Services.AddReservationsModule(builder.Configuration);

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapRoomManagementModule();
app.MapReservationsModule();

app.Run();
