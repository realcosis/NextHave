namespace NextHave.BL.Constants
{
    public static class ClientConstants
    {
        public readonly static int StepIntervalMs = 420;

        public readonly static int ClientFps = 80;

        public readonly static int StraightStepFrames = 34;

        public readonly static int DiagonalStepFrames = (int)Math.Round(StraightStepFrames * Math.Sqrt(2));

        public static int FramesToMs(int frames)
            => frames * 1000 / ClientFps;
    }
}