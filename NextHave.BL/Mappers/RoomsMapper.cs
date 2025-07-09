using MongoDB.Driver;
using NextHave.BL.Models.Groups;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Models.Rooms.Navigators;
using NextHave.DAL.Mongo.Entities;
using NextHave.DAL.Mongo.Entities.Rooms;
using NextHave.DAL.MySQL.Entities;
using System.Xml.Linq;

namespace NextHave.BL.Mappers
{
    public static class RoomsMapper
    {
        public static Room Map(this RoomEntity entity, Group? group = default, int usersNow = 0)
            => entity.GetMap<RoomEntity, Room>((dest => dest.Id, src => src.EntityId),
                                               (dest => dest.Name!, src => src.Name),
                                               (dest => dest.UsersNow!, src => usersNow),
                                               (dest => dest.Category, src => src.Category!.CategoryId),
                                               (dest => dest.OwnerId!, src => src.Author!.AuthorId),
                                               (dest => dest.Owner!, src => src.Author!.Name),
                                               (dest => dest.ModelName!, src => src.Model!.ModelId),
                                               (dest => dest.Group!, src => group),
                                               (dest => dest.State!, src => entity.AccessStatus));

        public static void Map(this RoomEntity entity, Room room, NavigatorCategory category)
        {
            entity.Name = room.Name;

            entity.AllowPets = room.AllowPets;
            entity.AllowPetsEat = room.AllowPetsEat;
            entity.AllowWalkthrough = room.AllowWalkthrough;
            entity.AllowHidewall = room.AllowHidewall;
            entity.Name = room.Name;
            entity.AccessStatus = room.State;
            entity.Description = room.Description;
            entity.Category = !entity.Category!.CategoryId.Equals(room.Category) ? new RoomCategoryEntity()
            {
                CategoryId = category.Id,
                Caption = category.Name
            } : entity.Category;
            entity.Password = room.Password;
            entity.BanSettings = room.BanSettings;
            entity.KickSettings = room.KickSettings;
            entity.MuteSettings = room.MuteSettings;
            entity.Tags.Clear();
            entity.Tags.AddRange(room.Tags);
            entity.MaxUsers = room.UsersMax;
            entity.WallThickness = room.WallThickness;
            entity.FloorThickness = room.FloorThickness;
        }

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