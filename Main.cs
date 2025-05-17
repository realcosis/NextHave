using Dolphin.Core.Hosting;
using System.Globalization;
using Dolphin.Core.Injection;

namespace NextHave
{
    [Service(ServiceLifetime.Singleton)]
    public class Main(ILogger<IMain> logger, IEnumerable<IStartableService> startables, IEnumerable<IDisposableService> disposables) : IMain
    {
        public readonly static CultureInfo CULTURE = new("it-IT");

        public readonly static DateTime UNIX = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public readonly static string VERSION = $"{ProjectConstants.ProjectName} {ProjectConstants.ProjectVersion}";

        async Task IMain.Dispose()
        {
            Console.Title = "NextHave || Shutdown";

            try
            {
                foreach (var disposable in disposables)
                    await disposable.DisposeAsync();

                logger.LogInformation("NextHave has successfully shutdowned");
                logger.LogWarning("Press ANY key to close the emulator.");
                Console.ReadKey();
                Environment.Exit(Environment.ExitCode);
            }
            catch (Exception ex)
            {
                GeneralException($"[UNHANDLED] {ex.Message}");
            }
        }

        async Task IMain.Start()
        {
            Console.Title = "NextHave || Loading";
            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine($"  {VERSION}");
            Console.WriteLine($"  Developed by Charlotte");
            Console.WriteLine();

            try
            {
                foreach (var startable in startables)
                    await startable.StartAsync();

                Console.Title = "NextHave || Started";
            }
            catch (Exception ex)
            {
                GeneralException($"[UNHANDLED] {ex.Message}");
            }
        }

        #region private methods

        private void GeneralException(string? text)
        {
            Console.Clear();

            logger.LogError("{message}", text);
            logger.LogWarning("Press ANY key to close the emulator.");
            Console.ReadKey();
            Environment.Exit(Environment.ExitCode);
        }

        #endregion
    }
}