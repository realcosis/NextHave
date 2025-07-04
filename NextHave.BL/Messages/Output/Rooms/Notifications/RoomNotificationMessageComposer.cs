namespace NextHave.BL.Messages.Output.Rooms.Notifications
{
    public class RoomNotificationMessageComposer(string title, string text, string image) : Composer(OutputCode.RoomNotificationMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddString(image);
            message.AddInt32(2);
            message.AddString("title");
            message.AddString(title);
            message.AddString("message");
            message.AddString(text);
        }
    }
}