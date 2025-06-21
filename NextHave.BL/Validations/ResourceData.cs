using System.Resources;

namespace NextHave.BL.Validations
{
	internal class ResourceData
	{
		internal string ResourceName { get; private set; }

		internal ResourceManager ResourceManager { get; private set; }

		internal Type? ResourceClass { get; private set; }

		internal ResourceData(string localizedResource)
		{
			ResourceName = localizedResource.Split('|')[1];
			ResourceManager = ResourceManagerInstances.GetResourceManager(localizedResource.Split('|')[0]);
			ResourceClass = Type.GetType(localizedResource.Split('|')[0]);
		}

		internal string? GetValue() => ResourceManager.GetString(ResourceName);
	}
}