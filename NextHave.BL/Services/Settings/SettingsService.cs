using Dolphin.Core.Injection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NextHave.BL.Models.Settings;
using NextHave.DAL.MySQL;

namespace NextHave.BL.Services.Settings
{
    [Service(ServiceLifetime.Singleton)]
    public class SettingsService(IServiceScopeFactory serviceScopeFactory) : ISettingsService, IStartableService
    {
        Dictionary<string, Setting> settings = [];

        ISettingsService Instance => this;

        async Task IStartableService.StartAsync()
        {
            await using var serviceProvider = serviceScopeFactory.CreateAsyncScope();
            var dbContext = serviceProvider.ServiceProvider.GetRequiredService<MySQLDbContext>();

            settings = await dbContext
                                .NextHaveSettings
                                    .ToDictionaryAsync(k => k.Key!, v => Setting.Create(v.Key, v.Value, v.Type, v.Description));
        }

        string? ISettingsService.GetSetting(string key)
            => Instance.TryGetSetting(key, out var value) ? value : string.Empty;

        bool ISettingsService.TryGetSetting(string key, out string? value)
        {
            var resut = false;
            if (settings.TryGetValue(key, out var setting))
            {
                value = setting.Value;
                return true;
            }

            value = string.Empty;
            return resut;
        }
    }
}