using NextHave.BL.Models;
using NextHave.BL.Models.Items;
using NextHave.DAL.MySQL.Entities;
using NextHave.DAL.Mongo.Entities;
using NextHave.BL.Services.Rooms.Instances;
using NextHave.BL.Services.Items.Interactions;

namespace NextHave.BL.Mappers
{
    public static class ItemMappers
    {
        public static ItemDefinition Map(this ItemDefinitionEntity entity, IInteractor? interactor)
            => entity.GetMap<ItemDefinitionEntity, ItemDefinition>((dest => dest.Interactor!, src => interactor));

        public static RoomItem Map(this RoomItemEntity entity, ItemDefinition definition, IRoomInstance roomInstance, int rareValue = 0)
            => new()
            {
                Base = definition,
                BaseItem = definition.Id,
                CurrentStack = entity.Limited?.CurrentStack ?? 0,
                ExtraData = entity.ExtraData,
                Id = entity.EntityId!.Value,
                TotalStack = entity.Limited?.TotalStack ?? 0,
                PaymentReason = entity.ValidRare?.PaymentReason,
                ValidRare = entity.ValidRare != default,
                Point = new ThreeDPoint(entity.X!.Value, entity.Y!.Value, entity.Z!.Value),
                RareValue = rareValue,
                RoomId = entity.RoomId!.Value,
                Rotation = entity.Rotation!.Value,
                RoomInstance = roomInstance,
                WallPosition = entity.WallPosition
            };
    }
}