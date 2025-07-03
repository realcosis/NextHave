using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Dolphin.Core.Injection;
using System.Collections.Concurrent;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Models.Groups;
using NextHave.DAL.MySQL;
using NextHave.BL.Mappers;

namespace NextHave.BL.Services.Groups
{
    [Service(ServiceLifetime.Singleton)]
    class GroupsService(IServiceScopeFactory serviceScopeFactory, ILogger<IGroupsService> logger) : IGroupsService, IStartableService
    {
        IGroupsService Instance => this;

        ConcurrentDictionary<string, GroupElement> IGroupsService.GroupElements { get; } = [];

        async Task<Group?> IGroupsService.GetGroup(int groupId)
        {
            await using var serviceProvider = serviceScopeFactory.CreateAsyncScope();
            var mysqlDbContext = serviceProvider.ServiceProvider.GetRequiredService<MySQLDbContext>();

            var groupEntity = await mysqlDbContext
                                        .Groups
                                            .AsNoTracking()
                                            .Include(g => g.Members)
                                                .ThenInclude(m => m.User)
                                            .FirstOrDefaultAsync(g => g.Id == groupId);

            return groupEntity?.Map();
        }

        async Task IStartableService.StartAsync()
        {
            Instance.GroupElements.Clear();

            try
            {
                await using var serviceProvider = serviceScopeFactory.CreateAsyncScope();
                var mysqlDbContext = serviceProvider.ServiceProvider.GetRequiredService<MySQLDbContext>();

                var groupElements = await mysqlDbContext.GroupElements.AsNoTracking().ToListAsync();
                groupElements.ForEach(groupElement => Instance.GroupElements.TryAdd($"{groupElement.Id}-{groupElement.Type}", groupElement.Map()));

                logger.LogInformation("GroupsService started with {count} group elements definitions", Instance.GroupElements.Count);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Exception during starting of GroupsService: {ex}", ex);
            }
        }
    }
}