// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using EdFi.Dashboards.Infrastructure.Data;
using SubSonic.Repository;

namespace EdFi.Dashboards.Data.Repository
{
    /// <summary>
    /// Provides read and write data access to the data entity of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the data entity.</typeparam>
    public interface IPersistingRepository<T> : IRepository<T> where T : class, new()
    {
        void Add(T entity);
        void Save(T entity);
        void Save(T entity, out bool created);
        void Delete(T entity);
        int Delete(Expression<Func<T, bool>> criteria);
    }

    /// <summary>
    /// Provides data access to the data entity of type <typeparamref name="T"/> through SubSonic's <see cref="SimpleRepository"/> class.
    /// </summary>
    /// <typeparam name="T">The type of the data entity.</typeparam>
    public class PersistingRepository<T> : IPersistingRepository<T>
        where T : class, new()
    {
        private readonly IDbConnectionStringSelector dbConnectionStringSelector;

        /// <summary>
        /// Initializes a new instance of the <see cref="PersistingRepository{T}" /> class using the supplied database connection name provider.
        /// </summary>
        /// <param name="dbConnectionStringSelector">The provider to use to obtain the name of the connection string that should be used from the configuration file.</param>
        public PersistingRepository(IDbConnectionStringSelector dbConnectionStringSelector)
        {
            this.dbConnectionStringSelector = dbConnectionStringSelector;
        }

        /// <summary>
        /// Gets a SubSonic <see cref="SimpleRepository"/> instance using an appropriate database connection.
        /// </summary>
        private SimpleRepository SimpleRepository
        {
            get
            {
                string connectionStringName = dbConnectionStringSelector.GetConnectionStringName();

                return new SimpleRepository(connectionStringName);
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

        /// <summary>
        /// Adds or updates the provided entity.
        /// </summary>
        /// <param name="entity">The entity to be saved.</param>
        public void Save(T entity)
        {
            bool ignored;
            Save(entity, out ignored);
        }

        public void Add(T entity)
        {
            SimpleRepository.Add(entity);
        }

        /// <summary>
        /// Adds or updates the provided entity, indicating whether a new entity was created.
        /// </summary>
        /// <param name="entity">The entity to be saved.</param>
        /// <param name="created">Indicates whether a new entity was created by the save operation.</param>
        public void Save(T entity, out bool created)
        {
            if (entity == null)
                throw new ArgumentNullException("entity cannot be null.");

            if (IsNewEntity(entity))
            {
                SimpleRepository.Add(entity);
                created = true;
            }
            else
            {
                SimpleRepository.Update(entity);
                created = false;
            }
        }

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">The entity to be deleted.</param>
        public void Delete(T entity)
        {
            SimpleRepository.Delete<T>(entity);
        }

        /// <summary>
        /// Deletes multiple entities based on the criteria provided.
        /// </summary>
        /// <param name="criteria">A predicate expression using the entity's properties to identify what should be deleted.</param>
        /// <returns>The number of entities deleted.</returns>
        public int Delete(Expression<Func<T, bool>> criteria)
        {
            return SimpleRepository.DeleteMany(criteria);
        }

        #region Support functions

        private static bool IsNewEntity(T entity)
        {
            var keyInfo = GetKeyInformation();

            // Get the current entity's key value (Item1 is PropertyInfo of key property)
            var keyValue = keyInfo.Item1.GetValue(entity, null);

            // Compare it to the default value, and return true if the key value is still the default value (Item2 is the default value for the key's type)
            return keyValue.Equals(keyInfo.Item2);
        }

        // Shared "registry" of entity types and their key properties and default value for the key's type.
        private static readonly ConcurrentDictionary<Type, Tuple<PropertyInfo, object>> keyInformationByEntityType = new ConcurrentDictionary<Type, Tuple<PropertyInfo, object>>();

        private static Tuple<PropertyInfo, object> GetKeyInformation()
        {
            Tuple<PropertyInfo, object> tuple;

            Type entityType = typeof(T);

            if (!keyInformationByEntityType.TryGetValue(entityType, out tuple))
            {
                var keyNameMethod = entityType.GetMethod("GetKeyColumn", BindingFlags.Public | BindingFlags.Static);

                if (keyNameMethod == null)
                    throw new ArgumentException(
                        string.Format("Cannot save SubSonic data entity of type '{0}' because it does not have a static GetKeyColumn method.", entityType.Name));

                string keyPropertyName = (string)keyNameMethod.Invoke(null, null);
                var keyProperty = entityType.GetProperty(keyPropertyName);
                var defaultKeyValue = GetDefault(keyProperty.PropertyType);

                tuple = new Tuple<PropertyInfo, object>(keyProperty, defaultKeyValue);

                // in case the key has been added in the time it took us to calculate the key
                // we'll try one more time to get the entry and if it isn't there add the value we've calculated
                tuple = keyInformationByEntityType.GetOrAdd(entityType, tuple);
            }

            return tuple;
        }

        /// <summary>
        /// Gets the default value for the type specified.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> for which the default value should be obtained.</param>
        /// <returns>The default value.</returns>
        private static object GetDefault(Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);

            return null;
        }

        #endregion
    }
}
