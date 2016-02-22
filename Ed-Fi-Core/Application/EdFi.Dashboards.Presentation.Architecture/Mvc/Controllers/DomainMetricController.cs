// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Web.Mvc;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Metric.Rendering;
using EdFi.Dashboards.Metric.Resources;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Navigation.Mvc;
using EdFi.Dashboards.Resources.Navigation.Support;

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.Controllers
{
    public class DomainMetricController<TMetricSetRequest> : Controller 
        where TMetricSetRequest : MetricInstanceSetRequestBase, new()
    {
        private readonly IDomainMetricService<TMetricSetRequest> domainMetricService;

        public DomainMetricController(IDomainMetricService<TMetricSetRequest> domainMetricService)
        {
            this.domainMetricService = domainMetricService;
        }

        public ActionResult Get(TMetricSetRequest metricSetRequest, int localEducationAgencyId) 
        {
            // TODO: Deferred - localEducationAgencyId can be dropped after drilldowns are no longer using WebForms.
            // localEducationAgencyId is here to force model binding to populate it in context so that it 
            // can be provided to the metric Action urls so that the context is available in order to 
            // use the correct database connection (for multitenancy with multiple databases).  Otherwise,
            // when the drilldown is initiated on a WebForms URL, there is no local education agency context
            // provided, and the only way to get it would be to go to the database, which itself also needs
            // the local education agency context in order to select the correct connection, and into a loop
            // we go.  Once all website artifacts are using the MVC routing, this parameter could be dropped.

            MetricBase model=null;
            var context = EdFiDashboardContext.Current;

            try
            {
                model = domainMetricService.Get(metricSetRequest).RootNode;
            }
            catch(MetricNodeNotFoundException)
            {
                var listContext = Request.QueryString["listContext"];
                if (context.MetricInstanceSetType == MetricInstanceSetType.School)
                    return Redirect(EdFiDashboards.Site.School.Overview(context.SchoolId.GetValueOrDefault()).AppendParameters("listContext=" + listContext));
                if (context.MetricInstanceSetType == MetricInstanceSetType.StudentSchool)
                    return Redirect(EdFiDashboards.Site.StudentSchool.Overview(context.SchoolId.GetValueOrDefault(), context.StudentUSI.GetValueOrDefault()).AppendParameters("listContext=" + listContext));
                throw;
            }

            ViewBag.RenderingMode = Enum.Parse(typeof (RenderingMode),
                                                   ControllerContext.RouteData.Values["renderingMode"].ToString());
            
            return View(model);
            
        }
    }
}
