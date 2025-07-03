using NextHave.BL.Messages.Input.Rooms;

namespace NextHave.BL.Messages.Parsers.Rooms.Rooms
{
    public class GetRoomEntryParser : AbstractParser<GetRoomEntryDataMessage>
    {
        public override IInput Parse(ClientMessage packet)
            => new GetRoomEntryDataMessage();
    }
}