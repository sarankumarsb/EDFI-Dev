// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Presentation.Web.Providers.Metric;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Metric.Rendering.Tests
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "mvp"), Ignore("GKM: This fixtured needs to be fixed ASAP.")] //When_calling_the_get_from_MetricMetadataNodeService
    public class When_calling_the_get_metric_templates_from_the_asp_net_web_forms_mvp_metric_template_provider : TestFixtureBase
    {
        private IMetricTemplatesProvider templateProvider;
        private IEnumerable<MetricTemplateMetadata> metricTemplateMetadata;
        private IMetricTemplateVirtualPathsProvider fileNamesProvider;

        private string basePath;
        //private string extensionsBasePath;
        private string fileName1;
        private string fileName2;
        private string fileName3;
        private string fileName4;
        private IMetricTemplateConventionsProvider conventionsProvider;

        protected override void EstablishContext()
        {
            basePath = "C:\\MetricTemplates";
            //extensionsBasePath = "C:\\Extensions\\MetricTemplates";

            fileName1 = basePath + "\\School\\Overview\\ContainerMetric.Level1.Disabled.LocalEducationAgencyId_1234.SchoolId_567.StudentUSI_959.NullValue.ParentMetricId_884.MetricId_432.OtherKey_111.ascx";
            fileName2 = basePath + "\\Student\\Metric\\AggregateMetric.Open.Level2.SchoolId_567.StudentUSI_959.MetricId_432.ascx";
            fileName3 = basePath + "\\LocalEducationAgency\\Overview\\GranularMetric.Close.SchoolId_555.MetricId_9.OtherKey_111.Level2.ParentMetricId_333.StudentUSI_444.LocalEducationAgencyId_888.ascx";
            fileName4 = basePath + "\\NoTemplate.ascx";

            fileNamesProvider = mocks.StrictMock<IMetricTemplateVirtualPathsProvider>();
            SetupResult.For(fileNamesProvider.GetMetricTemplates()).Return(GetSuppliedFileNamesList());

            conventionsProvider = mocks.StrictMock<IMetricTemplateConventionsProvider>();
            SetupResult.For(conventionsProvider.TemplatesVirtualPath).Return(basePath);

            //Expect.Call(fileNamesProvider.GetMetricTemplatesBasePath()).Return(basePath).Repeat.Any();
            //Expect.Call(fileNamesProvider.GetMetricTemplateExtensionsBasePath()).Return(extensionsBasePath).Repeat.Any();
        }

        protected override void ExecuteTest()
        {
            templateProvider = new MetricTemplatesProvider(fileNamesProvider.ToArray(), conventionsProvider);
            metricTemplateMetadata = templateProvider.GetMetricTemplatesMetadata();
        }

        private IEnumerable<string> GetSuppliedFileNamesList()
        {
            return new List<string>
                       {
                           fileName1,
                           fileName2,
                           fileName3,
                           fileName4
                       };
        }

        [Test]
        public void Should_return_model_that_is_not_null()
        {
            Assert.NotNull(metricTemplateMetadata);
        }

        [Test]
        public void Should_return_model_with_correct_name_and_all_corresponding_tokens()
        {
            var fileMetadata = metricTemplateMetadata.Where(x => x.FullPath == fileName1).SingleOrDefault();

            Assert.NotNull(fileMetadata);

            Assert.AreEqual(fileName1, fileMetadata.FullPath);

            //domain entity type
            Assert.AreEqual("School", fileMetadata.Tokens.Where(x=>x.Key=="MetricInstanceSetType").Select(x=>x.Value).SingleOrDefault());

            //RenderingMode
            Assert.AreEqual("Overview", fileMetadata.Tokens.Where(x => x.Key == "RenderingMode").Select(x => x.Value).SingleOrDefault());

            //MetricType
            Assert.AreEqual("ContainerMetric", fileMetadata.Tokens.Where(x => x.Key == "MetricType").Select(x => x.Value).SingleOrDefault());

            //Open/Close
            Assert.AreEqual("true", fileMetadata.Tokens.Where(x => x.Key == "Open").Select(x => x.Value).SingleOrDefault());

            //Depth
            Assert.AreEqual("Level1", fileMetadata.Tokens.Where(x => x.Key == "Depth").Select(x => x.Value).SingleOrDefault());

            //Enabled
            Assert.AreEqual("false", fileMetadata.Tokens.Where(x => x.Key == "Enabled").Select(x => x.Value).SingleOrDefault());

            //LocalEducationAgencyId
            Assert.AreEqual("1234", fileMetadata.Tokens.Where(x => x.Key == "LocalEducationAgencyId").Select(x => x.Value).SingleOrDefault());

            //SchoolId
            Assert.AreEqual("567", fileMetadata.Tokens.Where(x => x.Key == "SchoolId").Select(x => x.Value).SingleOrDefault());

            //StudentUSI
            Assert.AreEqual("959", fileMetadata.Tokens.Where(x => x.Key == "StudentUSI").Select(x => x.Value).SingleOrDefault());

            //ParentMetricId
            Assert.AreEqual("884", fileMetadata.Tokens.Where(x => x.Key == "ParentMetricId").Select(x => x.Value).SingleOrDefault());

            //MetricId
            Assert.AreEqual("432", fileMetadata.Tokens.Where(x => x.Key == "MetricId").Select(x => x.Value).SingleOrDefault());

            //Others
            Assert.AreEqual("111", fileMetadata.Tokens.Where(x => x.Key == "OtherKey").Select(x => x.Value).SingleOrDefault());

            //NullValue
            Assert.AreEqual("true", fileMetadata.Tokens.Where(x => x.Key == "NullValue").Select(x => x.Value).SingleOrDefault());
        }

        [Test]
        public void Should_return_template_metadata_correctly_with_some_optional_parameters_missing()
        {
            var fileMetadata = metricTemplateMetadata.Where(x => x.FullPath == fileName2).SingleOrDefault();
            //Student\\AggregateMetric.Level2.SchoolId_567.StudentUSI_959.MetricId_432.ascx";
            Assert.NotNull(fileMetadata);

            Assert.AreEqual(fileName2, fileMetadata.FullPath);

            //domain entity type
            Assert.AreEqual("Student", fileMetadata.Tokens.Where(x => x.Key == "MetricInstanceSetType").Select(x => x.Value).SingleOrDefault());

            //RenderingMode
            Assert.AreEqual("Metric", fileMetadata.Tokens.Where(x => x.Key == "RenderingMode").Select(x => x.Value).SingleOrDefault());

            //MetricType
            Assert.AreEqual("AggregateMetric", fileMetadata.Tokens.Where(x => x.Key == "MetricType").Select(x => x.Value).SingleOrDefault());

            //Open/Close
            Assert.AreEqual("true", fileMetadata.Tokens.Where(x => x.Key == "Open").Select(x => x.Value).SingleOrDefault());

            //Depth
            Assert.AreEqual("Level2", fileMetadata.Tokens.Where(x => x.Key == "Depth").Select(x => x.Value).SingleOrDefault());

            //Enabled
            Assert.IsNull(fileMetadata.Tokens.Where(x => x.Key == "Enabled").Select(x => x.Value).SingleOrDefault());

            //NullValue
            Assert.IsNull(fileMetadata.Tokens.Where(x => x.Key == "NullValue").Select(x => x.Value).SingleOrDefault());

            //LocalEducationAgencyId
            Assert.IsNull(fileMetadata.Tokens.Where(x => x.Key == "LocalEducationAgencyId").Select(x => x.Value).SingleOrDefault());

            //SchoolId
            Assert.AreEqual("567", fileMetadata.Tokens.Where(x => x.Key == "SchoolId").Select(x => x.Value).SingleOrDefault());

            //StudentUSI
            Assert.AreEqual("959", fileMetadata.Tokens.Where(x => x.Key == "StudentUSI").Select(x => x.Value).SingleOrDefault());

            //ParentMetricId
            Assert.IsNull(fileMetadata.Tokens.Where(x => x.Key == "ParentMetricId").Select(x => x.Value).SingleOrDefault());

            //MetricId
            Assert.AreEqual("432", fileMetadata.Tokens.Where(x => x.Key == "MetricId").Select(x => x.Value).SingleOrDefault());

            //Others
            Assert.IsNull(fileMetadata.Tokens.Where(x => x.Key == "OtherKey").Select(x => x.Value).SingleOrDefault());
        }

        [Test]
        public void Should_return_template_metadata_correctly_regardless_of_order_in_file_name()
        {
            var fileMetadata = metricTemplateMetadata.Where(x => x.FullPath == fileName3).SingleOrDefault();

            Assert.NotNull(fileMetadata);

            Assert.AreEqual(fileName3, fileMetadata.FullPath);

            //domain entity type
            Assert.AreEqual("LocalEducationAgency", fileMetadata.Tokens.Where(x => x.Key == "MetricInstanceSetType").Select(x => x.Value).SingleOrDefault());

            //RenderingMode
            Assert.AreEqual("Overview", fileMetadata.Tokens.Where(x => x.Key == "RenderingMode").Select(x => x.Value).SingleOrDefault());

            //MetricType
            Assert.AreEqual("GranularMetric", fileMetadata.Tokens.Where(x => x.Key == "MetricType").Select(x => x.Value).SingleOrDefault());

            //Open/Close
            Assert.AreEqual("false", fileMetadata.Tokens.Where(x => x.Key == "Open").Select(x => x.Value).SingleOrDefault());

            //Depth
            Assert.AreEqual("Level2", fileMetadata.Tokens.Where(x => x.Key == "Depth").Select(x => x.Value).SingleOrDefault());

            //Enabled
            Assert.IsNullOrEmpty(fileMetadata.Tokens.Where(x => x.Key == "Enabled").Select(x => x.Value).SingleOrDefault());

            //LocalEducationAgencyId
            Assert.AreEqual("888", fileMetadata.Tokens.Where(x => x.Key == "LocalEducationAgencyId").Select(x => x.Value).SingleOrDefault());

            //SchoolId
            Assert.AreEqual("555", fileMetadata.Tokens.Where(x => x.Key == "SchoolId").Select(x => x.Value).SingleOrDefault());

            //StudentUSI
            Assert.AreEqual("444", fileMetadata.Tokens.Where(x => x.Key == "StudentUSI").Select(x => x.Value).SingleOrDefault());

            //ParentMetricId
            Assert.AreEqual("333", fileMetadata.Tokens.Where(x => x.Key == "ParentMetricId").Select(x => x.Value).SingleOrDefault());

            //MetricId
            Assert.AreEqual("9", fileMetadata.Tokens.Where(x => x.Key == "MetricId").Select(x => x.Value).SingleOrDefault());

            //Others
            Assert.AreEqual("111", fileMetadata.Tokens.Where(x => x.Key == "OtherKey").Select(x => x.Value).SingleOrDefault());

            //NullValue
            Assert.IsNull(fileMetadata.Tokens.Where(x => x.Key == "NullValue").Select(x => x.Value).SingleOrDefault());

        }
    }
}
