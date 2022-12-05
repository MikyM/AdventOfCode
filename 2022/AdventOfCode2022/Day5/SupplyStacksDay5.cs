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
        void MoveCrate(ref SupplyStacks supplyStacks, ref CraneInstruction craneInstruction);
    }
    
    public abstract class Crane : ICrane
    {
        public void MoveCrates(ref SupplyStacks supplyStacks, ref CraneInstructionSet instructionSet)
        {
            foreach (ref var instruction in instructionSet.UnderlyingArray.AsSpan())
            {
                MoveCrate(ref supplyStacks, ref instruction);
            }
        }
        
        public abstract void MoveCrate(ref SupplyStacks supplyStacks, ref CraneInstruction craneInstruction);
    }
    
    public sealed class CrateMover9001 : Crane
    {
        public override void MoveCrate(ref SupplyStacks supplyStacks, ref CraneInstruction instruction)
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

            var popped = new SupplyCrate[instruction.CrateCount];
            for (var i = instruction.CrateCount - 1; i > -1; i--)
            {
                if (originStack.TryPop(out var crate))
                    popped[i] = crate;
            }
            
            foreach (ref var crate in popped.AsSpan())
                destinationStack.Push(crate);
        }
    }
    
    public sealed class CrateMover9000 : Crane
    {
        public override void MoveCrate(ref SupplyStacks supplyStacks, ref CraneInstruction instruction)
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
    
    public readonly partial struct CraneInstructionSet : IEnumerable
    {
        public readonly CraneInstruction[] UnderlyingArray;

        [GeneratedRegex(@"\d+")]
        private static partial Regex ExtractNumbersRegex();
        
        public CraneInstructionSet(IEnumerable<string> unparsedInstructions)
        {
            var enumerated = unparsedInstructions.ToArray();
            UnderlyingArray = new CraneInstruction[enumerated.Length];
            for (var i = 0; i < enumerated.Length; i++)
            {
                var matches = ExtractNumbersRegex().Matches(enumerated[i]);
                UnderlyingArray[i] = new CraneInstruction(
                    int.Parse(matches[1].Value), 
                    int.Parse(matches[2].Value),
                    int.Parse(matches[0].Value)
                    );
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
            => UnderlyingArray.GetEnumerator();
    }

    public readonly struct CraneInstruction
    {
        public CraneInstruction(int originStackId, int destinationStackId, int crateCount)
        {
            OriginStackId = originStackId;
            DestinationStackId = destinationStackId;
            CrateCount = crateCount;
        }

        public readonly int OriginStackId;
        public readonly int DestinationStackId;
        public readonly int CrateCount;
    }
    
    public readonly struct SupplyCrate
    {
        public SupplyCrate(char id)
        {
            Id = id;
        }

        public readonly char Id;
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
