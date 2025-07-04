using NextHave.DAL.MySQL;
using NextHave.BL.Mappers;
using Dolphin.Core.Injection;
using NextHave.BL.Models.Items;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Services.Items.Factories;

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

                var interactionFactory = scope.ServiceProvider.GetRequiredService<InteractorFactory>();

                var itemDefinitions = await mysqlDbContext.ItemDefinitions.AsNoTracking().ToListAsync();

                Parallel.ForEach(itemDefinitions, new()
                {
                    CancellationToken = new(),
                    MaxDegreeOfParallelism = Environment.ProcessorCount
                }, async item => Instance.ItemDefinitions.TryAdd(item.Id, item.Map(await interactionFactory.GetInteractor(item.InteractionType!.Value))));

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