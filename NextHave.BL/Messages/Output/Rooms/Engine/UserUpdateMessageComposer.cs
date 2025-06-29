using NextHave.BL.Services.Rooms.Instances;

namespace NextHave.BL.Messages.Output.Rooms.Engine
{
    public class UserUpdateMessageComposer(List<IRoomUserInstance> roomUserInstances) : Composer(OutputCode.UserUpdateMessageComposer)
    {
        public override void Compose(ServerMessage message)
        {
            message.AddInt32(roomUserInstances.Count);

            foreach (var roomUserInstance in roomUserInstances)
                roomUserInstance.SerializeStatus(message);
        }
    }
}