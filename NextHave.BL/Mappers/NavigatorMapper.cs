using NextHave.BL.Models.Navigators;
using NextHave.DAL.MySQL.Entities;

namespace NextHave.BL.Mappers
{
    public static class NavigatorMapper
    {
        public static NavigatorPublicCategory Map(this NavigatorPublicCategoryEntity entity)
            => new()
            {
                Name = entity.Name,
                Id = entity.Id,
                OrderNum = entity.OrderNumber,
                Rooms = []
            };
    }
}