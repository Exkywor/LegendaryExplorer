using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LegendaryExplorerCore.Gammtek.Collections
{
    public class ListComparer<T>(IEqualityComparer<T> elementComparer = null) : EqualityComparer<List<T>>
    {
        private readonly IEqualityComparer<T> ElementComparer = elementComparer ?? EqualityComparer<T>.Default;

        public override bool Equals(List<T> x, List<T> y) => Equals(x, y, ElementComparer);

        public override int GetHashCode(List<T> obj) => obj?.GetHashCode() ?? 0;

        public static bool Equals(List<T> x, List<T> y, IEqualityComparer<T> elementComparer)
        {
            if (x is null)
            {
                return y is null;
            }
            if (y is null)
            {
                return false;
            }
            if (x.Count != y.Count)
            {
                return false;
            }
            foreach ((T xElement, T yElement) in x.Zip(y))
            {
                if (!elementComparer.Equals(xElement, yElement))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
