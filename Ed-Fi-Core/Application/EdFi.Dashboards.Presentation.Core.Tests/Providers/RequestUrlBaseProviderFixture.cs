using System;
using System.Web;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;
using EdFi.Dashboards.Presentation.Architecture.Providers;

namespace EdFi.Dashboards.Presentation.Core.Tests.Providers
{
    [TestFixture]
    public abstract class When_Calling_the_RequestUrlBaseProvider : TestFixtureBase
    {
        private RequestUrlBaseProvider provider;
        protected string ActualUrlBase;

        protected HttpRequestBase SuppliedHttpRequest;

        protected string SuppliedApplicationPath;
        protected Uri SuppliedUrl;
        protected string ExpectedUrlBase;

        protected override void EstablishContext()
        {            
            SuppliedHttpRequest = mocks.StrictMock<HttpRequestBase>();

            Expect.Call(SuppliedHttpRequest.Url).Return(SuppliedUrl);
            Expect.Call(SuppliedHttpRequest.ApplicationPath).Return(SuppliedApplicationPath);
        }

        protected override void ExecuteTest()
        {
            provider = new RequestUrlBaseProvider();
            ActualUrlBase = provider.GetRequestUrlBase(SuppliedHttpRequest);
        }

        [Test]
        public void Should_return_expected_url_base()
        {
            Assert.That(ActualUrlBase, Is.EqualTo(ExpectedUrlBase));
        }
    }

    public class When_Calling_the_RequestUrlBaseProvider_with_a_url_that_has_an_application : When_Calling_the_RequestUrlBaseProvider
    {        
        protected override void EstablishContext()
        {
            SuppliedUrl = new Uri("https://localhost/application/districts/MyISD/Overview");
            SuppliedApplicationPath = "/application";
            ExpectedUrlBase = "https://localhost/application/";

            base.EstablishContext();
            
        }
    }

    public class When_Calling_the_RequestUrlBaseProvider_with_a_url_that_does_NOT_have_an_application : When_Calling_the_RequestUrlBaseProvider
    {
        protected override void EstablishContext()
        {
            SuppliedUrl = new Uri("https://localhost/districts/MyISD/Overview");
            SuppliedApplicationPath = "";
            ExpectedUrlBase = "https://localhost/";

            SuppliedHttpRequest = mocks.StrictMock<HttpRequestBase>();

            Expect.Call(SuppliedHttpRequest.Url).Repeat.Twice().Return(SuppliedUrl);
            Expect.Call(SuppliedHttpRequest.ApplicationPath).Return(SuppliedApplicationPath);
        }
    }

    public class When_Calling_the_RequestUrlBaseProvider_with_a_url_that_has_an_application_that_matches_other_strings_further_down_the_url : When_Calling_the_RequestUrlBaseProvider
    {
        protected override void EstablishContext()
        {
            //This test proves that we fixed a bug that was found.
            SuppliedUrl = new Uri("https://localhost/demo/districts/MyISD/demographics/studentList");
            SuppliedApplicationPath = "/demo";
            ExpectedUrlBase = "https://localhost/demo/";

            base.EstablishContext();
        }
    }
}
