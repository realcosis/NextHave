using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Enums;
using NextHave.BL.Mappers;
using NextHave.BL.Models.Groups;
using NextHave.BL.Models.Navigators;
using NextHave.BL.Models.Rooms;
using NextHave.BL.Models.Users;
using NextHave.DAL.Mongo;
using NextHave.DAL.Mongo.Entities;
using NextHave.DAL.MySQL;

namespace Dolphin.HabboHotel.Navigators.Filters
{
    [Service(ServiceLifetime.Singleton)]
    class NavigatorQueryFilter(MongoDbContext mongoDbContext, IDbContextFactory<MySQLDbContext> mysqlDbContextFactory) : IFilter
    {
        string IFilter.Name => "query";

        static Room Convert(RoomEntity room, List<Group> groups)
        {
            var group = default(Group?);
            if (room.Group != default)
                group = groups.FirstOrDefault(g => g.Id == room.Group.GroupId) ?? default;
            return room.Map(group);
        }

        async Task<List<SearchResultList>> IFilter.GetSearchResults(User habbo, string? query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return [];

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
                                    rooms = rooms.Where(r => r.Author!.Name == value);
                                    break;
                                }
                            case "roomname":
                                {
                                    rooms = rooms.Where(r => r.Name!.Contains(value));
                                    break;
                                }
                            case "group":
                                {
                                    rooms = rooms.Where(r => r.Group != default && r.Group.Name!.Contains(value));
                                    break;
                                }
                            case "tag":
                                {
                                    rooms = rooms.Where(r => r.Tags.Contains(value));
                                    break;
                                }
                        }
                    }
                    await using var mysqlDbContext = await mysqlDbContextFactory.CreateDbContextAsync();
                    //var groups = mysqlDbContext.Groups.AsNoTracking().Where(g => rooms.Where(r => r.Group != default).Select(r => r.Group!.GroupId).Contains(g.Id)).Select(g => g.Map());
                    var resultRooms = rooms.Select(r => Convert(r, new())).ToList(); //todo: replace new to groups
                    var result = new SearchResultList(0, "query", string.Empty, SearchAction.NONE, ListMode.LIST, DisplayMode.VISIBLE, resultRooms, false, false, DisplayOrder.ACTIVITY, 1);
                    resultLists.Add(result);
                }
            }
            else
            {
                var rooms = mongoDbContext.Rooms.AsNoTracking().Where(r => r.Tags.Contains(query) || (r.Group != default && r.Group.Name!.Contains(query)) || r.Name!.Contains(query) || r.Author!.Name == query);
                await using var mysqlDbContext = await mysqlDbContextFactory.CreateDbContextAsync();
                //var groups = mysqlDbContext.Groups.AsNoTracking().Where(g => rooms.Where(r => r.Group != default).Select(r => r.Group!.GroupId).Contains(g.Id)).Select(g => g.Map());
                var resultRooms = rooms.Select(r => Convert(r, new())).ToList(); //todo: replace new to groups
                var result = new SearchResultList(0, "query", string.Empty, SearchAction.NONE, ListMode.LIST, DisplayMode.VISIBLE, resultRooms, false, false, DisplayOrder.ACTIVITY, 1);
                resultLists.Add(result);
            }

            return resultLists;
        }
    }
}