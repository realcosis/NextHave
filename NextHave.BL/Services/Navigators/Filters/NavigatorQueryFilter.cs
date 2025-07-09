using NextHave.BL.Enums;
using NextHave.DAL.Mongo;
using NextHave.DAL.MySQL;
using NextHave.BL.Mappers;
using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using NextHave.BL.Models.Navigators;
using NextHave.BL.Services.Users.Instances;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Services.Rooms;

namespace NextHave.BL.Services.Navigators.Filters
{
    [Service(ServiceLifetime.Singleton)]
    class NavigatorQueryFilter(IServiceScopeFactory serviceScopeFactory) : IFilter
    {
        string IFilter.Name => "query";

        async Task<List<SearchResultList>> IFilter.GetSearchResults(IUserInstance userInstance, string? query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return [];

            var mongoDbContext = await serviceScopeFactory.GetRequiredService<MongoDbContext>();
            var mysqlDbContext = await serviceScopeFactory.GetRequiredService<MySQLDbContext>();
            var roomsService = await serviceScopeFactory.GetRequiredService<IRoomsService>();

            var resultLists = new List<SearchResultList>();

            if (query.Contains(':'))
            {
                var data = query.Split(':').ToArray();
                if (data.Length != 0)
                {
                    var rooms = mongoDbContext.Rooms.AsNoTracking();
                    var searchType = data[0];
                    var value = data[1];
                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        switch (searchType)
                        {
                            case "owner":
                                {
                                    rooms = rooms.Where(r => r.Author!.Name!.ToLower() == value.ToLower());
                                    break;
                                }
                            case "roomname":
                                {
                                    rooms = rooms.Where(r => r.Name!.ToLower().Contains(value.ToLower()));
                                    break;
                                }
                            case "group":
                                {
                                    rooms = rooms.Where(r => r.Group != default && r.Group.Name!.ToLower().Contains(value.ToLower()));
                                    break;
                                }
                            case "tag":
                                {
                                    rooms = rooms.Where(r => r.Tags.Contains(value));
                                    break;
                                }
                        }
                    }
                    var results = await rooms.ToListAsync();
                    var groupIds = results.Where(r => r.Group != default).Select(r => r.Group!.GroupId).Distinct().ToList();
                    var groups = await mysqlDbContext.Groups.AsNoTracking().Where(g => groupIds.Contains(g.Id)).Select(g => g.Map()).ToListAsync();
                    var resultRooms = results.Select(r => r.Map(groups.FirstOrDefault(g => g.RoomId == r.EntityId), roomsService.ActiveRooms.TryGetValue(r.EntityId!.Value, out var room) ? room.Room!.UsersNow : 0)).ToList();
                    var result = new SearchResultList(0, "query", string.Empty, SearchAction.NONE, ListMode.LIST, DisplayMode.VISIBLE, resultRooms, false, false, DisplayOrder.ACTIVITY, 1);
                    resultLists.Add(result);
                }
            }
            else
            {
                var rooms = await mongoDbContext.Rooms.AsNoTracking().Where(r => r.Tags.Contains(query) || (r.Group != default && r.Group.Name!.ToLower().Contains(query.ToLower())) || r.Name!.ToLower().Contains(query.ToLower()) || r.Author!.Name!.ToLower() == query.ToLower()).ToListAsync();
                var groupIds = rooms.Where(r => r.Group != default).Select(r => r.Group!.GroupId).Distinct().ToList();
                var groups = await mysqlDbContext.Groups.AsNoTracking().Where(g => groupIds.Contains(g.Id)).Select(g => g.Map()).ToListAsync();
                var resultRooms = rooms.Select(r => r.Map(groups.FirstOrDefault(g => g.RoomId == r.EntityId), roomsService.ActiveRooms.TryGetValue(r.EntityId!.Value, out var room) ? room.Room!.UsersNow : 0)).ToList();
                var result = new SearchResultList(0, "query", string.Empty, SearchAction.NONE, ListMode.LIST, DisplayMode.VISIBLE, resultRooms, false, false, DisplayOrder.ACTIVITY, 1);
                resultLists.Add(result);
            }

            return resultLists;
        }
    }
}