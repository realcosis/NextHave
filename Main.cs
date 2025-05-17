using Dolphin.Core.Hosting;
using Dolphin.Core.Injection;
using System.Globalization;

namespace NextHave
{
    [Service(ServiceLifetime.Singleton)]
    public class Main(ILogger<IMain> logger, IEnumerable<IStartableService> startables, IEnumerable<IDisposableService> disposables) : IMain
    {
        public readonly static CultureInfo CultureInfo = new("it-IT");

        public readonly static DateTime UnixStart = new(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        internal const string GitMajor = ThisAssembly.Git.SemVer.Major;
        internal const string GitMinor = ThisAssembly.Git.SemVer.Minor;
        internal const string GitPatch = ThisAssembly.Git.SemVer.Patch;
        internal const string GitBranch = ThisAssembly.Git.Branch;
        internal const string GitCommit = ThisAssembly.Git.Commit;

        public readonly static string VERSION = ;

        async Task IMain.Dispose()
        {
            throw new NotImplementedException();
        }

        async Task IMain.Start()
        {
            throw new NotImplementedException();
        }
    }
}