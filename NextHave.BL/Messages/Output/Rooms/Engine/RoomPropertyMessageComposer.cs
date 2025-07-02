namespace NextHave.BL.Messages.Output.Rooms.Engine
{
    public class RoomPropertyMessageComposer(string key, string value) : Composer(OutputCode.RoomPropertyMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddString(key);
            message.AddString(value);
        }
    }
}