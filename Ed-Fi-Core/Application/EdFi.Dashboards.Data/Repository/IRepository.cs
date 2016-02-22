// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Providers;
using EdFi.Dashboards.Infrastructure.Data;
using SubSonic.Repository;
using log4net;

namespace EdFi.Dashboards.Data.Repository
{
    /// <summary>
    /// Provides data access to the data entity of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the data entity.</typeparam>
    public interface IRepository<out T>
    {
        /// <summary>
        /// Provides <see cref="IQueryable"/> access to the data entity of type <typeparamref name="T"/>.
        /// </summary>
        /// <returns>An IQueryable of data entities.</returns>
        IQueryable<T> GetAll();
    }

    /// <summary>
    /// Helper methods for <see cref="IRepository{T}"/>
    /// </summary>
    public static class RepositoryExtensions
    {
        private static Dictionary<Type, Type> entityTypesByRepositoryType = new Dictionary<Type, Type>();

        private static IEnumerable<Type> _extendedTypes;
        public static IEnumerable<Type> ExtendedTypes {
            get
            {
                if (_extendedTypes == null || _extendedTypes.Any()) 
                    _extendedTypes = AppDomain.CurrentDomain.GetAssemblies()
                                 .Where(a => a.FullName.StartsWith("EdFi.Dashboards.Extensions"))
                                 .SelectMany(a => a.GetTypes());

                return _extendedTypes;
            }
        }

        /// <summary>
        /// Returns the run-time type that the <see cref="IRepository{T}"/> will return.
        /// </summary>
        /// <typeparam name="T">The entity type exposed by the repository.</typeparam>
        /// <param name="repository">The repository instance to use to determine the actual runtime entity type (it could 
        /// be a derived entity type if there is an implementation-specific extension).</param>
        /// <returns>The actual entity type exposed by the repository.</returns>
        public static Type GetEntityType<T>(this IRepository<T> repository)
        {
            var repositoryType = repository.GetType();

            Type entityType;

            if (!entityTypesByRepositoryType.TryGetValue(repositoryType, out entityType))
            {
                // This is what normally happens in our production code.
                if (repositoryType.IsGenericType && (repositoryType.GetGenericTypeDefinition() == typeof(Repository<>) || repositoryType.GetGenericTypeDefinition() == typeof(PersistingRepository<>)))
                {
                    entityType = repository.GetType().GetGenericArguments()[0];
                }
                else
                {
                    //If the repository isn't a generic of Repository, then we're probably in a test, so try to find the extension type by convention.
                    var extendedType = ExtendedTypes.FirstOrDefault(t => t.BaseType == typeof (T));

                    //If we cant find a extended type just return T
                    entityType = extendedType ?? typeof(T);
                }

                entityTypesByRepositoryType[repositoryType] = entityType;
            }

            return entityType;
        }
    }

    /// <summary>
    /// Provides data access to the data entity of type <typeparamref name="T"/> through SubSonic's <see cref="SimpleRepository"/> class.
    /// </summary>
    /// <typeparam name="T">The type of the data entity.</typeparam>
    public class Repository<T> : IRepository<T>
        where T : class, new()
    {
        private readonly ISubsonicDataProviderProvider subsonicDataProviderProvider;
        private static ILog log = LogManager.GetLogger(typeof (Repository<T>));

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository"/> class using the supplied database connection name provider.
        /// </summary>
        /// <param name="subsonicDataProviderProvider">The provider to use to obtain the Subsonic IDataProvider.</param>
        public Repository(ISubsonicDataProviderProvider subsonicDataProviderProvider)
        {
            this.subsonicDataProviderProvider = subsonicDataProviderProvider;
        }

        /// <summary>
        /// Gets a SubSonic <see cref="SimpleRepository"/> instance using an appropriate database connection.
        /// </summary>
        private SimpleRepository SimpleRepository
        {
            get
            {
                //Note: to change the way the connection string gets created you can implement your own ISubsonicDataProviderProvider.
                var dataProvider = subsonicDataProviderProvider.GetProvider();

                return new SimpleRepository(dataProvider);
            }
        }

        /// <summary>
        /// Gets an IQueryable of data entities for building queries using the SubSonic Linq provider.
        /// </summary>
        /// <returns>The IQueryable implementation.</returns>
        public IQueryable<T> GetAll()
        {
            return SimpleRepository.All<T>();
        }
    }
}
