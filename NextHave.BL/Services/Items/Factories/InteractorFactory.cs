using Dolphin.Core.Injection;
using Dolphin.Core.Plugins;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Attributes;
using NextHave.BL.Services.Items.Interactions;
using NextHave.DAL.Enums;
using System.Reflection;

namespace NextHave.BL.Services.Items.Factories
{
    [Service(ServiceLifetime.Singleton)]
    public class InteractorFactory(IServiceScopeFactory serviceScopeFactory)
    {
        List<Assembly> Assemblies = [];

        public async Task<IInteractor?> GetInteractor(InteractionTypes interactionType)
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();
            var pluginsService = scope.ServiceProvider.GetRequiredService<IPluginsService>();

            var plugins = pluginsService.GetPluginDirectory();

            if (Assemblies.Count <= 0)
            {
                var assembly = Assembly.GetEntryAssembly()!;
                var assemblies = new List<Assembly>()
                {
                    assembly
                };

                var referencedAssemblies = assembly.GetReferencedAssemblies().Where(a => a.Name!.Contains("NextHave")).ToList();

                if (referencedAssemblies.Count > 0)
                {
                    var referencedAssembliesToLoad = referencedAssemblies.Select(Assembly.Load).ToList();
                    if (referencedAssembliesToLoad.Count != 0)
                        assemblies.AddRange(referencedAssembliesToLoad);
                }

                var assembliesToLoad = plugins.Select(plugin =>
                {
                    var assemblyName = AssemblyName.GetAssemblyName(Path.Combine(Directory.GetCurrentDirectory(), plugin));
                    var assemblyToLoad = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == assemblyName.FullName) ?? Assembly.LoadFrom(Path.Combine(Directory.GetCurrentDirectory(), plugin));
                    return assemblyToLoad;
                }).ToList();
                if (assembliesToLoad.Count != 0)
                    assemblies.AddRange(assembliesToLoad);
                Assemblies = assemblies;
            }

            var interactor = Assemblies.SelectMany(a => a.GetTypes()).Where(t => t.GetCustomAttribute<InteractorAttribute>()?.InteractionType == interactionType).Select(t => new
            {
                t.Name,
                Type = t
            }).FirstOrDefault();

            return interactor != default ? scope.ServiceProvider.GetRequiredKeyedService(interactor.Type, interactor.Name) as IInteractor : default;
        }
    }
}