using AutoMapper;
using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Reflection;

namespace NextHave.BL
{
    public static class BusinessLogicExtensions
    {
        public static void AddNextHaveServices(this IServiceCollection serviceCollection, Assembly assembly)
        {
            var assemblies = new List<Assembly>();
            var referencedAssemblies = assembly.GetReferencedAssemblies().Where(a => a.Name!.Contains("NextHave")).ToList();

            if (referencedAssemblies.Count > 0)
            {
                var referencedAssembliesToLoad = referencedAssemblies.Select(Assembly.Load).ToList();
                if (referencedAssembliesToLoad.Count != 0)
                    assemblies.AddRange(referencedAssembliesToLoad);
            }

            var services = assemblies.SelectMany(assembly => assembly.GetTypes().Where(t => t.GetCustomAttributes<Service>(true).Any())).ToList();
            foreach (var service in services)
            {
                var attributeData = service.GetCustomAttribute<Service>();
                if (attributeData != default)
                {
                    if (attributeData.Keyed)
                        serviceCollection.AddKeyedWithInterfaces(service, attributeData.Lifetime, attributeData.Key);
                    else
                        serviceCollection.AddWithInterfaces(service, attributeData.Lifetime);
                }
            }
        }

        public static IMappingExpression<E, R> MapProperties<E, R>(this IMappingExpression<E, R> mappingExpression, params (Expression<Func<R, object>> destinationMember, Expression<Func<E, object?>> sourceMember)[] properties)
        {
            foreach (var (dest, src) in properties)
            {
                mappingExpression.ForMember(dest, opt => opt.MapFrom(src));
            }
            return mappingExpression;
        }

        internal static R GetMap<E, R>(this E @object, params (Expression<Func<R, object>> destinationMember, Expression<Func<E, object?>> sourceMember)[] properties)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<E, R>().MapProperties(properties));
            return config.CreateMapper().Map<R>(@object);
        }
    }
}