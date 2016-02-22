// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.Dashboards.Resources.Tests
{
	public static class Extensions
	{
		public static IEnumerable<T> ForEach<T>(this IEnumerable<T> source, Action<T> action)
		{
			foreach (T item in source)
				action(item);

			return source;
		}

        public static string FormatWith(this string formatSpecifier, params object[] parameters)
		{
			return string.Format(formatSpecifier, parameters);
		}

		public static T Second<T>(this IEnumerable<T> source)
		{
			return source.Skip(1).First();
		}
	}
}
