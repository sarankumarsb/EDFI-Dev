// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Common
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
                action(item);

            return source;
        }

        //public static int IndexOf<T>(this IEnumerable<T> source, T item)
        //{
        //    int i = -1;

        //    foreach (var member in source)
        //    {
        //        i++;

        //        if (member.Equals(item))
        //            return i;
        //    }

        //    return -1;
        //}

        //public static int IndexOfFirst<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        //{
        //    int i = -1;

        //    foreach (var member in source)
        //    {
        //        i++;

        //        if (predicate(member))
        //            return i;
        //    }

        //    return -1;
        //}
    }
}
