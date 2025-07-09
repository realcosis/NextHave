namespace NextHave.BL.Messages.Output.Rooms.Settings
{
    public class RoomSettingsSavedMessageComposer(int roomId) : Composer(OutputCode.RoomSettingsSavedMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(roomId);
        }
    }
}