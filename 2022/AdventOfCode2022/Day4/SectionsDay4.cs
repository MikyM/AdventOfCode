namespace AdventOfCode2022.Day4;

public class SectionsDay4
{
    public static async Task<int> SolvePart1Async()
        => (await File.ReadAllTextAsync(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day4", "input.txt")))
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(x =>
                x.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(y => y.SelectMany(z => z.Split('-', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)).ToArray())
            .Count(x => ((x[0] >= x[2] && x[1] <= x[3]) || (x[2] >= x[0] && x[3] <= x[1])));
    
    
    public static async Task<int> SolvePart2Async()
        => (await File.ReadAllTextAsync(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day4", "input.txt")))
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(x =>
                x.Split(',', StringSplitOptions.RemoveEmptyEntries))
            .Select(y => y.SelectMany(z => z.Split('-', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse)).ToArray())
            .Count(x => x[0] <= x[3] && x[2] <= x[1]);
}
