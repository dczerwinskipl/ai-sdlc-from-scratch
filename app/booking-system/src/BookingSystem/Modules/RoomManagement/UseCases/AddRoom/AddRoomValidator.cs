namespace BookingSystem.Modules.RoomManagement.UseCases.AddRoom;

internal sealed class AddRoomValidator
{
    public string? Validate(AddRoomCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return "Room name is required.";

        if (command.Capacity <= 0)
            return "Capacity must be greater than 0.";

        return null;
    }
}
