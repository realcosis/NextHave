using NextHave.DAL.MySQL;
using Dolphin.Core.Injection;
using NextHave.BL.Models.Texts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace NextHave.BL.Services.Texts
{
    [Service(ServiceLifetime.Singleton)]
    public class TextsService(IServiceScopeFactory serviceScopeFactory) : ITextsService, IStartableService
    {
        Dictionary<string, Text> texts = [];

        ITextsService Instance => this;

        async Task IStartableService.StartAsync()
        {
            await using var scope = serviceScopeFactory.CreateAsyncScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<MySQLDbContext>();

            texts = await dbContext
                                .NextHaveTexts
                                    .ToDictionaryAsync(k => k.Key!, v => Text.Create(v.Key, v.Value, v.Description));

            await scope.DisposeAsync();
        }

        string ITextsService.GetText(string key, string defaultValue)
            => Instance.TryGetText(key, defaultValue, out var value) ? value! : defaultValue!;

        string ITextsService.FormatText(string key, string defaultValue, params object[] objects)
            => Instance.TryGetText(key, defaultValue, out var value) ? string.Format(value!, objects) : defaultValue!;

        bool ITextsService.TryGetText(string key, string defaultValue, out string? value)
        {
            var resut = false;
            if (texts.TryGetValue(key, out var setting))
            {
                value = setting.Value;
                return true;
            }

            value = defaultValue;
            return resut;
        }
    }
}