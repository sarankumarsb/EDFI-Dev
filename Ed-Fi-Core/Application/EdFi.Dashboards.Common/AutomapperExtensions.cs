using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoMapper;

namespace EdFi.Dashboards.Common
{
    public static class AutoMapperExtensions
    {
        public static void Include<TSource, TDestination>(this IMappingExpression<TSource, TDestination> mapping, System.Type otherSource, Type otherDestination)
        {
            var includeMethod = mapping.GetType().GetMethod("Include").MakeGenericMethod(otherSource, otherDestination);
            includeMethod.Invoke(mapping, new object[0]);
        }
    }
}
