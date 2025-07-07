using NextHave.BL.Models.Users;
using NextHave.BL.Models.Users.Messenger;

namespace NextHave.BL.Messages.Output.Messenger
{
    public class FriendListUpdateMessageComposer(User? user, MessengerBuddy? buddy, int? friendId) : Composer(OutputCode.FriendListUpdateMessageComposer)
    {
        public override void Compose(ServerMessage packet)
        {
            if (user != default && buddy != default)
            {
                packet.AddInt32(0);
                packet.AddInt32(1);
                packet.AddInt32(0);

                buddy.Serialize(packet, user);
            }
            else if (friendId.HasValue)
            {
                packet.AddInt32(0);
                packet.AddInt32(1);
                packet.AddInt32(-1);
                packet.AddInt32(friendId.Value);
            }
            else
            {
                packet.AddInt32(0);
                packet.AddInt32(0);
            }
        }
    }
}