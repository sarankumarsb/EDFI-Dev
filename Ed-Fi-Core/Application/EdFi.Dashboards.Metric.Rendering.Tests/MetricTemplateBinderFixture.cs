// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Presentation.Web.Providers.Metric;
using NUnit.Framework;
using EdFi.Dashboards.Testing;
using Rhino.Mocks;

namespace EdFi.Dashboards.Metric.Rendering.Tests
{
    [TestFixture]
    public class When_calling_get_template_name_on_the_metric_template_binder : TestFixtureBase
    {
        private IMetricTemplateBinder metricTemplateBinder;
        private IMetricTemplatesProvider metricTemplatesProvider;
        private Dictionary<string, string> renderingContextValues1FullMatch;
        private Dictionary<string, string> renderingContextValues2;
        private Dictionary<string, string> renderingContextValues3NoMatch;
        private Dictionary<string, string> renderingContextValues4;
        private Dictionary<string, string> renderingContextValues5;
        private Dictionary<string, string> renderingContextValues6;
        private string templateNameNoTemplate;
        private string templateName1;
        private string templateName2;
        private string templateName3;
        private string templateName4;
        private string templateName5;
        private string templateName6;
        private string templateName7;

        protected override void EstablishContext()
        {
            templateNameNoTemplate = "~/Views/MetricTemplates/NoTemplate.cshtml";
            templateName1 = "TemplateNameOne";
            templateName2 = "TemplateNameTwo";
            templateName3 = "TemplateNameThree";
            templateName4 = "TemplateNameFour";
            templateName5 = "TemplateNameFive";
            templateName6 = "TemplateNameSix";
            templateName7 = "TemplateNameSeven";

            renderingContextValues1FullMatch = getRenderingContextValues_FullMatch_1();
            renderingContextValues2 = getRenderingContextValues_2();
            renderingContextValues3NoMatch = getRenderingContextValues_3_NoMatch();
            renderingContextValues4 = getRenderingContextValues_4();
            renderingContextValues5 = getRenderingContextValues_5();
            renderingContextValues6 = getRenderingContextValues_6();

            metricTemplatesProvider = mocks.StrictMock<IMetricTemplatesProvider>();
            Expect.Call(metricTemplatesProvider.GetGroupedMetricTemplatesMetadata()).Repeat.Times(7).Return(GetSuppliedMetricTemplates());
        }

        protected override void ExecuteTest()
        {
            metricTemplateBinder = new MetricTemplateBinder(metricTemplatesProvider);
        }

        private Dictionary<string, string> getRenderingContextValues_FullMatch_1()
        {
            return new Dictionary<string, string>
                       {
                           { "MetricInstanceSetType",  "School" },
                           { "RenderingMode", "Overview" },
                           { "MetricType",  "GranularMetric" },
                           { "Depth",  "Level2" },
                           { "Enabled",  "True" },
                           { "LocalEducationAgencyId",  "888" },
                           { "SchoolId",  "555" },
                           { "StudentUSI",  "444" },
                           { "ParentMetricId",  "333" },
                           { "MetricId",  "9" },
                           { "NullValue",  "false" },
                           { "OtherKey",  "111" },
                           { "Open", "true"}
                       };
        }

        private Dictionary<string, string> getRenderingContextValues_2()
        {
            return new Dictionary<string, string>
                       {
                           { "MetricInstanceSetType",  "Student" },
                           { "RenderingMode", "Overview" },
                           { "MetricType",  "GranularMetric" },
                           { "Depth",  "Level1" },
                           { "Enabled",  "True" },
                           { "LocalEducationAgencyId",  "1111" },
                           { "SchoolId",  "1234" },
                           { "StudentUSI",  "4444" },
                           { "ParentMetricId",  "3333" },
                           { "MetricId",  "19" },
                           { "NullValue",  "false" },
                           { "OtherKey",  "111" },
                           { "Open", "false"}
                       };
        }

        private Dictionary<string, string> getRenderingContextValues_3_NoMatch()
        {
            return new Dictionary<string, string>
                       {
                           { "MetricInstanceSetType",  "School" },
                           { "RenderingMode", "Overview" },
                           { "MetricType",  "AggregateMetric" },
                           { "Depth",  "Level3" },
                           { "Enabled",  "false" },
                           { "LocalEducationAgencyId",  "1" },
                           { "SchoolId",  "2" },
                           { "StudentUSI",  "3" },
                           { "ParentMetricId",  "4" },
                           { "MetricId",  "5" },
                           { "NullValue",  "false" },
                           { "OtherKey",  "6" },
                           { "Open", "true"}
                       };
        }

