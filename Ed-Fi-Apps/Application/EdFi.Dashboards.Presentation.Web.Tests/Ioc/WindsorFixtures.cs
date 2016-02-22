using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Castle.MicroKernel;
using Castle.MicroKernel.Handlers;
using Castle.MicroKernel.Resolvers.SpecializedResolvers;
using Castle.Windsor;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.CastleWindsorInstallers;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Resources;
using NUnit.Framework;
using Component = Castle.MicroKernel.Registration.Component;

namespace EdFi.Dashboards.Presentation.Web.Tests.Ioc
{
    public class WindsorFixtures
    {
        [TestFixture]
        public class When_Building_A_New_Container
        {
            [TestFixtureSetUp]
            public void RunOnceBeforeAny()
            {
                // Initialize configuration providers to use the actual web.config file, linked into this project from main web project
                var configSectionProvider = new ExternalConfigFileSectionProvider("web.config");
                var configValueProvider = new ExternalConfigFileConfigValueProvider(configSectionProvider);

                var containerFactory = new InversionOfControlContainerFactory(
                    configSectionProvider,
                    configValueProvider);

                // Get the container
                Container = containerFactory.CreateContainer(c =>
                    {
                        // Add the array resolver for resolving arrays of services automatically
                        c.Kernel.Resolver.AddSubResolver(new ArrayResolver(c.Kernel));

                        // Make sure for this testing that we're using the config value provider initialized above
                        c.Register(Component.For<IConfigSectionProvider>().Instance(configSectionProvider));
                        c.Register(Component.For<IConfigValueProvider>().Instance(configValueProvider));

                        // Initialize the service locator with the container 
                        // (enabling installers to access container through IoC during registration process)
                        IoC.Initialize(c);
                    });
                // Register the wrapped service locator singleton instance
                Container = Container.Register(
                    Component.For<IServiceLocator>()
                        .Instance(IoC.WrappedServiceLocator));
            }

            protected IWindsorContainer Container;

            [Test]
            public void Verify_windsor_container_can_return_everything_that_is_configured()
            {
                foreach (IHandler handler in Container.Kernel.GetAssignableHandlers(typeof (object)))
                {
                    try
                    {
                        if (handler is DefaultGenericHandler)
                        {
                            //If it's a generic handler, we have to figure out a reasonable set of parameters to pass in,
                            //  so that Resolve can create a closed generic, an give us the type back
                            var list = handler
                                .ComponentModel
                                .Service
                                .GetGenericArguments()
                                .Select(FindAClassThatMeetsTheGenericParameter)
                                .ToList();

                            Container.Resolve(handler.ComponentModel.Service.MakeGenericType(list.ToArray()));
                        }
                        else
                        {
                            Container.Resolve(handler.ComponentModel.Service);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("An exception occurred while testing: " + handler.Service);
                        throw;
                    }
                }
            }

            protected Type FindAClassThatMeetsTheGenericParameter(Type input)
            {
                var genericConstraint = input.GetGenericParameterConstraints().FirstOrDefault();

                //Now find a class that meets this constraint...

                //If there are no constraints, then any object should do, including System.Object.
                if (genericConstraint == null)
                {
                    return typeof (object);
                }

                bool constraintRequiresDefaultConstructor =
                    (input.GenericParameterAttributes & GenericParameterAttributes.DefaultConstructorConstraint) !=
                    GenericParameterAttributes.DefaultConstructorConstraint;
                bool isConstraintConcrete = !genericConstraint.IsAbstract;

                bool typeOfTheConstraintFulfillsTheConstraint = constraintRequiresDefaultConstructor || isConstraintConcrete;
                
                if (typeOfTheConstraintFulfillsTheConstraint)
                {
                    return genericConstraint;
                }

                //But if it's abstract, and the generic requires a default constructor, we have to find a class that implments it.
                //If we've made it this far, then we know that typeOfGenericInterface is an abstract class.  Go find something that implments it...
                
                //Uncomment this and change the code it to use the correct marker interface if you have the State Resource Extensions.
                //var extensionClassThatInheritsFromTheConstraint =
                //    typeof(Marker_EdFi_Dashboards_Extensions_STATE_Resources).Assembly.GetTypes().FirstOrDefault(type => type.IsSubclassOf(genericConstraint));
                //if (extensionClassThatInheritsFromTheConstraint != null)
                //    return extensionClassThatInheritsFromTheConstraint;

                var coreClassThatInheritsFromTheConstraint =
                    typeof(Marker_EdFi_Dashboards_Resources).Assembly.GetTypes().FirstOrDefault(type => type.IsSubclassOf(genericConstraint));
                
                if (coreClassThatInheritsFromTheConstraint != null)
                    return coreClassThatInheritsFromTheConstraint;

                throw new ApplicationException("Unable to find an implementation for " + input);
            }
        }
    }
}
