using NextHave.BL.Models.Users;

namespace NextHave.BL.Messages.Output.Users
{
    public class UserRightsMessageComposer(int clubLevel, int rank, bool isAmbassador) : Composer(OutputCode.UserRightsMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(clubLevel);
            message.AddInt32(rank);
            message.AddBoolean(isAmbassador);
        }
    }
}