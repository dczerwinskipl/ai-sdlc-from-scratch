using BookingSystem.BuildingBlocks.Domain;

namespace BookingSystem.Modules.RoomManagement.UseCases.EditRoom;

internal sealed class EditRoomValidator
{
    public Error? Validate(EditRoomCommand command)
    {
        if (string.IsNullOrWhiteSpace(command.Name))
            return new ValidationError("Room name is required.");

        if (command.Capacity <= 0)
            return new ValidationError("Capacity must be greater than 0.");

        return null;
    }
}
