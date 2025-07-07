namespace NextHave.BL.Messages.Output.Messenger
{
    public class MessengerInitMessageComposer() : Composer(OutputCode.MessengerInitMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(500);
            message.AddInt32(300);
            message.AddInt32(800);
            message.AddInt32(0);
        }
    }
}