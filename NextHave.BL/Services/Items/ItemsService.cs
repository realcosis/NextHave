using Dolphin.Core.Backgrounds;
using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NextHave.BL.Mappers;
using NextHave.BL.Models.Items;
using NextHave.DAL.MySQL;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Items
{
    [Service(ServiceLifetime.Singleton)]
    class ItemsService(IServiceScopeFactory serviceScopeFactory, ILogger<IItemsService> logger) : IItemsService, IStartableService
    {
        IItemsService Instance => this;

        ConcurrentDictionary<int, ItemDefinition> IItemsService.ItemDefinitions { get; } = [];

        async Task IStartableService.StartAsync()
        {
            Instance.ItemDefinitions.Clear();

            try
            {
                using var scope = serviceScopeFactory.CreateAsyncScope();
                var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

                var itemDefinitions = await mysqlDbContext.ItemDefinitions.AsNoTracking().ToListAsync();

                Parallel.ForEach(itemDefinitions, new()
                {
                    CancellationToken = new(),
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                }, item => Instance.ItemDefinitions.TryAdd(item.Id, item.Map()));

                logger.LogInformation("ItemsService started with {count} item definitions", Instance.ItemDefinitions.Count);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Exception during starting of ItemsService: {ex}", ex);
            }
        }

        bool IItemsService.TryGetItemDefinition(int id, out ItemDefinition? itemDefinition)
            => Instance.ItemDefinitions.TryGetValue(id, out itemDefinition);
    }
}