// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace EdFi.Dashboards.Presentation.Architecture.CastleWindsor
{
    /// <summary>
    /// Registers the controller classes (for the Model-View-Controller implementation) with the Castle Windsor Inversion of Control container.
    /// </summary>
    public class ControllerInstaller<TMarker> : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Find all the controllers in the assembly
            var allControllerTypes =
                (from t in Assembly.GetAssembly(typeof(TMarker)).GetTypes()
                 where typeof(IController).IsAssignableFrom(t)
                 select t)
                 .ToList();

            // Iterate through controllers to be registered
            foreach (var controllerType in allControllerTypes)
            {
                //Don't register abstract types, they can't be fufilled.
                if (controllerType.IsAbstract)
                    continue;

                // Register the controller
                container.Register(
                    Component
                        .For(controllerType)
                        .LifeStyle.Transient);
            }
        }
    }
}
