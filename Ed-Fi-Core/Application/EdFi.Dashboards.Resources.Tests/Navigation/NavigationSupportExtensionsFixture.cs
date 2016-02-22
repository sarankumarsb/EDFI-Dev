// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Resources.Navigation.Support;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Navigation
{
    [TestFixture]
    public class When_invoking_Navigation_Support_Extensions_AppendParameters_method : TestFixtureBase
    {
        //The Injected Dependencies.


        //The Actual Model.
        private string actualModel;

        //The supplied Data models.
        private string suppliedURL = "http://www.edfi.com/some.aspx";


        protected override void EstablishContext()
        {
            //Prepare supplied data collections



            //Set up the mocks


            //Set expectations
            //Expect.Call().Return();
        }

        protected override void ExecuteTest()
        {
            actualModel = NavigationSupportExtensions.AppendParameters(suppliedURL, "a=a1","b=b1");
        }

        [Test]
        public void Should_append_parameters_correctly()
        {
            Assert.That(actualModel, Is.EqualTo(suppliedURL+"?a=a1&b=b1"));
        }

    }

    [TestFixture]
    public class When_invoking_Navigation_Support_Extensions_AppendParameters_method_for_url_with_fragment : TestFixtureBase
    {
        //The Injected Dependencies.


        //The Actual Model.
        private string actualModel;

        //The supplied Data models.
        private const string SuppliedUrl = "http://www.edfi.com/some.aspx#someFragments";


        protected override void EstablishContext()
        {
            //Prepare supplied data collections



            //Set up the mocks


            //Set expectations
            //Expect.Call().Return();
        }

        protected override void ExecuteTest()
        {
            actualModel = SuppliedUrl.AppendParameters("a=a1", "b=b1");
        }

        [Test]
        public void Should_append_parameters_correctly()
        {
            Assert.That(actualModel, Is.EqualTo("http://www.edfi.com/some.aspx?a=a1&b=b1#someFragments"));
        }
    }
    [TestFixture]
    public class When_invoking_Navigation_Support_Extensions_AppendParameters_method_for_url_with_fragment_and_adding_key_value : TestFixtureBase
    {
        //The Injected Dependencies.


        //The Actual Model.
        private string actualModel;

        //The supplied Data models.
        private const string SuppliedUrl = "http://www.edfi.com/some.aspx#someFragments";


        protected override void EstablishContext()
        {
            //Prepare supplied data collections



            //Set up the mocks


            //Set expectations
            //Expect.Call().Return();
        }

        protected override void ExecuteTest()
        {
            var parameters = new[]
            {
                new KeyValuePair<string, string>("a", "a1"),
                new KeyValuePair<string, string>("b", "b1"),
            };

            actualModel = SuppliedUrl.AppendParameters(parameters);
        }

        [Test]
        public void Should_append_parameters_correctly()
        {
            Assert.That(actualModel, Is.EqualTo("http://www.edfi.com/some.aspx?a=a1&b=b1#someFragments"));
        }
    }
}
