namespace CalculatriceLeJeux
{
    using CalculatriceLeJeux.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    internal static class Program
    {
        private static void Main(string[] args)
        {
            const int start = 303;
            const int result = 100;
            const int tries = 3;

            var ops = new List<Operation>
            {
                new SortAsc(),
                new Plus(1),
                new Times(3)
            };

            Parallel.ForEach(GetPermutationsWithRept(ops, tries), combo =>
            {
                var x = start;
                foreach (var op in combo)
                {
                    x = op.Do(x);
                    if (x == result)
                        Console.WriteLine(string.Join(", ", combo));
                }
            });
        }

        private static IEnumerable<IEnumerable<T>> GetPermutationsWithRept<T>(IEnumerable<T> list, int length)
        {
            if (length == 1) return list.Select(t => new T[] { t });
            return GetPermutationsWithRept(list, length - 1).SelectMany(_ => list, (t1, t2) => t1.Concat(new T[] { t2 }));
        }
    }
}