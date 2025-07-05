namespace NextHave.BL
{
    public static class ProjectConstants
    {
        public readonly static string Name = "NextHave";

        public readonly static string GitMajor = ThisAssembly.Git.SemVer.Major;
        public readonly static string GitMinor = ThisAssembly.Git.SemVer.Minor;
        public readonly static string GitPatch = ThisAssembly.Git.SemVer.Patch;
        
        public readonly static string Build = $"{GitMinor}.{GitMinor}.{GitPatch}";
     
        public readonly static DateTime StartDate = DateTime.Now;
    }
}