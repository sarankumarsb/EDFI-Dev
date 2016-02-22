using System.Linq;
using System.Windows.Forms;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Dashboards.Infrastructure.CastleWindsorInstallers;
using EdFi.Dashboards.Presentation.Web.Providers.Metric;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Images.ContentProvider;
using EdFi.Dashboards.Resources.Images.Navigation;
using EdFi.Dashboards.Resources.Plugins;
using EdFi.Dashboards.Resources.Security;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.SecurityTokenService.Authentication.Analytics;
using EdFi.Dashboards.SecurityTokenService.Authentication.Implementations;

namespace EdFi.Dashboards.Presentation.Web.Utilities.CastleWindsor
{
    public class ConfigurationSpecificInstallerBase : Core.Utilities.CastleWindsor.ConfigurationSpecificInstallerBase
    {
        //Placeholder for customizing the plugable components. I.E. Plugin your "CustomFlagProvider"

        protected virtual void RegisterIMetricTemplateVirtualPathsProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IMetricTemplateVirtualPathsProvider>()
                .ImplementedBy<MetricRendering.RazorGeneratorMetricTemplatesPathProvider>());

            container.Register(Component
                .For<IMetricTemplateVirtualPathsProvider>()
                .ImplementedBy<RazorGeneratorMetricTemplatesPathProvider>());
        }

        protected override void RegisterIImageContentProvider(IWindsorContainer container)
        {
            var assemblyTypes = typeof(Marker_EdFi_Dashboards_Resources).Assembly.GetTypes();

            var chainTypes = (from t in assemblyTypes
                              let serviceType = t.GetInterface(typeof(IImageContentProvider).Name)
                              //where serviceType != null && !t.IsAbstract && t.Name.EndsWith("RandomImageContentProvider")
                              where serviceType != null && !t.IsAbstract && t.Name.EndsWith("GenericImageContentProvider")                              
                              select t);

            var chainRegistrar = new ChainOfResponsibilityRegistrar(container);
            chainRegistrar.RegisterChainOf<IImageContentProvider, NullImageProvider>(chainTypes.ToArray(), "ImageContentProviderChain");
        }
        protected override void RegisterISignInRequestMessageProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<ISignInRequestMessageProvider>()
                .ImplementedBy<SecurityTokenServiceSignInRequestMessageProvider>());
        }

        protected override void RegisterIClaimsAuthenticationManagerProvider(IWindsorContainer container)
        {
            //Client side claims enrichment - when IdentityClaimsGetOutputClaimsIdentityProvider for IGetOutputClaimsIdentityProvider
            container.Register(Component
                .For<IClaimsAuthenticationManagerProvider>()
                .ImplementedBy<QEduDashboardClaimsAuthenticationManagerProvider>());
                //.ImplementedBy<DashboardClaimsAuthenticationManagerProvider>());
        }
        
        protected virtual void RegisterIStaffInformationFromEmailProvider(IWindsorContainer container)
        {
            container.Register(Component
                .For<IStaffInformationFromEmailProvider>()
                .ImplementedBy<QEduLoginInformationFromUsernameProvider>());
                //.ImplementedBy<StaffInformationFromEmailProvider>());
        }
    }
}