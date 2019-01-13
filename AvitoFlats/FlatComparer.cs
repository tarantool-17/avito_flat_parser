using System.Collections.Generic;

namespace AvitoFlats
{
    public class FlatComparer : IEqualityComparer<Flat>
    {
        public bool Equals(Flat x, Flat y)
        {
            return x.Url.Equals(y?.Url) && x.Address.Equals(y?.Address);
        }

        public int GetHashCode(Flat obj)
        {
            return obj.Address.Length;
        }
    }
}