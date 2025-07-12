using AutoMapper;
using System.Reflection;
using Dolphin.Core.Injection;
using System.Linq.Expressions;
using Microsoft.Extensions.Logging;
using NextHave.BL.Services.Rooms.Instances;
using NextHave.BL.Services.Users.Instances;
using NextHave.BL.Services.Rooms.Factories;
using NextHave.BL.Services.Users.Components;
using NextHave.BL.Services.Rooms.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Reflection.Metadata;

namespace NextHave.BL
{
    public static class BusinessLogicExtensions
    {
        public static void AddNextHaveServices(this IServiceCollection services, Assembly assembly)
        {
            var assemblies = new List<Assembly>();
            var referencedAssemblies = assembly.GetReferencedAssemblies().Where(a => a.Name!.Contains("NextHave")).ToList();

            if (referencedAssemblies.Count > 0)
            {
                var referencedAssembliesToLoad = referencedAssemblies.Select(Assembly.Load).ToList();
                if (referencedAssembliesToLoad.Count != 0)
                    assemblies.AddRange(referencedAssembliesToLoad);
            }

            var assemblyServices = assemblies.SelectMany(assembly => assembly.GetTypes().Where(t => t.GetCustomAttributes<Service>(true).Any())).ToList();
            foreach (var assemblyService in assemblyServices)
            {
                var attributeData = assemblyService.GetCustomAttribute<Service>();
                if (attributeData != default)
                {
                    if (attributeData.Keyed)
                        services.AddKeyedWithInterfaces(assemblyService, attributeData.Lifetime, attributeData.Key);
                    else
                        services.AddWithInterfaces(assemblyService, attributeData.Lifetime);
                }
            }

            services.AddKeyedService<IRoomInstance, RoomInstance>((sp, key) =>
            {
                var roomComponents = sp.GetRequiredService<IEnumerable<IRoomComponent>>();
                var roomEventsFactory = sp.GetRequiredService<RoomEventsFactory>();
                var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                var logger = sp.GetRequiredService<ILogger<IRoomInstance>>();
                return new RoomInstance(roomComponents, roomEventsFactory, serviceScopeFactory, logger);
            });

            services.AddKeyedService<IUserInstance, UserInstance>((sp, key) =>
            {
                var userComponents = sp.GetRequiredService<IEnumerable<IUserComponent>>();
                var userEventsFactory = sp.GetRequiredService<UserEventsFactory>();
                var roomFactory = sp.GetRequiredService<RoomFactory>();
                return new UserInstance(userComponents, userEventsFactory, roomFactory);
            });

            services.AddKeyedService<IRoomUserInstance, RoomUserInstance>((sp, key) => new RoomUserInstance());

            services.AddKeyedServices();
        }

        public static async Task<T> GetRequiredService<T>(this IServiceScopeFactory serviceScopeFactory) where T : class
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var service = scope.ServiceProvider.GetRequiredService<T>();

            await scope.DisposeAsync();

            return service;
        }

        public static async Task<T> GetRequiredKeyedService<T>(this IServiceScopeFactory serviceScopeFactory, Type type, string serviceKey) where T : class
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var service = scope.ServiceProvider.GetRequiredKeyedService(type, serviceKey) as T;

            await scope.DisposeAsync();

            return service!;
        }

        public static async Task<T> GetRequiredKeyedService<T>(this IServiceScopeFactory serviceScopeFactory, string serviceKey) where T : class
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var service = scope.ServiceProvider.GetRequiredKeyedService<T>(serviceKey);

            await scope.DisposeAsync();

            return service;
        }

        static IMappingExpression<E, R> MapProperties<E, R>(this IMappingExpression<E, R> mappingExpression, params (Expression<Func<R, object>> destinationMember, Expression<Func<E, object?>> sourceMember)[] properties)
        {
            foreach (var (dest, src) in properties)
            {
                mappingExpression.ForMember(dest, opt => opt.MapFrom(src));
            }
            return mappingExpression;
        }

        public static R GetMap<E, R>(this E @object, params (Expression<Func<R, object>> destinationMember, Expression<Func<E, object?>> sourceMember)[] properties)
        {
            var config = new MapperConfiguration(cfg => cfg.CreateMap<E, R>().MapProperties(properties), LoggerFactory.Create((configure) => { }));
            return config.CreateMapper().Map<R>(@object);
        }
    }
}