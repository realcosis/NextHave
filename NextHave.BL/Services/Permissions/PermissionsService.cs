using NextHave.DAL.MySQL;
using Dolphin.Core.Injection;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using Microsoft.EntityFrameworkCore;
using NextHave.BL.Models.Permissions;
using Microsoft.Extensions.DependencyInjection;

namespace NextHave.BL.Services.Permissions
{
    [Service(ServiceLifetime.Singleton)]
    class PermissionsService(ILogger<IPermissionsService> logger, IServiceScopeFactory serviceScopeFactory) : IPermissionsService, IStartableService
    {
        IPermissionsService Instance => this;

        ConcurrentDictionary<int, Permission> IPermissionsService.Groups { get; } = [];

        async Task IStartableService.StartAsync()
        {
            Instance.Groups.Clear();

            await using var scope = serviceScopeFactory.CreateAsyncScope();

            try
            {
                var mysqlDbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

                var permissionGroups = await mysqlDbContext
                                            .PermissionGroups
                                                .Include(p => p.Permissions)
                                                    .ThenInclude(p => p.Right)
                                                .AsNoTracking()
                                                .ToListAsync();

                permissionGroups.ForEach(permissionGroup => Instance.Groups.TryAdd(permissionGroup.Id, new()
                {
                    Id = permissionGroup.Id,
                    Name = permissionGroup.Name,
                    Badge = permissionGroup.Badge,
                    SecurityLevel = permissionGroup.SecurityLevel,
                    Rights = [.. permissionGroup.Permissions.Select(p => p.Right!.Name!)],
                }));

                logger.LogInformation("PermissionsService started with {count} permission groups definitions", Instance.Groups.Count);
            }
            catch (Exception ex)
            {
                logger.LogWarning("Exception during starting of PermissionsService: {ex}", ex);
            }
            finally
            {
                await scope.DisposeAsync();
            }
        }
    }
}