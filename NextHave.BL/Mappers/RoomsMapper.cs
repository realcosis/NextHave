using NextHave.BL.Models.Rooms;
using NextHave.BL.Models.Groups;
using NextHave.DAL.Mongo.Entities;
using NextHave.DAL.MySQL.Entities;

namespace NextHave.BL.Mappers
{
    public static class RoomsMapper
    {
        public static Room Map(this RoomEntity entity, Group? group = default, int usersNow = 0)
            => entity.GetMap<RoomEntity, Room>((dest => dest.Id, src => src.EntityId),
                                               (dest => dest.Caption!, src => src.Name),
                                               (dest => dest.UsersNow!, src => usersNow),
                                               (dest => dest.Category, src => src.Category!.CategoryId),
                                               (dest => dest.OwnerId!, src => src.Author!.AuthorId),
                                               (dest => dest.Owner!, src => src.Author!.Name),
                                               (dest => dest.ModelName!, src => src.Model!.ModelId),
                                               (dest => dest.Group!, src => group),
                                               (dest => dest.State!, src => entity.AccessStatus));

        public static RoomToner Map(this RoomTonerEntity entity)
            => new()
            {
                Brightness = entity.Brightness!.Value,
                Enabled = entity.Enabled,
                Hue = entity.Hue!.Value,
                ItemId = entity.ItemId!.Value,
                RoomId = entity.RoomId!.Value,
                Saturation = entity.Saturation!.Value
            };
    }
}