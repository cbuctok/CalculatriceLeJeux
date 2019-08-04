namespace CalculatriceLeJeux.Models
{
    using System;
    using System.Linq;

    public interface Operation
    {
        int Do(int x);
    }

    public class Multiplication : Operation
    {
        public readonly int What;

        public Multiplication(int what)
        {
            What = what;
        }

        public int Do(int x)
        {
            return x * What;
        }

        public override string ToString()
        {
            return $"{base.ToString().Split('.').Last()}({What})";
        }
    }

    public class Division : Operation
    {
        public readonly int What;

        public Division(int what)
        {
            What = what;
        }

        public int Do(int x)
        {
            if (x % What != 0) throw new InvalidOperationException("Has remainder");
            return x / What;
        }

        public override string ToString()
        {
            return $"{base.ToString().Split('.').Last()}({What})";
        }
    }

    public class Plus : Operation
    {
        public readonly int What;

        public Plus(int what)
        {
            What = what;
        }

        public int Do(int x)
        {
            return x + What;
        }

        public override string ToString()
        {
            return $"{base.ToString().Split('.').Last()}({What})";
        }
    }

    public class Remove : Operation
    {
        public int Do(int x)
        {
            var str = x.ToString();
            if (str.Length < 2) return 0;
            return int.Parse(str.Remove(str.Length - 1));
        }

        public override string ToString()
        {
            return base.ToString().Split('.').Last();
        }
    }

    public class Add : Operation
    {
        public readonly string What;

        public Add(int what)
        {
            What = what.ToString();
        }

        public int Do(int x)
        {
            return int.Parse($"{x}{What}");
        }

        public override string ToString()
        {
            return $"{base.ToString().Split('.').Last()}({What})";
        }
    }

    public class Replace : Operation
    {
        public readonly string What;
        public readonly string With;

        public Replace(string what, string with)
        {
            What = what;
            With = with;
        }

        public int Do(int x)
        {
            return int.Parse(x.ToString().Replace(What, With));
        }

        public override string ToString()
        {
            return $"{base.ToString().Split('.').Last()}({What} => {With})";
        }
    }

    public class SortAsc : Operation
    {
        public int Do(int x)
        {
            return int.Parse(string.Concat(x.ToString().OrderBy(c => c)));
        }

        public override string ToString()
        {
            return base.ToString().Split('.').Last();
        }
    }

    public class SortDes : Operation
    {
        public int Do(int x)
        {
            return int.Parse(string.Concat(x.ToString().OrderByDescending(c => c)));
        }

        public override string ToString()
        {
            return base.ToString().Split('.').Last();
        }
    }
}