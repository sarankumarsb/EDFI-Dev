// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Testing;
using NUnit.Framework;

namespace EdFi.Dashboards.Metric.Resources.Tests.Providers
{
    [TestFixture]
    class TrendRenderingDispositionProviderFixture : TestFixtureBase
    {
        private ITrendRenderingDispositionProvider provider;

        protected override void ExecuteTest()
        {
            provider = new TrendRenderingDispositionProvider();
        }

        [Test]
        public void Should_return_trend_disposition_that_is_not_null()
        {
            Assert.NotNull(provider);
        }

        [Test]
        public void Should_return_correct_trend_disposition()
        {
            Assert.AreEqual(TrendEvaluation.DownBad, provider.GetTrendRenderingDisposition(TrendDirection.Decreasing, TrendInterpretation.Standard));
            Assert.AreEqual(TrendEvaluation.UpGood, provider.GetTrendRenderingDisposition(TrendDirection.Increasing, TrendInterpretation.Standard));
            Assert.AreEqual(TrendEvaluation.None, provider.GetTrendRenderingDisposition(TrendDirection.None, TrendInterpretation.Standard));
            Assert.AreEqual(TrendEvaluation.NoChangeNoOpinion, provider.GetTrendRenderingDisposition(TrendDirection.Unchanged, TrendInterpretation.Standard));
            Assert.AreEqual(TrendEvaluation.UpGood, provider.GetTrendRenderingDisposition(TrendDirection.Decreasing, TrendInterpretation.Inverse));
            Assert.AreEqual(TrendEvaluation.DownBad, provider.GetTrendRenderingDisposition(TrendDirection.Increasing, TrendInterpretation.Inverse));
            Assert.AreEqual(TrendEvaluation.None, provider.GetTrendRenderingDisposition(TrendDirection.None, TrendInterpretation.Inverse));
            Assert.AreEqual(TrendEvaluation.NoChangeNoOpinion, provider.GetTrendRenderingDisposition(TrendDirection.Unchanged, TrendInterpretation.Inverse));
        }

        [Test]
        public void Should_return_default_correctly()
        {
            Assert.AreEqual(TrendEvaluation.None, provider.GetTrendRenderingDisposition(TrendDirection.None, TrendInterpretation.Inverse));
            Assert.AreEqual(TrendEvaluation.None, provider.GetTrendRenderingDisposition(TrendDirection.None, TrendInterpretation.Standard));
        }
    }
}
