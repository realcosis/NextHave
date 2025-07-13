namespace NextHave.BL.Messages.Output.Navigators
{
    public class FlatCreatedMessageComposer(int roomId, string roomName) : Composer(OutputCode.FlatCreatedMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(roomId);
            message.AddString(roomName);
        }
    }
}