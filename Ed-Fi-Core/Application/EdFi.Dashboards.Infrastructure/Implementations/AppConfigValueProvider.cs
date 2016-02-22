// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Configuration;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
	public class AppConfigValueProvider : IConfigValueProvider
	{
		public string GetValue(string name)
		{
			return ConfigurationManager.AppSettings[name];
		}
	}
}
