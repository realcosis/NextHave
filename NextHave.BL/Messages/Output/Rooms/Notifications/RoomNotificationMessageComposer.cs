namespace NextHave.BL.Messages.Output.Rooms.Notifications
{
    public class RoomNotificationMessageComposer(string title, string text, string type, string? image = default) : Composer(OutputCode.RoomNotificationMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddString(type);
            message.AddInt32(!string.IsNullOrWhiteSpace(image) ? 3 : 2);
            message.AddString("title");
            message.AddString(title);
            message.AddString("message");
            message.AddString(text);
            if (!string.IsNullOrWhiteSpace(image))
            {
                message.AddString("image");
                message.AddString(image);
            }
        }
    }
}