        private Dictionary<string, string> getRenderingContextValues_4()
        {
            return new Dictionary<string, string>
                       {
                           { "MetricInstanceSetType", "Student" },
                           { "RenderingMode", "Overview" },
                           { "MetricType", "GranularMetric" },
                           { "Depth",  "Level1" },
                           { "Enabled",  "True" },
                           { "LocalEducationAgencyId",  "1111" },
                           { "SchoolId",  "1234" },
                           { "StudentUSI",  "4444" },
                           { "ParentMetricId",  "3333" },
                           { "MetricId",  "19" },
                           { "NullValue",  "false" },
                           { "OtherKey",  "111" },
                           { "Open", "false"}
                       };
        }

        private Dictionary<string, string> getRenderingContextValues_5()
        {
            return new Dictionary<string, string>
                       {
                           { "MetricInstanceSetType", "NullTest" },
                           { "RenderingMode", "Overview" },
                           { "MetricType", "GranularMetric" },
                           { "Depth",  "Level1" },
                           { "Enabled",  "True" },
                           { "LocalEducationAgencyId",  "99" },
                           { "SchoolId",  "1" },
                           { "StudentUSI",  "7" },
                           { "ParentMetricId",  "9" },
                           { "MetricId",  "77" },
                           { "NullValue",  "false" },
                           { "Open", "true"}
                       };
        }

        private Dictionary<string, string> getRenderingContextValues_6()
        {
            return new Dictionary<string, string>
                       {
                           { "MetricInstanceSetType", "NullTest" },
                           { "RenderingMode", "Overview" },
                           { "MetricType", "GranularMetric" },
                           { "Depth",  "Level1" },
                           { "Enabled",  "True" },
                           { "LocalEducationAgencyId",  "99" },
                           { "SchoolId",  "1" },
                           { "StudentUSI",  "7" },
                           { "ParentMetricId",  "9" },
                           { "MetricId",  "77" },
                           { "NullValue",  "true" },
                           { "Open", "true"}
                       };
        }


