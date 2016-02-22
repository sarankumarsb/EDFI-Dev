// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Routing;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Testing;
using Moq;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Tests.Navigation
{
    public abstract class When_preparing_route_values : TestFixtureBase {
        protected Mock<IRouteValueProvider> firstValueProviderMock;
        protected Mock<IRouteValueProvider> secondValueProviderMock;
        protected RouteValueDictionary suppliedRouteValues = new RouteValueDictionary();

        protected void MockCall_CanProvideRouteValue(Mock<IRouteValueProvider> valueProviderMock, string key, bool result)
        {
            valueProviderMock.Setup(x =>
                x.CanProvideRouteValue(key, It.IsAny<Func<string, object>>()))
                .Returns(result);
        }

        protected void MockCall_ProvideRouteValue(Mock<IRouteValueProvider> valueProviderMock, Action<string, Func<string, object>, Action<string, object>> action)
        {
            valueProviderMock.Setup(x => 
                x.ProvideRouteValue(It.IsAny<string>(), It.IsAny<Func<string, object>>(), It.IsAny<Action<string, object>>()))
                .Callback(action);
        }

        protected void MockCall_ProvideRouteValue(Mock<IRouteValueProvider> valueProviderMock, string key, Action<string, Func<string, object>, Action<string, object>> action)
        {
            valueProviderMock.Setup(x => 
                x.ProvideRouteValue(key, It.IsAny<Func<string, object>>(), It.IsAny<Action<string, object>>()))
                .Callback(action);
        }

        protected void RegisterRoute(string routeName, string routePattern)
        {
            RouteTable.Routes.Add(routeName, new Route(routePattern, null));
        }

        protected override void ExecuteTest()
        {
            var preparer = new RouteValuesPreparer(new[] { firstValueProviderMock.Object, secondValueProviderMock.Object });
            preparer.PrepareRouteValues("TestRoute", suppliedRouteValues);
        }
    }

    public class When_preparing_route_values_where_none_of_the_providers_can_provide_any_of_the_values_requested : When_preparing_route_values
    {
        protected override void EstablishContext()
        {
            firstValueProviderMock = new Mock<IRouteValueProvider>();
            MockCall_CanProvideRouteValue(firstValueProviderMock, It.IsAny<string>(), false);

            secondValueProviderMock = new Mock<IRouteValueProvider>();
            MockCall_CanProvideRouteValue(secondValueProviderMock, It.IsAny<string>(), false);

            RouteTable.Routes.Clear();
            RegisterRoute("TestRoute", "{one}/{two}");
        }

        [Test]
        public void Should_query_all_providers_for_all_route_values_found_in_the_route_URL_parameters()
        {
            firstValueProviderMock.Verify(x => x.CanProvideRouteValue("one", It.IsAny<Func<string, object>>()));
            firstValueProviderMock.Verify(x => x.CanProvideRouteValue("two", It.IsAny<Func<string, object>>()));

            secondValueProviderMock.Verify(x => x.CanProvideRouteValue("one", It.IsAny<Func<string, object>>()));
            secondValueProviderMock.Verify(x => x.CanProvideRouteValue("two", It.IsAny<Func<string, object>>()));
        }
    }

    public class When_preparing_route_values_where_one_of_the_values_is_provided : When_preparing_route_values
    {
        protected override void EstablishContext()
        {
            firstValueProviderMock = new Mock<IRouteValueProvider>();

            MockCall_CanProvideRouteValue(firstValueProviderMock, "one", true);
            MockCall_CanProvideRouteValue(firstValueProviderMock, "two", false);

            secondValueProviderMock = new Mock<IRouteValueProvider>();
            MockCall_CanProvideRouteValue(secondValueProviderMock, "two", false);

            MockCall_ProvideRouteValue(firstValueProviderMock, (key, getValue, setValue) => setValue(key, "abcd"));

            RouteTable.Routes.Clear();
            RegisterRoute("TestRoute", "{one}/{two}");
        }

        [Test]
        public void Should_not_ask_second_provider_for_value_already_provided_by_first_provider()
        {
            secondValueProviderMock.Verify(x => x.CanProvideRouteValue("one", It.IsAny<Func<string, object>>()), Times.Never());
            secondValueProviderMock.Verify(x => x.CanProvideRouteValue("two", It.IsAny<Func<string, object>>()), Times.Once());
        }

        [Test]
        public void Should_add_the_provided_route_value_to_the_route_values_dictionary()
        {
            Assert.That(suppliedRouteValues.ContainsKey("one"));

            Assert.That(suppliedRouteValues["one"], Is.EqualTo("abcd"));
        }
    }

    public class When_preparing_route_values_where_all_of_the_values_are_provided_as_derived_from_ids : When_preparing_route_values
    {
        protected override void EstablishContext()
        {
            // Set up some initial incoming route values
            suppliedRouteValues["oneId"] = 1;
            suppliedRouteValues["twoId"] = 2;

            firstValueProviderMock = new Mock<IRouteValueProvider>();

            MockCall_CanProvideRouteValue(firstValueProviderMock, "one", true);
            MockCall_ProvideRouteValue(firstValueProviderMock, (key, getValue, setValue) => setValue(key, "1"));
            MockCall_CanProvideRouteValue(firstValueProviderMock, "two", false);

            secondValueProviderMock = new Mock<IRouteValueProvider>();
            MockCall_CanProvideRouteValue(secondValueProviderMock, "two", true);
            MockCall_ProvideRouteValue(secondValueProviderMock, (key, getValue, setValue) => setValue(key, "2"));

            RouteTable.Routes.Clear();
            RegisterRoute("TestRoute", "{one}/{two}");
        }

        [Test]
        public void Should_add_the_required_route_values()
        {
            Assert.That(suppliedRouteValues["one"], Is.EqualTo("1"));
            Assert.That(suppliedRouteValues["two"], Is.EqualTo("2"));
        }

        [Test]
        public void Should_remove_all_the_non_required_route_values()
        {
            Assert.That(suppliedRouteValues.Count, Is.EqualTo(2));

            Assert.That(!suppliedRouteValues.ContainsKey("oneId"));
            Assert.That(!suppliedRouteValues.ContainsKey("twoId"));
        }
    }

    public class When_preparing_route_values_where_one_of_the_providers_depends_on_a_value_provided_by_another_provider : When_preparing_route_values
    {
        private Mock<IRouteValueProvider> metricProviderMock;
        private Mock<IRouteValueProvider> schoolValueProviderMock;
        private Mock<IRouteValueProvider> leaValueProviderMock;

        private const int suppliedSchoolId = 1;
        private const int suppliedMetricId = 1000;

        protected override void EstablishContext()
        {
            // Set up some initial incoming route values
            suppliedRouteValues["schoolId"] = suppliedSchoolId;
            suppliedRouteValues["metricId"] = suppliedMetricId;

            leaValueProviderMock = new Mock<IRouteValueProvider>();
            MockCall_CanProvideRouteValue(leaValueProviderMock, "localEducationAgency", true);
            MockCall_ProvideRouteValue(leaValueProviderMock, "localEducationAgency", (key, getValue, setValue) =>
                {
                    // First, it needs the LEA Id, which hasn't been provided (Can another provider supply it?)
                    var leaId = Convert.ToInt32(getValue("localEducationAgencyId"));
                    
                    // Incorporate Id (which should have been fetched by schoolValueProviderMock) into the result, for verification purposes
                    setValue("localEducationAgency", "SomeISD-" + leaId);
                });


            schoolValueProviderMock = new Mock<IRouteValueProvider>();
            MockCall_CanProvideRouteValue(schoolValueProviderMock, "localEducationAgencyId", true);
            MockCall_CanProvideRouteValue(schoolValueProviderMock, "school", true);
            MockCall_ProvideRouteValue(schoolValueProviderMock, "localEducationAgencyId", (key, getValue, setValue) =>
                {
                    // Set the school id
                    int schoolId = Convert.ToInt32(getValue("schoolId"));

                    // Incorporate schoolId into the LEA Id, this value will be requested by leaValueProviderMock
                    setValue("localEducationAgencyId", schoolId * 100);
                });
            MockCall_ProvideRouteValue(schoolValueProviderMock, "school", (key, getValue, setValue) =>
                {
                    int schoolId = Convert.ToInt32(getValue("schoolId"));

                    // Set the requested value
                    setValue("school", "Some-School-" + schoolId);
                });

            metricProviderMock = new Mock<IRouteValueProvider>();
            MockCall_CanProvideRouteValue(metricProviderMock, "metricName", true);
            MockCall_ProvideRouteValue(metricProviderMock, "metricName", (key, getValue, setValue) =>
                {
                    int metricId = Convert.ToInt32(getValue("metricId"));

                    // Incorporate metricId into the metricName
                    setValue(key, "Metric" + metricId);   
                });

            RouteTable.Routes.Clear();
            RegisterRoute("TestRoute", "{localEducationAgency}/{school}/{metricName}-{metricId}");
        }

        protected override void ExecuteTest()
        {
            var preparer = new RouteValuesPreparer(new[] { leaValueProviderMock.Object, schoolValueProviderMock.Object, metricProviderMock.Object });
            preparer.PrepareRouteValues("TestRoute", suppliedRouteValues);
        }

        [Test]
        public void Should_ask_school_provider_for_the_localEducationAgencyId_even_though_it_is_not_a_required_route_value_because_it_is_needed_by_the_lea_value_provider()
        {
            // Make sure we asked for it...
            schoolValueProviderMock.Verify(x => x.CanProvideRouteValue("localEducationAgencyId", It.IsAny<Func<string, object>>()), Times.Once());

            // And then actually got it...
            schoolValueProviderMock.Verify(x => x.ProvideRouteValue("localEducationAgencyId", It.IsAny<Func<string, object>>(), It.IsAny<Action<string, object>>()), Times.Once());
        }

        [Test]
        public void Should_add_the_dependent_localEducationAgency_route_value_to_the_route_values()
        {
            Assert.That(suppliedRouteValues.ContainsKey("localEducationAgency"));
            Assert.That(suppliedRouteValues["localEducationAgency"], Is.EqualTo("SomeISD-" + (suppliedSchoolId * 100)));
        }

        [Test]
        public void Should_add_the_school_value_to_the_route_values()
        {
            Assert.That(suppliedRouteValues.ContainsKey("school"));
            Assert.That(suppliedRouteValues["school"], Is.EqualTo("Some-School-" + suppliedSchoolId));
        }

        [Test]
        public void Should_add_metricName_route_value_to_the_route_values()
        {
            Assert.That(suppliedRouteValues.ContainsKey("metricName"));
            Assert.That(suppliedRouteValues["metricName"], Is.EqualTo("Metric" + suppliedMetricId));
        }

        [Test]
        public void Should_leave_metricId_in_route_values()
        {
            Assert.That(suppliedRouteValues.ContainsKey("metricId"));
            Assert.That(suppliedRouteValues["metricId"], Is.EqualTo(suppliedMetricId));
        }

        [Test]
        public void Should_end_up_with_a_total_of_4_route_values()
        {
            Assert.That(suppliedRouteValues.Count, Is.EqualTo(4));
        }
    }

    public class When_preparing_route_values_where_the_providers_depend_on_each_others_values : When_preparing_route_values
    {
        private Exception actualException;

        private bool alreadyCalled = false;

        protected override void EstablishContext()
        {
            firstValueProviderMock = new Mock<IRouteValueProvider>();

            // First provider depends on value provided on second provider
            MockCall_CanProvideRouteValue(firstValueProviderMock, "one", true);
            MockCall_ProvideRouteValue(firstValueProviderMock, "one", (key, getValue, setValue) =>
                {
                    if (alreadyCalled)
                        throw new Exception("Should never have called back in to the first provider. An infinite loop will result."); // Stop infinite loop from killing test, if it exists
                    
                    getValue("two"); // First provider depends on value provided by second provider   
                    alreadyCalled = true;
                });

            secondValueProviderMock = new Mock<IRouteValueProvider>();
            MockCall_CanProvideRouteValue(secondValueProviderMock, "two", true);
            MockCall_ProvideRouteValue(secondValueProviderMock, "two", (key, getValue, setValue) => getValue("one")); // Second provider depends on value provided by first provider

            RouteTable.Routes.Clear();
            RegisterRoute("TestRoute", "{one}/{two}");
        }

        protected override void ExecuteTest()
        {
            try
            {
                base.ExecuteTest();
            }
            catch (Exception ex)
            {
                actualException = ex;
            }
        }

        [Test]
        public void Should_throw_an_exception_stopping_an_infinite_loop_due_to_the_cyclical_dependency_in_the_route_value_providers()
        {
            Assert.That(actualException, Is.Not.Null);
            Assert.That(actualException.Message, Is.StringContaining("cyclical dependency"));
        }
    }
}
