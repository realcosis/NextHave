namespace NextHave.BL.Messages.Output.Rooms.Engine
{
    public class RoomEntryInfoMessageComposer(int roomId, bool isOwner) : Composer(OutputCode.RoomEntryInfoMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(roomId);
            message.AddBoolean(isOwner);
        }
    }
}