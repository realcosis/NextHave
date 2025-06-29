namespace NextHave.BL.Messages.Output.Rooms.Engine
{
    public class UserRemoveMessageComposer(int virtualId) : Composer(OutputCode.UserRemoveMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddString(virtualId.ToString());
        }
    }
}