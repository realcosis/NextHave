using AutoMapper;
using Dolphin.Core.Injection;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.PacketParsers;
using NextHave.BL.Services.Rooms.Components;
using NextHave.BL.Services.Rooms.Factories;
using NextHave.BL.Services.Rooms.Instances;
using NextHave.BL.Services.Users.Components;
using NextHave.BL.Services.Users.Instances;
using NextHave.DAL.Enums;
using System.Linq.Expressions;
using System.Reflection;

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
                    if (attributeData.Keyed && attributeData.Key == nameof(GamePacketParser))
                        Console.WriteLine("non funziono");
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
                return new RoomInstance(roomComponents, roomEventsFactory, serviceScopeFactory);
            });

            services.AddKeyedService<IUserInstance, UserInstance>((sp, key) =>
            {
                var userComponents = sp.GetRequiredService<IEnumerable<IUserComponent>>();
                var userEventsFactory = sp.GetRequiredService<UserEventsFactory>();
                var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                return new UserInstance(userComponents, userEventsFactory, serviceScopeFactory);
            });

            services.AddKeyedService<IRoomUserInstance, RoomUserInstance>((sp, key) => new RoomUserInstance());

            services.AddKeyedServices();
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
            var config = new MapperConfiguration(cfg => cfg.CreateMap<E, R>().MapProperties(properties));
            return config.CreateMapper().Map<R>(@object);
        }
    }
}