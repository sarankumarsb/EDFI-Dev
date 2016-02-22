using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Common
{
    public static class HashSetExtensions
    {
        public static HashSet<T> AddRange<T>(this HashSet<T> hashSet, IEnumerable<T> items)
        {
            foreach (var item in items)
                hashSet.Add(item);

            return hashSet;
        }
    }
}
