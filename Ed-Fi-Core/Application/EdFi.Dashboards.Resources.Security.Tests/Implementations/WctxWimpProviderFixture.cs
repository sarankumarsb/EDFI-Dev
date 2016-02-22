using System.Web;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.Testing;
using Rhino.Mocks;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Security.Tests.Implementations
{
    public abstract class WctxWimpProviderFixture : TestFixtureBase
    {
        protected IHttpRequestProvider httpRequestProvider;
        protected WctxWimpProvider wctxWimpProvider;
        protected string wimp = "MjA3MjY1";
        protected string result;

        protected override void EstablishContext()
        {
            httpRequestProvider = mocks.Stub<IHttpRequestProvider>();
            wctxWimpProvider = new WctxWimpProvider(httpRequestProvider);
        }

        protected override void ExecuteTest()
        {
            result = wctxWimpProvider.GetWimp();
        }
    }

    public class When_Calling_WctxWimp_GetWimp_No_Wctx : WctxWimpProviderFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(httpRequestProvider.GetValue("wctx")).Return("");
        }

        [Test]
        public void Should_Return_Wimp()
        {
            Assert.That(result, Is.EqualTo(null));
        }
    }

    public class When_Calling_WctxWimp_GetWimp : WctxWimpProviderFixture
    {
        public static readonly string sampleWctxValue = "rm=0&id=passive&ru=https%3a%2f%2flocalhost%2fEdFiDashboardDev%2fDistricts%2fGrandBendISD%2fEntry%3fLocalEducationAgencyId%3d255901%26LocalEducationAgencyCode%3dGrandBendISD%26wimp%3dMjA3MjY1";
        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(httpRequestProvider.GetValue("wctx")).Return(sampleWctxValue);
        }

        [Test]
        public void Should_Return_Wimp()
        {
            Assert.That(result, Is.EqualTo(wimp));
        }
    }
}
