// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Configuration;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
	public class AppConfigSectionProvider : IConfigSectionProvider
	{
		public object GetSection(string sectionName)
		{
			return ConfigurationManager.GetSection(sectionName);
		}
	}
}
