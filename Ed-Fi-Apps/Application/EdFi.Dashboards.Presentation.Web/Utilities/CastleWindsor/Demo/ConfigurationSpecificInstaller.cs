// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Linq;
using Castle.Windsor;
using EdFi.Dashboards.Infrastructure.CastleWindsorInstallers;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Images;

namespace EdFi.Dashboards.Presentation.Web.Utilities.CastleWindsor.Demo
{
    /// <summary>
    /// Installs infrastructure services used by this application in the Demo environment
	/// </summary>
	public class ConfigurationSpecificInstaller : ConfigurationSpecificInstallerBase
    {
        //Placeholder for environment specific installation of plugable components. 
        //I.E.: You would use a different ImageProvider depending if you are in Development or Production environments.
	}
}
