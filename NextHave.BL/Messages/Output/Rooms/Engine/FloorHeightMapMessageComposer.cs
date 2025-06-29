using NextHave.BL.Models.Rooms;

namespace NextHave.BL.Messages.Output.Rooms.Engine
{
    public class FloorHeightMapMessageComposer(WorkRoomModel roomModel) : Composer(OutputCode.FloorHeightMapMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            roomModel.SerializeHeightmap(message);
        }
    }
}