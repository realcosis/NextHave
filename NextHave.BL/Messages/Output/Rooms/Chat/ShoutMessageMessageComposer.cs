namespace NextHave.BL.Messages.Output.Rooms.Chat
{
    public class ShoutMessageMessageComposer(int virtualId, string text, int emotion, int color) : Composer(OutputCode.ShoutMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(virtualId);
            message.AddString(text);
            message.AddInt32(emotion);
            message.AddInt32(color);
            message.AddInt32(0);
            message.AddInt32(-1);
        }
    }
}