using NextHave.BL.Models.Rooms;

namespace NextHave.BL.Messages.Output.Rooms.Engine
{
    public class HeightMapMessageComposer(WorkRoomModel roomModel) : Composer(OutputCode.HeightMapMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            roomModel.SerializeHeight(message);
        }
    }
}