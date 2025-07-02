namespace NextHave.BL.Messages.Output.Navigators
{
    public class FlatAccessDeniedMessageComposer(string username) : Composer(OutputCode.FlatAccessDeniedMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddString(username);
        }
    }
}