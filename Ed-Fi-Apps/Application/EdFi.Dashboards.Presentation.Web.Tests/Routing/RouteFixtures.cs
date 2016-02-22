/*
 NOTE: This file demonstrates how to override the route fixtures for an override controller specific to
 * your implementation of the dashboards.  The core test methods are all declared as "virtual", and the
 * expected controller can be changed by overriding the core test fixture, as shown below.
 */
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using NUnit.Framework;

// Add reference to namespaces for overridden controllers here...
//using EdFi.Dashboards.Presentation.Web.Areas.StudentSchool.Controllers.Detail;

namespace EdFi.Dashboards.Presentation.Web.Tests.Routing
{
    /// <summary>
    /// Provides test methods for overridden controllers, or custom routes/controllers that are implementation-specific.
    /// If there is a lot of overiding/extending being done, this class should probably be split into several partial classes,
    /// broken out by area (as done in the EdFi.Dashboards.Presentation.Core.Tests project).
    /// </summary>
    public class When_resolving_routes_to_controllers : Core.Tests.Routing.When_resolving_routes_to_controllers
    {
        //[Test]
        //public override void Should_go_to_student_school_Metric_Detail_Historical_Learning_Objective()
        //{
        //    // This test demonstrates how that the routing system for a particular implementation will correctly resolve to 
        //    // the implementation-specific controller (in this example using the "Historical Learning Objectives" chart)
        //    // NOTE: The controller type referenced below should be the replacement/override controller.

        //    // Student.MetricsDrilldown(TestId.School, TestId.Student, TestId.MetricVariant, "HistoricalLearningObjectivesChart", TestName.Student).ToVirtual()
        //    //    .ShouldMapTo<HistoricalLearningObjectivesChartController>(); 
        //}
    }
}
