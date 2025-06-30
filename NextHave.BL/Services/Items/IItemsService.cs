using NextHave.BL.Models.Items;
using System.Collections.Concurrent;

namespace NextHave.BL.Services.Items
{
    public interface IItemsService
    {
        ConcurrentDictionary<int, ItemDefinition> ItemDefinitions { get; }

        bool TryGetItemDefinition(int id, out ItemDefinition? itemDefinition);
    }
}