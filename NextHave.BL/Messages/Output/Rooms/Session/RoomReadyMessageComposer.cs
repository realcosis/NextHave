namespace NextHave.BL.Messages.Output.Rooms.Session
{
    public class RoomReadyMessageComposer(int roomId, string modelName) : Composer(OutputCode.RoomReadyMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddString(modelName);
            message.AddInt32(roomId);
        }
    }
}