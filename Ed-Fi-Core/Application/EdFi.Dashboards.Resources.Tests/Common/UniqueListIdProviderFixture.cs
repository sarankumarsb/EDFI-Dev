// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Common
{
    [TestFixture]
    public class When_invoking_the_Unique_List_Id_Provider : TestFixtureBase
    {
        //The Injected Dependencies.
        IHttpRequestProvider httpRequestProvider;

        //The Actual Model.
        private string actualUniqueId;
        private string actualUniqueIdWithMetricId;

        //The supplied Data models.
        private int suppliedMetricId = 10;


        protected override void EstablishContext()
        {
            //Prepare supplied data collections


            //Set up the mocks
            httpRequestProvider = mocks.StrictMock<IHttpRequestProvider>();

            //Set expectations
            Expect.Call(httpRequestProvider.Url).Return(new Uri("http://www.edfi.com/mypage.aspx"));
            Expect.Call(httpRequestProvider.UrlReferrer).Return(new Uri("http://www.edfi.com/referrer.aspx"));
        }

        protected override void ExecuteTest()
        {
            var service = new UniqueListIdProvider(httpRequestProvider);
            actualUniqueId = service.GetUniqueId();
            actualUniqueIdWithMetricId = service.GetUniqueId(suppliedMetricId);
        }

        [Test]
        public void Should_return_unique_id()
        {
            Assert.That(actualUniqueId, Is.EqualTo("mypageaspx"));
        }

        [Test]
        public void Should_return_unique_id_with_metric_id()
        {
            Assert.That(actualUniqueIdWithMetricId, Is.EqualTo("referreraspx"+suppliedMetricId));
        }

    }
}
