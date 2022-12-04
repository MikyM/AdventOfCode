namespace AdventOfCode2022.Day3;

public class RucksacksDay3
{
    public static async Task<int> SolvePart1Async()
        => (await File.ReadAllTextAsync(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day3", "input.txt")))
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(x =>
            {
                var half = x.Length / 2;
                var compartments = new[] { x[..(half)], x.Substring(half, half) };
                var common = compartments[0].Intersect(compartments[1]).First();
                return char.IsUpper(common) ? (common & 0b11111) + 26 : common & 0b11111;
            })
            .Sum();
    
    public static async Task<int> SolvePart2Async()
        => (await File.ReadAllTextAsync(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day3", "input.txt")))
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Chunk(3)
            .Select(x =>
            {
                var common = x[0].Intersect(x[1]).Intersect(x[2]).First();
                return char.IsUpper(common) ? (common & 0b11111) + 26 : common & 0b11111;
            })
            .Sum();
}
