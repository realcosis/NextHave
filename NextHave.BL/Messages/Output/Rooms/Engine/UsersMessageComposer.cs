using NextHave.BL.Services.Rooms.Instances;

namespace NextHave.BL.Messages.Output.Rooms.Engine
{
    public class UsersMessageComposer(List<IRoomUserInstance> roomUserInstances) : Composer(OutputCode.UsersMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(roomUserInstances.Count);
            foreach (var roomUserInstance in roomUserInstances)
                roomUserInstance.Serialize(message);
        }
    }
}