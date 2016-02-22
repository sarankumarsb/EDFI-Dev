// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.Common.Utilities
{
	public static class ReflectionUtility
	{
		public static object GetDefault(Type type)
		{
			if (type.IsValueType)
				return Activator.CreateInstance(type);

			return null;
		}
	}
}
