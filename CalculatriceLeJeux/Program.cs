namespace CalculatriceLeJeux
{
    using CalculatriceLeJeux.Models;
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public static class Program
    {
        private const StringComparison _icic = StringComparison.InvariantCultureIgnoreCase;

        private static readonly Dictionary<string, HashSet<int>> _intCharDic = new Dictionary<string, HashSet<int>>
        {
            ["1"] = new HashSet<int> { 'a', 'b', 'c' },
            ["2"] = new HashSet<int> { 'd', 'e', 'f' },
            ["3"] = new HashSet<int> { 'g', 'h', 'i' },
            ["4"] = new HashSet<int> { 'j', 'k', 'l' },
            ["5"] = new HashSet<int> { 'm', 'n', 'o' },
            ["6"] = new HashSet<int> { 'p', 'q', 'r' },
            ["7"] = new HashSet<int> { 's', 't', 'u' },
            ["8"] = new HashSet<int> { 'v', 'w', 'x' },
            ["9"] = new HashSet<int> { 'y', 'z' }
        };

        public static void ProcessCombination(IEnumerable<Operation> combo, int start, ConcurrentDictionary<int, ConcurrentBag<List<Operation>>> successfulOperations, int goal)
        {
            var x = start;
            var listOfCompletedOperations = new List<Operation>();
            foreach (var op in combo)
            {
                x = op.Do(x);
                listOfCompletedOperations.Add(op);
                if (x == goal)
                    successfulOperations[goal].Add(listOfCompletedOperations);
            }
        }

        private static int CharsToNums(string arg)
        {
            arg = arg.ToLower().Trim().Remove(0, 1);
            var s = string.Empty;
            for (int i = 0; i < arg.Length; i++)
                s += _intCharDic.First(w => w.Value.Contains(arg[i])).Key;

            return int.Parse(s);
        }

        private static Operation CheckOperation(string input)
        {
            if (input.StartsWith("Add", _icic))
            {
                return new Add(int.Parse(input.Remove(0, 3)));
            }
            else if (input.StartsWith("insert", _icic))
            {
                return new Insert(input.Remove(0, 6));
            }
            else if (input.StartsWith("CUT", _icic))
            {
                return new Cut(int.Parse(input.Remove(0, 3)));
            }
            else if (input.Equals("delete", _icic))
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
            else if (input.Equals("sum", _icic))
            {
                return new Sum();
            }
            else if (input.Equals("reverse", _icic))
            {
                return new Reverse();
            }
            throw new InvalidOperationException($"Unknown operation {input}");
        }

        private static IEnumerable<IEnumerable<T>> GetPermutationsWithRept<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutationsWithRept(list, length - 1).SelectMany(_ => list, (t1, t2) => t1.Concat(new T[] { t2 }));
        }

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
                    if (int.TryParse(arg.Remove(0, 1), out int num))
                        goals.Add(num);
                    else
                        goals.Add(CharsToNums(arg));
                }
                else
                {
                    var operation = CheckOperation(arg);
                    if (operation is Delete)
                    {
                        IList list = Enum.GetValues(typeof(Position));
                        for (int k = 0; k < list.Count; k++)
                        {
                            for (var n = 0; n < 10; n++)
                                ops.Add(new Delete($"{n}", (Position)list[k]));
                        }
                    }
                    else if (operation is Insert insert)
                    {
                        for (var n = 0; n < 5; n++)
                            ops.Add(new Insert(insert.What, n));
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
                Parallel.ForEach(GetPermutationsWithRept(ops, moves), combo => ProcessCombination(combo, start, successfulOperations, goal));
            }

            var results = new Dictionary<int, ConcurrentBag<List<Operation>>>(successfulOperations);

            foreach (var (res, opers) in results.SelectMany(res => res.Value.Select(opers => (res, opers)).OrderBy(o => o.res.Key).ThenBy(o => o.opers.Count)))
                Console.WriteLine($"{res.Key}: {string.Join(", ", opers)}");

            Console.WriteLine("****");
            foreach (var res in results.Where(s => s.Value.Count != 0))
                Console.WriteLine($"{res.Key}: {string.Join(", ", res.Value.FirstOrDefault())}");
        }
    }
}