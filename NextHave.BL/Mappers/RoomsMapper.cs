using NextHave.BL.Models.Groups;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Models.Rooms.Navigators;
using NextHave.DAL.Mongo.Entities;
using NextHave.DAL.MySQL.Entities;

namespace NextHave.BL.Mappers
{
    public static class RoomsMapper
    {
        public static Room Map(this RoomEntity entity, Group? group = default)
            => entity.GetMap<RoomEntity, Room>((dest => dest.Id, src => src.EntityId),
                                               (dest => dest.Caption!, src => src.Name),
                                               (dest => dest.Category, src => src.Category!.CategoryId),
                                               (dest => dest.OwnerId!, src => src.Author!.AuthorId),
                                               (dest => dest.Owner!, src => src.Author!.Name),
                                               (dest => dest.ModelName!, src => src.Model!.ModelId),
                                               (dest => dest.Group!, src => group),
                                               (dest => dest.State!, src => entity.AccessStatus));
    }
}