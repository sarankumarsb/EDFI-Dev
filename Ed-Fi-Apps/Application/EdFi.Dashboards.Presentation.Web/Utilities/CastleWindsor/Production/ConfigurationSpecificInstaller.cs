// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Dashboards.Infrastructure.CastleWindsorInstallers;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Images;
using EdFi.Dashboards.Resources.Metric.MetricInstanceSetKeyResolvers;

namespace EdFi.Dashboards.Presentation.Web.Utilities.CastleWindsor.Production
{
    /// <summary>
    /// Installs infrastructure services used by this application in the Production environment.
	/// </summary>
	public class ConfigurationSpecificInstaller : ConfigurationSpecificInstallerBase
    {
        //Placeholder for environment specific installation of plugable components. 
        //I.E.: You would use a different ImageProvider depending if you are in Development or Production environments.
	}
}
