// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Testing
{
	public class Call<T1>
	{
		public T1 Arg1 { get; set; }

		public Call() { }

		public Call(T1 arg1)
		{
			Arg1 = arg1;
		}
	}

	public class Call<T1, T2>
	{
		public T1 Arg1 { get; set; }
		public T2 Arg2 { get; set; }

		public Call(){}

		public Call(T1 arg1, T2 arg2)
		{
			Arg1 = arg1;
			Arg2 = arg2;
		}
	}

	public class Call<T1, T2, T3>
	{
		public T1 Arg1 { get; set; }
		public T2 Arg2 { get; set; }
		public T3 Arg3 { get; set; }

		public Call() { }

		public Call(T1 arg1, T2 arg2, T3 arg3)
		{
			Arg1 = arg1;
			Arg2 = arg2;
			Arg3 = arg3;
		}
	}

	public class Call<T1, T2, T3, T4>
	{
		public T1 Arg1 { get; set; }
		public T2 Arg2 { get; set; }
		public T3 Arg3 { get; set; }
		public T4 Arg4 { get; set; }

		public Call() { }

		public Call(T1 arg1, T2 arg2, T3 arg3, T4 arg4)
		{
			Arg1 = arg1;
			Arg2 = arg2;
			Arg3 = arg3;
			Arg4 = arg4;
		}
	}

	public class Call<T1, T2, T3, T4, T5>
	{
		public T1 Arg1 { get; set; }
		public T2 Arg2 { get; set; }
		public T3 Arg3 { get; set; }
		public T4 Arg4 { get; set; }
		public T5 Arg5 { get; set; }

		public Call() { }

		public Call(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
		{
			Arg1 = arg1;
			Arg2 = arg2;
			Arg3 = arg3;
			Arg4 = arg4;
			Arg5 = arg5;
		}
	}

	public class Call<T1, T2, T3, T4, T5, T6>
	{
		public T1 Arg1 { get; set; }
		public T2 Arg2 { get; set; }
		public T3 Arg3 { get; set; }
		public T4 Arg4 { get; set; }
		public T5 Arg5 { get; set; }
		public T6 Arg6 { get; set; }

		public Call() { }

		public Call(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
		{
			Arg1 = arg1;
			Arg2 = arg2;
			Arg3 = arg3;
			Arg4 = arg4;
			Arg5 = arg5;
			Arg6 = arg6;
		}
	}
}
