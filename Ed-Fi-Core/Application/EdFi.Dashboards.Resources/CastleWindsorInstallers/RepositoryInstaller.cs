// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Common.Utilities;

namespace EdFi.Dashboards.Resources.CastleWindsorInstallers
{
    public class RepositoryInstaller<T> : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var genericType = typeof(T);

            var allEntityTypes =
                (from t in genericType.Assembly.GetTypes()
                 where t.IsPublic
                       && !t.IsInterface
                       && t.Namespace != null && t.Namespace.EndsWith(".Entities")
                 select t)
                 .ToList();
            
            RegisterGenericRepositories(container, allEntityTypes);
        }

        protected virtual void RegisterGenericRepositories(IWindsorContainer container, IEnumerable<Type> entityTypes)
        {
            foreach (var entityType in entityTypes)
                if (!container.Kernel.HasComponent(CreateGenericType(typeof(IRepository<>), entityType)))
                {
                    // Get base entity type (will be same as entity type unless entity type is an extended type)
                    var baseEntityType = entityType.GetBaseEntityTypeForServiceRegistration();

                    var componentRegistration = Component.For(CreateGenericType(typeof (IRepository<>), baseEntityType))
                        .ImplementedBy(CreateGenericType(typeof (Repository<>), entityType));

                    if (baseEntityType != entityType)
                        componentRegistration.Forward(CreateGenericType(typeof (IRepository<>), entityType));
                        
                    container.Register(componentRegistration);
                }
        }

        protected Type CreateGenericType(Type generic, Type innerType)
        {
            return generic.MakeGenericType(new[] { innerType });
        }
    }
}
