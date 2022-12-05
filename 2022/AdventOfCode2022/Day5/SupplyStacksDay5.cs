using System.Collections;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace AdventOfCode2022.Day5;

public partial class SupplyStacksDay5
{
    public static async Task<(string Part1, string Part2)> SolveAsync()
    {
        var input = await File.ReadAllTextAsync(
            Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Day5", "input.txt"));

        var split = input.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        
        var unparsedSupplyStacks = split.TakeWhile(x => !x.Contains("move")).ToArray();
        var unparsedInstructions = split.Skip(unparsedSupplyStacks.Length);

        var supplyStacks1 = new SupplyStacks(unparsedSupplyStacks);
        var supplyStacks2 = new SupplyStacks(supplyStacks1);
        var instructions = new CraneInstructionSet(unparsedInstructions);

        ICrane crane9000 = new CrateMover9000();
        crane9000.MoveCrates(ref supplyStacks1, ref instructions);
        
        ICrane crane9001 = new CrateMover9001();
        crane9001.MoveCrates(ref supplyStacks2, ref instructions);

        var part1 = string.Join(string.Empty, supplyStacks1.OrderBy(x => x.Key).Select(x => x.Value.Peek().Id));
        var part2 = string.Join(string.Empty, supplyStacks2.OrderBy(x => x.Key).Select(x => x.Value.Peek().Id));
        return (part1, part2);
    }

    public interface ICrane
    {
        void MoveCrates(ref SupplyStacks supplyStacks, ref CraneInstructionSet instructionSet);
    }
    
    public sealed class CrateMover9001 : ICrane
    {
        public void MoveCrates(ref SupplyStacks supplyStacks, ref CraneInstructionSet instructionSet)
        {
            foreach (var instruction in instructionSet)
            {
                if (!supplyStacks.TryGetValue(instruction.OriginStackId, out var originStack))
                {
                    Console.WriteLine(JsonSerializer.Serialize(instruction));
                    throw new InvalidOperationException();
                }

                if (!supplyStacks.TryGetValue(instruction.DestinationStackId, out var destinationStack))
                {
                    Console.WriteLine(JsonSerializer.Serialize(instruction));
                    throw new InvalidOperationException();
                }

                var popped = new List<SupplyCrate>();
                for (var i = 0; i < instruction.CrateCount; i++)
                {
                    if (originStack.TryPop(out var crate))
                        popped.Add(crate);
                }

                popped.Reverse();
                foreach (var crate in popped)
                    destinationStack.Push(crate);
            }
        }
    }
    
    public sealed class CrateMover9000 : ICrane
    {
        public void MoveCrates(ref SupplyStacks supplyStacks, ref CraneInstructionSet instructionSet)
        {
            foreach (var instruction in instructionSet)
            {
                if (!supplyStacks.TryGetValue(instruction.OriginStackId, out var originStack))
                {
                    Console.WriteLine(JsonSerializer.Serialize(instruction));
                    throw new InvalidOperationException();
                }

                if (!supplyStacks.TryGetValue(instruction.DestinationStackId, out var destinationStack))
                {
                    Console.WriteLine(JsonSerializer.Serialize(instruction));
                    throw new InvalidOperationException();
                }

                for (var i = 0; i < instruction.CrateCount; i++)
                {
                    if (originStack.TryPop(out var crate))
                        destinationStack.Push(crate);
                }
            }
        }
    }
    
    public sealed partial class SupplyStacks : Dictionary<int,SupplyStack>
    {
        [GeneratedRegex(@"^[0-9\s\[\]]*$")]
        private static partial Regex ExtractIndexLineRegex();

        public SupplyStacks(SupplyStacks stacks)
        {
            foreach (var (_, stack) in stacks)
            {
                TryAdd(stack.Id, new SupplyStack(stack) );
            }
        }
        
        public SupplyStacks(IEnumerable<string> lines)
        {
            var enumerated = lines.ToList();

            // init parse
            var indexLine = enumerated.First(x => ExtractIndexLineRegex().IsMatch(x));
            var indexes = indexLine.Where(char.IsNumber).ToDictionary(key => indexLine.IndexOf(key), value => value - '0');
            var initialStackState = enumerated.TakeWhile(x => x.Contains('['));

            // actual parse
            var stacks = indexes.ToDictionary(x => x.Key, x => new SupplyStack(x.Value));
            
            foreach (var line in initialStackState.Reverse().ToArray())
            {
                for (var i = 0; i < line.Length; i++)
                {
                    if (!char.IsLetter(line[i]))
                        continue;
                    
                    stacks[i].Push(new SupplyCrate(line[i]) );
                }
            }
            
            foreach (var (_, stack) in stacks)
            {
                TryAdd(stack.Id, stack);
            }
        }
        
    }
    
    public readonly partial struct CraneInstructionSet : IEnumerable<CraneInstruction>
    {
        private readonly IEnumerable<CraneInstruction> _underlyingEnumerable;

        [GeneratedRegex(@"\d+")]
        private static partial Regex ExtractNumbersRegex();
        
        public CraneInstructionSet(IEnumerable<string> unparsedInstructions)
        {
            var actual = new List<CraneInstruction>();
            foreach (var line in unparsedInstructions)
            {
                var matches = ExtractNumbersRegex().Matches(line);
                actual.Add(new CraneInstruction
                {
                    CrateCount = int.Parse(matches[0].Value), 
                    OriginStackId = int.Parse(matches[1].Value), 
                    DestinationStackId = int.Parse(matches[2].Value)
                });
            }
            _underlyingEnumerable = actual;
        }

        public IEnumerator<CraneInstruction> GetEnumerator()
            => _underlyingEnumerable.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => _underlyingEnumerable.GetEnumerator();
    }

    public readonly struct CraneInstruction
    {
        public CraneInstruction(int originStackId, int destinationStackId, int crateCount)
        {
            OriginStackId = originStackId;
            DestinationStackId = destinationStackId;
            CrateCount = crateCount;
        }

        public int OriginStackId { get; init; }
        public int DestinationStackId { get; init; }
        public int CrateCount { get; init; }
    }
    
    public readonly struct SupplyCrate
    {
        public SupplyCrate(char id)
        {
            Id = id;
        }
        
        public char Id { get; init; }
    }
    
    public sealed class SupplyStack : Stack<SupplyCrate>
    {
        public SupplyStack(int id)
        {
            Id = id;
        }
        
        public SupplyStack(SupplyStack stack)
        {
            Id = stack.Id;
            
            foreach (var crate in stack.Reverse())
            {
                Push(crate);
            }
        }
        
        public int Id { get; init; }
    }
}
