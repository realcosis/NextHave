namespace NextHave.BL.Messages.Output.Handshake
{
    public class GenericErrorMessageComposer(int errorId) : Composer(OutputCode.GenericErrorMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(errorId);
        }
    }
}