using System.Diagnostics;

namespace NextHave.BL.Utils
{
    public static class PerformanceUtility
    {
        public static async Task<T> MeasureAndLogAsync<T>(Func<Task<T>> action, string description)
        {
            var beforeMemory = GC.GetTotalMemory(true);
            var sw = Stopwatch.StartNew();

            var result = await action();

            sw.Stop();
            var afterMemory = GC.GetTotalMemory(true);
            var usedMemory = afterMemory - beforeMemory;

            Console.WriteLine($"Misura per: {description}");
            Console.WriteLine($"Tempo esecuzione: {sw.ElapsedMilliseconds} ms");
            Console.WriteLine($"RAM utilizzata: {usedMemory / 1024.0} KB");
            Console.WriteLine($"RAM utilizzata (MB): {usedMemory / (1024.0 * 1024.0)} MB");

            return result;
        }
    }
}