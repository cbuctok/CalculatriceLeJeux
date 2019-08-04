namespace CalculatriceLeJeux
{
    using CalculatriceLeJeux.Models;
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal static class Program
    {
        private const StringComparison _icic = StringComparison.InvariantCultureIgnoreCase;

        private static void Main(string[] args)
        {
            var start = int.Parse(args[0]);
            var moves = int.Parse(args[1]);

            var goals = new List<int>();
            var ops = new List<Operation>();

            for (int i = 2; i < args.Length; i++)
            {
                var arg = args[i];
                if (arg.StartsWith('?') || arg.StartsWith("o"))
                {
                    goals.Add(int.Parse(arg.Remove(0, 1)));
                }
                else
                {
                    var operation = CheckOperation(arg);
                    if (operation is Delete)
                    {
                        foreach (Position position in Enum.GetValues(typeof(Position)))
                        {
                            for (var n = 0; n < 10; n++)
                                ops.Add(new Delete($"{n}", position));
                        }
                    }
                    else
                    {
                        ops.Add(operation);
                    }
                }
            }

            var successfulOperations = new ConcurrentDictionary<int, ConcurrentBag<List<Operation>>>();

            foreach (var goal in goals)
            {
                successfulOperations.TryAdd(goal, new ConcurrentBag<List<Operation>>());
                Parallel.ForEach(GetPermutationsWithRept(ops, moves), combo =>
                {
                    var x = start;
                    var listOfCompletedOperations = new List<Operation>();
                    foreach (var op in combo)
                    {
                        try
                        {
                            x = op.Do(x);
                            listOfCompletedOperations.Add(op);
                            if (x == goal)
                                successfulOperations[goal].Add(listOfCompletedOperations);
                        }
                        catch (InvalidOperationException)
                        {
                            break;
                        }
                    }
                });
            }

            var results = new Dictionary<int, ConcurrentBag<List<Operation>>>(successfulOperations);

            // TODO: better output
            foreach (var (res, opers) in results.SelectMany(res => res.Value.Select(opers => (res, opers)).OrderBy(o => o.res.Key).ThenBy(o => o.opers.Count)))
                Console.WriteLine($"{res.Key}: {string.Join(", ", opers)}");
        }

        private static Operation CheckOperation(string input)
        {
            if (input.StartsWith("Add", _icic))
            {
                return new Add(int.Parse(input.Remove(0, 3)));
            }
            else if (input.StartsWith("CUT", _icic))
            {
                return new Cut(int.Parse(input.Remove(0, 3)));
            }
            else if (input.StartsWith("delete", _icic))
            {
                return new Delete();
            }
            else if (input.StartsWith("/", _icic))
            {
                return new Division(int.Parse(input.Remove(0, 1)));
            }
            else if (input.StartsWith("-", _icic))
            {
                return new Minus(int.Parse(input.Remove(0, 1)));
            }
            else if (input.StartsWith("*", _icic) || input.StartsWith("x", _icic))
            {
                return new Multiplication(int.Parse(input.Remove(0, 1)));
            }
            else if (input.StartsWith("+", _icic))
            {
                return new Plus(int.Parse(input.Remove(0, 1)));
            }
            else if (input.Equals("Remove", _icic) || input.Equals("<<", _icic))
            {
                return new Remove();
            }
            else if (input.Contains("=>", _icic))
            {
                var s = input.Split('=');
                return new Replace(s[0], s[1].Remove(0, 1));
            }
            else if (input.Equals("reverse", _icic))
            {
            }
            throw new InvalidOperationException($"Unknown operation {input}");
        }

        private static IEnumerable<IEnumerable<T>> GetPermutationsWithRept<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutationsWithRept(list, length - 1).SelectMany(_ => list, (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}