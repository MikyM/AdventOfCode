namespace AdventOfCode2022.Day2;

public static class RPSDay2
{
    public static async Task<int> SolvePart1Async()
        => (await File.ReadAllTextAsync(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day2", "input.txt")))
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Map).ToArray()).ToArray()
            .Sum(round => GetPointsForOutcome(round[1], round[0]) + GetPointsForChosenShape(round[1]));
    
    public static async Task<int> SolvePart2Async()
        => (await File.ReadAllTextAsync(
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day2", "input.txt")))
            .Split('\n', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(Map).ToArray()).ToArray()
            .Sum(round =>
            {
                var choice = GetChoiceBasedOnOutcome(round[0], round[1]);
                return GetPointsForOutcome(choice, round[0]) + GetPointsForChosenShape(choice);
            });
    
    private static string GetChoiceBasedOnOutcome(string opponent, string outcome)
        => outcome switch
        {
            "B" => opponent,
            "A" when opponent is "A" => "C",
            "A" when opponent is "B" => "A",
            "A" when opponent is "C" => "B",
            "C" when opponent is "A" => "B",
            "C" when opponent is "B" => "C",
            "C" when opponent is "C" => "A",
            _ => throw new ArgumentOutOfRangeException(nameof(outcome), outcome, null)
        };
    private static int GetPointsForChosenShape(string input)
        => input switch
        {
            "A" => 1,
            "B" => 2,
            "C" => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(input), input, null)
        };
    
    private static string Map(string input)
        => input switch
        {
            "X" => "A",
            "Y" => "B",
            "Z" => "C",
            _ => input
        };
    private static int GetPointsForOutcome(string me, string opponent)
        => me == opponent ? 3 : me switch
        {
            "A" when opponent is "C" => 6,
            "B" when opponent is "A" => 6,
            "C" when opponent is "B" => 6,
            _ => 0
        };
}
