using System;
using System.Linq;
using System.Reflection;
using RazorGenerator.Mvc;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.Core
{
    public class RazorGeneratorEngine
    {
        private const string MetricTemplateLocations = "~/Views/MetricTemplates/Shared/{0}.cshtml";
        private const string PartialDetailLocations = "~/Views/Detail/{0}.cshtml";
        private const string AreaDetailLocations = "~/Areas/{2}/Views/Detail/{1}/{0}.cshtml";

        public PrecompiledMvcEngine Create<T>() 
            where T : class
        {
            return Create(typeof(T));
        }

        public PrecompiledMvcEngine Create(Assembly assembly)
        {
            var engine = new PrecompiledMvcEngine(assembly)
            {
                PreemptPhysicalFiles = true,
                UsePhysicalViewsIfNewer = false
            };

            engine.PartialViewLocationFormats = engine.PartialViewLocationFormats.Concat(new[] { MetricTemplateLocations, PartialDetailLocations }).ToArray();
            engine.AreaViewLocationFormats = engine.AreaViewLocationFormats.Concat(new[] { AreaDetailLocations }).ToArray();

            return engine;
        }

        public PrecompiledMvcEngine Create(Type assemblyMarkerType)
        {
            return Create(assemblyMarkerType.Assembly);
        }
    }
}