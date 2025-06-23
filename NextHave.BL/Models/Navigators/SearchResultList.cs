using NextHave.BL.Enums;
using NextHave.BL.Messages;
using NextHave.BL.Models.Rooms;
using NextHave.DAL.Enums;

namespace NextHave.BL.Models.Navigators
{
    public class SearchResultList(int order, string code, string query, SearchAction action, ListMode mode, DisplayMode hidden,
                                  List<Room> rooms, bool filter, bool showInvisible, DisplayOrder displayOrder, int categoryOrder)
    {
        public int Order { get; set; } = order;

        public string? Code { get; set; } = code;

        public string? Query { get; set; } = query;

        public SearchAction Action { get; set; } = action;

        public ListMode Mode { get; set; } = mode;

        public DisplayMode Hidden { get; set; } = hidden;

        public List<Room> Rooms { get; set; } = rooms ?? [];

        public bool Filter { get; set; } = filter;

        public bool ShowInvisible { get; set; } = showInvisible;

        public DisplayOrder DisplayOrder { get; set; } = displayOrder;

        public int CategoryOrder { get; set; } = categoryOrder;

        public void Serialize(ServerMessage packet)
        {
            packet.AddString(Code!);
            packet.AddString(Query!);
            packet.AddInt32((int)Action);
            packet.AddBoolean(Hidden == DisplayMode.COLLAPSED);
            packet.AddInt32((int)Mode);

            if (!ShowInvisible)
                packet.AddInt32(rooms.Where(r => r.State != RoomAccessStatus.Invisible).Count());
            else
                packet.AddInt32(rooms.Count);

            var roomsToSend = rooms;

            roomsToSend = !ShowInvisible ? [.. roomsToSend.Where(r => r.State != RoomAccessStatus.Invisible)] : roomsToSend;

            foreach (var room in rooms)
                room.Serialize(packet);
        }
    }
}