namespace CalculatriceLeJeux.Models
{
    using System;
    using System.Linq;

    public enum Position
    {
        Start,
        End
    }

    public interface Operation
    {
        int Do(int x);
    }

    public class Add : Operation
    {
        public readonly string What;

        public Add(int what) => What = what.ToString();

        public int Do(int x) => int.Parse($"{x}{What}");

        public override string ToString() => $"{base.ToString().Split('.').Last()}({What})";
    }

    public class Cut : Operation
    {
        public readonly string What;

        public Cut(int what) => What = what.ToString();

        public int Do(int x)
        {
            var t = x.ToString().Replace(What, string.Empty);
            if (string.IsNullOrWhiteSpace(t)) return 0;
            return int.Parse(t);
        }

        public override string ToString() => $"{base.ToString().Split('.').Last()}({What})";
    }

    public class Delete : Operation
    {
        public readonly string What;
        public readonly Position Where;

        public Delete()
        {
        }

        public Delete(string what, Position where = Position.Start)
        {
            What = what;
            Where = where;
        }

        public int Do(int x)
        {
            var numberS = x.ToString();
            int k;
            if (Where == Position.Start)
                k = numberS.IndexOf(What);
            else
                k = numberS.LastIndexOf(What);
            if (k < 0) return x;
            var s = numberS.Remove(k, 1);
            if (string.IsNullOrWhiteSpace(s)) return x;
            return int.Parse(s);
        }

        public override string ToString() => $"{base.ToString().Split('.').Last()}({What}, {Where})";
    }

    public class Division : Operation
    {
        public readonly int What;

        public Division(int what) => What = what;

        public int Do(int x)
        {
            if (x % What != 0) return x;
            return x / What;
        }

        public override string ToString() => $"{base.ToString().Split('.').Last()}({What})";
    }

    public class Insert : Operation
    {
        public readonly string What;
        public readonly int Where;

        public Insert(string what)
        {
            What = what;
        }

        public Insert(string what, int where)
        {
            What = what;
            Where = where;
        }

        public int Do(int x)
        {
            var str = x.ToString();
            if (str.Length < Where) return x;
            return int.Parse(str.Insert(Where, What));
        }

        public override string ToString() => $"{base.ToString().Split('.').Last()}({What},{Where})";
    }

    public class Minus : Operation
    {
        public readonly int What;

        public Minus(int what) => What = what;

        public int Do(int x) => x - What;

        public override string ToString() => $"{base.ToString().Split('.').Last()}({What})";
    }

    public class Multiplication : Operation
    {
        public readonly int What;

        public Multiplication(int what) => What = what;

        public int Do(int x) => x * What;

        public override string ToString() => $"{base.ToString().Split('.').Last()}({What})";
    }

    public class Plus : Operation
    {
        public readonly int What;

        public Plus(int what) => What = what;

        public int Do(int x) => x + What;

        public override string ToString() => $"{base.ToString().Split('.').Last()}({What})";
    }

    public class Remove : Operation
    {
        public int Do(int x)
        {
            var str = x.ToString();
            if (str.Length < 2) return 0;
            return int.Parse(str.Remove(str.Length - 1));
        }

        public override string ToString() => base.ToString().Split('.').Last();
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

        public int Do(int x) => int.Parse(x.ToString().Replace(What, With));

        public override string ToString() => $"{base.ToString().Split('.').Last()}({What} => {With})";
    }

    public class Reverse : Operation
    {
        public int Do(int x)
        {
            var str = x.ToString();
            str.Reverse();
            return int.Parse(str);
        }

        public override string ToString() => base.ToString().Split('.').Last();
    }

    public class SortAsc : Operation
    {
        public int Do(int x) => int.Parse(string.Concat(x.ToString().OrderBy(c => c)));

        public override string ToString() => base.ToString().Split('.').Last();
    }

    public class SortDes : Operation
    {
        public int Do(int x) => int.Parse(string.Concat(x.ToString().OrderByDescending(c => c)));

        public override string ToString() => base.ToString().Split('.').Last();
    }

    public class Sum : Operation
    {
        public int Do(int x)
        {
            var str = x.ToString().Trim();
            var result = 0;
            for (int i = 0; i < str.Length; i++)
                result += str[i] - 48;

            return result;
        }

        public override string ToString() => base.ToString().Split('.').Last();
    }
}