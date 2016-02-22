// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.Infrastructure
{
	public interface IConfigSectionProvider
	{
		object GetSection(string sectionName);
	}
}
