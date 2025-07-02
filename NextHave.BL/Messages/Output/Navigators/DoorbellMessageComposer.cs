namespace NextHave.BL.Messages.Output.Navigators
{
    public class DoorbellMessageComposer(string username) : Composer(OutputCode.DoorbellMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddString(username);
        }
    }
}