        private TemplateLookup GetSuppliedMetricTemplates()
        {
            var templates =
                new List<MetricTemplateMetadata>
                       {
                           new MetricTemplateMetadata
                               {
                                   FullPath=templateNameNoTemplate,
                                   Tokens = new Dictionary<string, string>()
                               },
                           new MetricTemplateMetadata
                               {
                                   FullPath = templateName1,
                                   Tokens = new Dictionary<string, string>
                                       {
                                           { "MetricInstanceSetType", "LocalEducationAgency" },
                                           { "RenderingMode", "Overview" },
                                           { "MetricType", "ContainerMetric" },
                                           { "SchoolId", "555" },
                                           { "MetricId", "10" },
                                           { "Open", "true"}
                                       }
                               },
                           new MetricTemplateMetadata
                               {
                                   FullPath = templateName2,
                                   Tokens = new Dictionary<string, string>
                                       {
                                           { "MetricInstanceSetType", "School" },
                                           { "RenderingMode", "Overview" },
                                           { "MetricType", "GranularMetric" },
                                           { "SchoolId", "555" },
                                           { "MetricId", "9" },
                                           { "Enabled", "True" },
                                           { "OtherKey", "111" },
                                           { "Depth", "Level2" },
                                           { "ParentMetricId", "333" },
                                           { "LocalEducationAgencyId", "888" },
                                           { "StudentUSI", "444" },
                                           { "Open", "true"}
                                       }
                               },
                           new MetricTemplateMetadata
                               {
                                   FullPath = templateName2 + "Close",
                                   Tokens = new Dictionary<string, string>
                                       {
                                           { "MetricInstanceSetType", "School" },
                                           { "RenderingMode", "Overview" },
                                           { "MetricType", "GranularMetric" },
                                           { "SchoolId", "555" },
                                           { "MetricId", "9" },
                                           { "Enabled", "True" },
                                           { "OtherKey", "111" },
                                           { "Depth", "Level2" },
                                           { "ParentMetricId", "333" },
                                           { "LocalEducationAgencyId", "888" },
                                           { "StudentUSI", "444" },
                                           { "Open", "false"}
                                       }
                               },
                           new MetricTemplateMetadata
                               {
                                   FullPath = templateName3,
                                   Tokens = new Dictionary<string, string>
                                       {
                                           { "MetricInstanceSetType", "LocalEducationAgency" },
                                           { "RenderingMode", "Overview" },
                                           { "MetricType", "AggregateMetric" },
                                           { "LocalEducationAgencyId", "888" },
                                           { "ParentMetricId", "9" },
                                           { "Open", "true"}
                                       }
                               },
                           new MetricTemplateMetadata
                               {
                                   FullPath = templateName4,
                                   Tokens = new Dictionary<string, string>
                                       {
                                           { "MetricInstanceSetType", "Student" },
                                           { "RenderingMode", "Overview" },
                                           { "MetricType", "GranularMetric" },
                                           { "SchoolId", "1234" },
                                           { "MetricId", "19" },
                                           { "Open", "false"}
                                       }
                               },
                           new MetricTemplateMetadata
                               {
                                   FullPath = templateName4 + "Open",
                                   Tokens = new Dictionary<string, string>
                                       {
                                           { "MetricInstanceSetType", "Student" },
                                           { "RenderingMode", "Overview" },
                                           { "MetricType", "GranularMetric" },
                                           { "SchoolId", "1234" },
                                           { "MetricId", "19" },
                                           { "Open", "true"}
                                       }
                               },
                           new MetricTemplateMetadata
                               {
                                   FullPath = templateName5,
                                   Tokens = new Dictionary<string, string>
                                       {
                                           { "MetricInstanceSetType", "Student" },
                                           { "RenderingMode", "Overview" },
                                           { "MetricType", "GranularMetric" },
                                           { "Depth", "Level0" },
                                           { "Open", "true"}
                                       }
                               },
                           new MetricTemplateMetadata
                               {
                                   FullPath = templateName6,
                                   Tokens = new Dictionary<string, string>
                                       {
                                           { "MetricInstanceSetType", "NullTest" },
                                           { "RenderingMode", "Overview" },
                                           { "MetricType", "GranularMetric" },
                                           { "Depth", "Level1" },
                                           { "NullValue", "true" },
                                           { "Open", "true"}
                                       }
                               },
                           new MetricTemplateMetadata
                               {
                                   FullPath = templateName7,
                                   Tokens = new Dictionary<string, string>
                                       {
                                           { "MetricInstanceSetType", "NullTest" },
                                           { "RenderingMode", "Overview" },
                                           { "MetricType", "GranularMetric" },
                                           { "Depth", "Level1" },
                                           { "Open", "true"}
                                       }
                               }
                       };

            // Fill in the metric template grouping Ids
            templates.ForEach(m => m.TemplateGroupingId = MetricTemplateMetadata.GetTemplateGroupingId(m.Tokens));

            var lookupData = templates
                .GroupBy(x => x.TemplateGroupingId)
                .ToDictionary(g => g.Key,
                              g => g.Select(x => x).ToArray());

            return new TemplateLookup(lookupData);
        }

        [Test]
        public void Should_return_string_that_is_not_empty()
        {
            var templateName = metricTemplateBinder.GetTemplateName(renderingContextValues1FullMatch);

            Assert.IsNotNullOrEmpty(templateName);
        }

        [Test]
        public void Should_return_the_correct_file_name_when_all_context_rendering_values_are_supplied()
        {
            var templateName = metricTemplateBinder.GetTemplateName(renderingContextValues1FullMatch);

            Assert.AreEqual(templateName2, templateName);
        }

        [Test]
        public void Should_return_the_correct_template_name_when_not_all_the_rendering_values_are_supplied()
        {
            var templateName = metricTemplateBinder.GetTemplateName(renderingContextValues2);

            Assert.AreEqual(templateName4, templateName);
        }

        [Test]
        public void Should_return_the_no_template_template_when_no_matching_templates_are_found()
        {
            var templateName = metricTemplateBinder.GetTemplateName(renderingContextValues3NoMatch);

            Assert.AreEqual(templateNameNoTemplate, templateName);
        }

        [Test]
        public void Should_not_return_template_that_does_not_have_all_matching_template_context_values()
        {
            var templateName = metricTemplateBinder.GetTemplateName(renderingContextValues4);

            Assert.AreEqual(templateName4, templateName);
        }

        [Test]
        public void Should_match_on_null_value_parameter()
        {
            var templateName = metricTemplateBinder.GetTemplateName(renderingContextValues5);
            Assert.AreEqual(templateName7, templateName);

            templateName = metricTemplateBinder.GetTemplateName(renderingContextValues6);
            Assert.AreEqual(templateName, templateName6);
        }
    }
}
