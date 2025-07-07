namespace NextHave.BL.Messages.Output.Friends
{
    public class NewConsoleMessageMessageComposer(int userId, string text) : Composer(OutputCode.NewConsoleMessageMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(userId);
            message.AddString(text);
            message.AddInt32(0);
        }
    }
}