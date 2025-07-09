using NextHave.DAL.Utils;
using NextHave.BL.Services.Rooms.Instances;

namespace NextHave.BL.Messages.Output.roomInstances.Settings
{
    public class RoomSettingsDataMessageComposer(IRoomInstance roomInstance) : Composer(OutputCode.RoomSettingsDataMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(roomInstance.Room!.Id);
            message.AddString(roomInstance.Room.Name!);
            message.AddString(roomInstance.Room.Description!);
            message.AddInt32(roomInstance.Room.State!.Value.ToInt());
            message.AddInt32(roomInstance.Room.Category);
            message.AddInt32(roomInstance.Room.UsersMax);
            message.AddInt32(((roomInstance.RoomModel!.MapSizeX * roomInstance.RoomModel.MapSizeY) > 100) ? 50 : 25);

            message.AddInt32(roomInstance.Room.Tags.Count);
            foreach (var tag in roomInstance.Room.Tags)
                message.AddString(tag);

            message.AddInt32(roomInstance.Room.TradeSettings);
            message.AddInt32(roomInstance.Room.AllowPets ? 1 : 0);
            message.AddInt32(roomInstance.Room.AllowPetsEat ? 1 : 0);
            message.AddInt32(roomInstance.Room.AllowWalkthrough ? 1 : 0);
            message.AddInt32(roomInstance.Room.AllowHidewall ? 1 : 0);
            message.AddInt32(roomInstance.Room.WallThickness);
            message.AddInt32(roomInstance.Room.FloorThickness);

            message.AddInt32(roomInstance.Room.ChatMode);
            message.AddInt32(roomInstance.Room.ChatWeight);
            message.AddInt32(roomInstance.Room.ChatSpeed);
            message.AddInt32(roomInstance.Room.ChatDistance);
            message.AddInt32(roomInstance.Room.ChatProtection);

            message.AddBoolean(roomInstance.Room.AllowNavigatorDynamicCategories);

            message.AddInt32(roomInstance.Room.MuteSettings);
            message.AddInt32(roomInstance.Room.KickSettings);
            message.AddInt32(roomInstance.Room.BanSettings);
        }
    }
}