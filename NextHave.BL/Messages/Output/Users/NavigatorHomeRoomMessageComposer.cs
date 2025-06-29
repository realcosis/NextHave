namespace NextHave.BL.Messages.Output.Users
{
    public class NavigatorHomeRoomMessageComposer(int homeRoom, int roomToEnter) : Composer(OutputCode.NavigatorHomeRoomMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(homeRoom);
            message.AddInt32(roomToEnter);
        }
    }
}