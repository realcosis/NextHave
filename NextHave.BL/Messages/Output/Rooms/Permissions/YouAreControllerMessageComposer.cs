namespace NextHave.BL.Messages.Output.Rooms.Permissions
{
    public class YouAreControllerMessageComposer(int setting) : Composer(OutputCode.YouAreControllerMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(setting);
        }
    }
}