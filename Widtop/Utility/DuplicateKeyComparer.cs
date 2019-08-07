using System;
using System.Collections.Generic;

namespace Widtop.Utility
{
    public class DuplicateKeyComparer<TKey> : IComparer<TKey> where TKey : IComparable
    {
        public int Compare(TKey x, TKey y)
        {
            var result = x?.CompareTo(y) ?? 0;

            return result == 0 ? 1 : result;
        }
    }
}
