using NextHave.BL.Models.Users;
using NextHave.BL.Utils;

namespace NextHave.BL.Messages.Output.Users
{
    public class UserObjectMessageComposer(User user) : Composer(OutputCode.UserObjectMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(user.Id);
            message.AddString(user.Username!);
            message.AddString(user.Look!);
            message.AddString(user.Gender!.ToUpper());
            message.AddString(user.Motto ?? string.Empty);
            message.AddString(string.Empty);
            message.AddBoolean(false);
            message.AddInt32(0);
            message.AddInt32(0);
            message.AddInt32(0);
            message.AddBoolean(false);
            message.AddString(user.LastOnline!.Value.GetDifference().ToString());
            message.AddBoolean(false);
            message.AddBoolean(false);
        }
    }
}