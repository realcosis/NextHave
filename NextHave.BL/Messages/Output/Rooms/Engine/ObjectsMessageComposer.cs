using NextHave.BL.Models.Items;

namespace NextHave.BL.Messages.Output.Rooms.Engine
{
    public class ObjectsMessageComposer(int ownerId, string ownerName, List<RoomItem> items) : Composer(OutputCode.ObjectsMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(1);
            message.AddInt32(ownerId);
            message.AddString(ownerName);

            message.AddInt32(items.Count);
            foreach (var roomItem in items)
                roomItem.Serialize(message, ownerId);

            message.AddString(ownerName);
        }
    }
}