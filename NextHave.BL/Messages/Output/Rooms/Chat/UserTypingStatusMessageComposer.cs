namespace NextHave.BL.Messages.Output.Rooms.Chat
{
    public class UserTypingStatusMessageComposer(int virtualId, bool isTyping) : Composer(OutputCode.UserTypingStatusMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(virtualId);
            message.AddInt32(isTyping ? 1 : 0);
        }
    }
}