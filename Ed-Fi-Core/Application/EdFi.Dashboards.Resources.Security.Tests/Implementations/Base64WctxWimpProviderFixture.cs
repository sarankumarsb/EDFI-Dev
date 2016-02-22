using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.Testing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Rhino.Mocks;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Security.Tests.Implementations
{
    public class When_Calling_Base64WctxWimpProvider : TestFixtureBase
    {
        protected IHttpRequestProvider httpRequestProvider;
        protected Base64WctxWimpProvider wctxWimpProvider;
        public static readonly string sampleWctxValue = "rm=0&id=passive&ru=https%3a%2f%2flocalhost%2fEdFiDashboardDev%2fDistricts%2fGrandBendISD%2fEntry%3fLocalEducationAgencyId%3d255901%26LocalEducationAgencyCode%3dGrandBendISD%26wimp%3d{0}";

        protected string wimp = "123456";
        protected string base64Wimp;
        protected string actualWimp;

        protected override void EstablishContext()
        {
            httpRequestProvider = mocks.Stub<IHttpRequestProvider>();
            base64Wimp = Convert.ToBase64String(Encoding.UTF8.GetBytes(wimp));
            Expect.Call(httpRequestProvider.GetValue("wctx")).Return(string.Format(sampleWctxValue, base64Wimp));

            wctxWimpProvider = new Base64WctxWimpProvider(httpRequestProvider);
        }

        protected override void ExecuteTest()
        {
            actualWimp = wctxWimpProvider.GetWimp();
        }

        [Test]
        public void Should_Return_Wimp()
        {
            Assert.That(actualWimp, Is.EqualTo(wimp));
        }
    }
}
