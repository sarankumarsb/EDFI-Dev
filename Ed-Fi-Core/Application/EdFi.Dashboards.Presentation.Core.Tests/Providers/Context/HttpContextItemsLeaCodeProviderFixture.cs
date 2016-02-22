using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Testing;
using EdFi.Dashboards.Presentation.Core.Providers.Context;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Presentation.Core.Tests.Providers.Context
{
    public abstract class HttpContextItemsLeaCodeProviderFixture : TestFixtureBase
    {
        protected IIdCodeService idCodeService;
        protected IHttpRequestProvider httpRequestProvider;
        protected IHttpContextItemsProvider httpContextItemsProvider;
        protected HttpContextItemsLeaCodeProvider httpContextItemsLeaCodeProvider;
        protected string actualResult;

        protected override void EstablishContext()
        {
            idCodeService = mocks.Stub<IIdCodeService>();
            httpRequestProvider = mocks.Stub<IHttpRequestProvider>();
            httpContextItemsProvider = mocks.Stub<IHttpContextItemsProvider>();
            httpContextItemsLeaCodeProvider = new HttpContextItemsLeaCodeProvider(idCodeService, httpRequestProvider, httpContextItemsProvider, null);
        }

        protected override void ExecuteTest()
        {
            actualResult = httpContextItemsLeaCodeProvider.GetLeaCode(new LeaCodeRequest());
        }
    }

    public class When_Calling_HttpContextItemsLeaCodeProvider_Item_Not_In_Context : HttpContextItemsLeaCodeProviderFixture
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(httpContextItemsProvider.Contains("lea")).Return(false);
        }

        [Test]
        public void Should_Return_Null()
        {
            Assert.That(actualResult, Is.Null);
        }
    }

    public class When_Calling_HttpContextItemsLeaCodeProvider_Item_In_Context : HttpContextItemsLeaCodeProviderFixture
    {
        protected string suppliedLeaValue = "suppliedLea";

        protected override void EstablishContext()
        {
            base.EstablishContext();
            Expect.Call(httpContextItemsProvider.Contains("lea")).Return(true);
            Expect.Call(httpContextItemsProvider.GetValue("lea")).Return(suppliedLeaValue);
        }

        [Test]
        public void Should_Return_Value()
        {
            Assert.That(actualResult, Is.EqualTo(suppliedLeaValue));
        }
    }
}
