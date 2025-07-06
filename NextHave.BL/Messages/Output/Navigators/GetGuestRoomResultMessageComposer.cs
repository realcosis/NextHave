using NextHave.BL.Models.Rooms;
using NextHave.DAL.Utils;

namespace NextHave.BL.Messages.Output.Navigators
{
    public class GetGuestRoomResultMessageComposer(Room room, bool isOwner, bool isLoading, bool checkEntry) : Composer(OutputCode.GetGuestRoomResultMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddBoolean(isLoading);
            message.AddInt32(room.Id);
            message.AddString(room.Caption!);
            message.AddInt32(room.OwnerId ?? 0);
            message.AddString(room.Owner ?? string.Empty);
            message.AddInt32(room.State!.Value.ToInt());
            message.AddInt32(room.UsersNow);
            message.AddInt32(room.UsersMax);
            message.AddString(room.Description ?? string.Empty);
            message.AddInt32(room.TradeSettings);
            message.AddInt32(room.Score);
            message.AddInt32(0);
            message.AddInt32(room.Category);

            message.AddInt32(room.Tags.Count);
            foreach (string tag in room.Tags)
                message.AddString(tag);

            if (room.Group != default)
            {
                message.AddInt32(58);
                message.AddInt32(room.Group.Id);
                message.AddString(room.Group.Name!);
                message.AddString(room.Group.Image!);
            }
            else
                message.AddInt32(56);


            message.AddBoolean(checkEntry);
            message.AddBoolean(false);
            message.AddBoolean(false);
            message.AddBoolean(false);

            message.AddInt32(room.MuteSettings);
            message.AddInt32(room.KickSettings);
            message.AddInt32(room.BanSettings);

            message.AddBoolean(isOwner);

            //todo 
            message.AddInt32(0);
            message.AddInt32(0);
            message.AddInt32(0);
            message.AddInt32(0);
            message.AddInt32(0);
        }
    }
}