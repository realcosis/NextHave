using System.Resources;

namespace NextHave.BL.Validations
{
	internal static class ResourceManagerInstances
	{
		readonly static Lock Lock = new();
		readonly static string resourceManagerName = "ResourceManager";
		readonly static Dictionary<string, ResourceManager> resourceManagers = [];

		internal static ResourceManager GetResourceManager(string key)
		{
			if (!resourceManagers.ContainsKey(key))
				lock (Lock)
				{
					if (!resourceManagers.ContainsKey(key))
						resourceManagers.Add(key, (ResourceManager)Type.GetType(key)!.GetProperty(resourceManagerName)!.GetValue(null)!);
				}

			return resourceManagers[key];
		}
	}
}