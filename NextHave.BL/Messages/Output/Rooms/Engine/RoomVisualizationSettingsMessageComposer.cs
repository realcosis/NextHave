namespace NextHave.BL.Messages.Output.Rooms.Engine
{
    public class RoomVisualizationSettingsMessageComposer(bool hideWall, int wallThickness, int floorThickness) : Composer(OutputCode.RoomVisualizationSettingsMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddBoolean(hideWall);
            message.AddInt32(wallThickness);
            message.AddInt32(floorThickness);
        }
    }
}