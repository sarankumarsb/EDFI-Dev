// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.Dashboards.Testing
{
	public class CallRecorder<T> : IEnumerable<T>
		where T : new()
	{
		private readonly List<T> calls = new List<T>();

		public T this[int index]
		{
			get
			{
				if (index < calls.Count)
					return calls[index];

				if (index == calls.Count)
				{
					var item = new T();
					calls.Add(item);
					return item;
				}

				throw new ArgumentOutOfRangeException("index", "Index must be <= current list count.");
			}
		}

		public int Count
		{
			get { return calls.Count(); }
		}


		public IEnumerator<T> GetEnumerator()
		{
			return calls.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
