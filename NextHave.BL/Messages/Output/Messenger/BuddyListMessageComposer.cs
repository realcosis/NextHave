using NextHave.BL.Models.Users.Messenger;

namespace NextHave.BL.Messages.Output.Messenger
{
    public class BuddyListMessageComposer(List<MessengerBuddy> friends, int pages, int page) : Composer(OutputCode.BuddyListMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(pages);
            message.AddInt32(page);
            message.AddInt32(friends.Count);
            foreach (var friend in friends)
            {
                message.AddInt32(friend.UserId);
                message.AddString(friend.Username!);
                message.AddInt32(1);
                message.AddBoolean(friend.Online);
                message.AddBoolean(friend.Online);//&& friend.InRoom);
                message.AddString(friend.Look!);
                message.AddInt32(0);
                message.AddString(friend.Motto!);
                message.AddString(string.Empty);
                message.AddString(string.Empty);
                message.AddBoolean(true);
                message.AddBoolean(false);
                message.AddBoolean(false);
                message.AddInt16((short)friend.Relationship!);
            }
        }
    }
}