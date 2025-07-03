namespace NextHave.BL.Messages.Output.Rooms.Chat
{
    public class ChatMessageMessageComposer(int virtualId, string text, int emotion, int color) : Composer(OutputCode.ChatMessageComposer)
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