namespace Orbit.Shared.Addressable;

public abstract class Key
{
    public static Key Of(object value)
    {
        return value switch
        {
            string str => new StringKey(str),
            int intValue => new Int32Key(intValue),
            long longValue => new Int64Key(longValue),
            NoKey => new NoKey(),
            _ => throw new ArgumentException($"No key type for '{value.GetType().Name}'")
        };
    }


    public static Key None()
    {
        return new NoKey();
    }

    public class NoKey : Key
    {
        public override string ToString()
        {
            return $"{nameof(NoKey)}()";
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is NoKey)
            {
                return true;
            }

            return false;
        }

        public override int GetHashCode()
        {
            return 194837;
        }
    }

    public class StringKey : Key
    {
        public StringKey(string key)
        {
            this.Key = key;
        }

        public string Key { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is StringKey sk)
            {
                return sk.Key == Key;
            }

            return false;
        }

        public override string ToString()
        {
            return Key;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }

    public class Int32Key : Key
    {
        public Int32Key(int key)
        {
            this.Key = key;
        }

        public int Key { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is Int32Key sk)
            {
                return sk.Key == Key;
            }

            return false;
        }

        public override string ToString()
        {
            return Key.ToString();
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }

    public class Int64Key : Key
    {
        public Int64Key(long key)
        {
            this.Key = key;
        }

        public long Key { get; set; }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is Int64Key sk)
            {
                return sk.Key == Key;
            }

            return false;
        }

        public override string ToString()
        {
            return Key.ToString();
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}