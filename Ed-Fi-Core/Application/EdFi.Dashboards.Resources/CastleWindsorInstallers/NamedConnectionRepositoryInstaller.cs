// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using EdFi.Dashboards.Data.Repository;

namespace EdFi.Dashboards.Resources.CastleWindsorInstallers
{
    public class NamedConnectionRepositoryInstaller<T> : RepositoryInstaller<T>
    {
        public string DatabaseDataProvider { get; set; }

        protected override void RegisterGenericRepositories(IWindsorContainer container, IEnumerable<Type> entityTypes)
        {
            foreach (var entityType in entityTypes)
                if (!container.Kernel.HasComponent(CreateGenericType(typeof(IRepository<>), entityType)))
                {
                    // Get base entity type (will be same as entity type unless entity type is an extended type)
                    var baseEntityType = entityType.GetBaseEntityTypeForServiceRegistration();

                    container.Register(
                        Component
                            .For(CreateGenericType(typeof(IRepository<>), baseEntityType))
                            .ImplementedBy(CreateGenericType(typeof(Repository<>), entityType))
                            .DependsOn(Dependency.OnComponent("subsonicDataProviderProvider", DatabaseDataProvider)));
                }
        }
    }
}
