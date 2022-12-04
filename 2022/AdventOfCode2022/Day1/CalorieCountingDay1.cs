namespace AdventOfCode2022.Day1;

public class CalorieCountingDay1
{
    public static async Task<int> SolvePart1Async()
        => (await File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day1", "input.txt")))
            .Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(x =>
                x.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .Sum()
            ).Max();

    public static async Task<int> SolvePart2Async()
        => (await File.ReadAllTextAsync(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day1", "input.txt")))
            .Split(Environment.NewLine + Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Select(x =>
                x.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
                    .Select(int.Parse)
                    .Sum()
            ).OrderDescending().Take(3).Sum();
}
