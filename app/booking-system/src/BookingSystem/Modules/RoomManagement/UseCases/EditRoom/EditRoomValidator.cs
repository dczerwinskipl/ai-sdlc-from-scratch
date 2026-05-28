namespace BookingSystem.Modules.RoomManagement.UseCases.EditRoom;

internal sealed class EditRoomValidator
{
    public string? Validate(EditRoomCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return "Room name is required.";

        if (command.Capacity <= 0)
            return "Capacity must be greater than 0.";

        return null;
    }
}
