using Dolphin.Core.Injection;

namespace NextHave.Gateway.Services
{
    [Service(ServiceLifetime.Singleton)]
    public class ConnectionsService(IConnectionThrottler connectionThrottler) : IStartableService
    {
        public async Task StartAsync()
        {
            var cancellationSource = new CancellationTokenSource();
            _ = Task.Run(async () =>
            {
                using var timer = new PeriodicTimer(TimeSpan.FromMinutes(1));
                while (await timer.WaitForNextTickAsync(cancellationSource.Token))
                    connectionThrottler.CleanupConnections();
            });
            await Task.CompletedTask;
        }
    }
}