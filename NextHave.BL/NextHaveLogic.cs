using Dolphin.Core.Plugins;

namespace NextHave.BL
{
    public class NextHave : DolphinPlugin
    {
        public new string? Name { get; } = "NextHaveLogic";

        public new string? Author { get; } = "RealCosis";

        public new string? Version { get; private set; } = "1.0.0";
    }
}