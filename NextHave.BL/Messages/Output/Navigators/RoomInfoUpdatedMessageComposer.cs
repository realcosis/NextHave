namespace NextHave.BL.Messages.Output.Navigators
{
    public class RoomInfoUpdatedMessageComposer(int roomId) : Composer(OutputCode.RoomInfoUpdatedMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(roomId);
        }
    }
}