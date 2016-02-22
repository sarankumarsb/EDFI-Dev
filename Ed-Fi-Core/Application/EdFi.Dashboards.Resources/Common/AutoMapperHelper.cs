using System;
using AutoMapper;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;

namespace EdFi.Dashboards.Resources.Common
{
    public static class AutoMapperHelper
    {
        public static void EnsureMapping<TEntityBase, TModelBase, TModelRuntime>
            (IRepository<TEntityBase> repository,
            params System.Linq.Expressions.Expression<Func<TModelBase, object>>[] ignores)
        {
            EnsureMapping<TEntityBase, TModelBase, TModelRuntime>(repository, mapping => mapping, ignores);
        }

        //Do not change to return the IMappingExpression<TEntityBase, TModelBase> because of order of execution look at comment on line 28.
        public static void EnsureMapping<TEntityBase, TModelBase, TModelRuntime>
            (IRepository<TEntityBase> repository,
            Func<IMappingExpression<TEntityBase, TModelBase>, IMappingExpression<TEntityBase, TModelBase>> mappingOptions,
            params System.Linq.Expressions.Expression<Func<TModelBase, object>>[] ignores)
        {
            var runtimeDataEntityType = repository.GetEntityType();
            if (runtimeDataEntityType != null && Mapper.FindTypeMapFor(runtimeDataEntityType, typeof(TModelRuntime)) == null)
            {
                var mapping = Mapper.CreateMap<TEntityBase, TModelBase>();

                //We have to do this here if not the .Include() will not get the .ForMember and apply it to the extended model. 
                mapping = mappingOptions(mapping);

                //Apply the ignores.
                foreach (var expression in ignores)
                    mapping = mapping.ForMember(expression, conf => conf.Ignore());

                mapping.Include(runtimeDataEntityType, typeof(TModelRuntime));

                Mapper.CreateMap(runtimeDataEntityType, typeof(TModelRuntime));

            }
        }
    }
}
