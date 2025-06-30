using NextHave.DAL.MySQL.Entities;
using NextHave.BL.Models.Navigators;
using NextHave.BL.Models.Rooms.Navigators;

namespace NextHave.BL.Mappers
{
    public static class NavigatorMapper
    {
        public static NavigatorPublicCategory Map(this NavigatorPublicCategoryEntity entity)
            => new()
            {
                Name = entity.Name,
                Id = entity.Id,
                OrderNumber = entity.OrderNumber,
                Rooms = []
            };

        public static NavigatorCategory Map(this NavigatorUserCategoryEntity entity)
            => new()
            {
                Id = entity.Id,
                Name = entity.Name,
                Enabled = entity.Enabled,
                MinRank = entity.MinRank
            };
    }
}