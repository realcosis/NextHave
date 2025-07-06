using NextHave.BL.Messages.Input.Navigators;

namespace NextHave.BL.Messages.Parsers.Navigators
{
    public class GetGuestRoomParser : AbstractParser<GetGuestRoomMessage>
    {
        public override IInput Parse(ClientMessage packet)
            => new GetGuestRoomMessage()
            {
                RoomId = packet.ReadInt(),
                IsLoading = packet.ReadInt() == 1,
                CheckEntry = packet.ReadInt() == 1
            };
    }
}