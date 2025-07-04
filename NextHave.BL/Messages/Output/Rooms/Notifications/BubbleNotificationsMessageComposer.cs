namespace NextHave.BL.Messages.Output.Rooms.Notifications
{
    public class BubbleNotificationsMessageComposer(string type, Dictionary<string, string> items) : Composer(OutputCode.RoomNotificationMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddString(type);
            message.AddInt32(items.Count);
            foreach (var item in items)
            {
                message.AddString(item.Key);
                message.AddString(item.Value);
            }
        }
    }
}