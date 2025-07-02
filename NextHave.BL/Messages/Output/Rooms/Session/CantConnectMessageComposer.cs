namespace NextHave.BL.Messages.Output.Rooms.Session
{
    public class CantConnectMessageComposer(int error) : Composer(OutputCode.CantConnectMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(error);
        }
    }
}