namespace NextHave.BL.Messages.Output.Rooms.Session
{
    public class OpenConnectionMessageComposer(int roomId) : Composer(OutputCode.OpenConnectionMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(roomId);
        }
    }
}