// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.Resources.Security
{
	/// <summary>
	/// 
	/// </summary>
	/// <remarks>There is the capability to restrict access based on the verb "Get","Post".</remarks>
	public interface IMetricActionUrlAuthorizationProvider
	{
		bool CurrentUserHasAccessToPath(string virtualPath, int educationalOrganization);
	}
}
