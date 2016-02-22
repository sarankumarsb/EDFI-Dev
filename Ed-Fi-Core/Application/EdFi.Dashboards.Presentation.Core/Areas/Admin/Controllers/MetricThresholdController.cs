using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using EdFi.Dashboards.Application.Resources.LocalEducationAgency;
using EdFi.Dashboards.Presentation.Core.Areas.Admin.Models;

namespace EdFi.Dashboards.Presentation.Core.Areas.Admin.Controllers
{
    public class MetricThresholdController : Controller
    {
        protected readonly IMetricThresholdService metricThresholdService;
        protected const int GradesBelowCLevelMetricId = 26;
        protected const decimal GradesBelowCLevelThresholdDefaultValue = 0.75m;

        public MetricThresholdController(IMetricThresholdService metricThresholdService)
        {
            this.metricThresholdService = metricThresholdService;
        }

        [HttpGet]
        public virtual ViewResult Get(int localEducationAgencyId, string message, bool? delete)
        {
            var viewModelCollection = GetViewModelCollection(localEducationAgencyId);

            ViewBag.Message = message;

            return View("Get", viewModelCollection);
        }

        protected virtual MetricThresholdMetricModel[] GetViewModelCollection(int localEducationAgencyId)
        {
            //hard coding metric id 26 for the Grades Below C Level Metric which is the only metric threshold that will be edited for now
            //also the get method on the service is only designed to return a collection, even for the single item call. so changing it 
            var viewModel = GetMetricThresholdViewModel(localEducationAgencyId, GradesBelowCLevelMetricId, GradesBelowCLevelThresholdDefaultValue, "Grades Below C Level", "Each district has a range used to define a \"C\" level. Please enter the lower end of this range to set the metric threshold for grades below a C level. Only whole numbers can be entered in the system (e.g. the default setting is 75).");
            var viewModelCollection = new[] {viewModel};
            return viewModelCollection;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="localEducationAgencyId"></param>
        /// <param name="metricId"></param>
        /// <param name="defaultThreshold"></param>
        /// <param name="metricTitle"></param>
        /// <param name="description"></param>
        /// <param name="multiplier">This model was first used to represent percentages, where the value displayed is 100x the value stored in the database.  In this case pass in .01.  If you're using a whole number, such as days absent, use 1.</param>
        /// <returns></returns>
        protected virtual MetricThresholdMetricModel GetMetricThresholdViewModel(int localEducationAgencyId, int metricId, decimal defaultThreshold, string metricTitle, string description, decimal multiplier = .01m)
        {
            var serviceModel =
                metricThresholdService.Get(new MetricThresholdGetRequest
                                               {
                                                   LocalEducationAgencyId = localEducationAgencyId,
                                                   MetricId = metricId
                                               })
                                      .FirstOrDefault();

            //if there is no existing record then create new object
            if (serviceModel == null)
            {
                serviceModel = new MetricThresholdGetResponse
                                   {
                                       MetricId = metricId,
                                       Threshold = defaultThreshold
                                   };
            }

            var viewModel = new MetricThresholdMetricModel { MetricThresholdGetResponse = serviceModel, DefaultValue = defaultThreshold, MetricTitle = metricTitle, Description = description, Multiplier = multiplier };
            return viewModel;
        }

        [HttpPost]
        public virtual bool Get(MetricThresholdPutRequest request)
        {
            bool created;
            var response = metricThresholdService.Put(request, out created);

            return true;
        }

        [HttpPost]
        public virtual bool Delete(MetricThresholdDeleteRequest request)
        {
            metricThresholdService.Delete(request);
            return true;
        }
    }
}
