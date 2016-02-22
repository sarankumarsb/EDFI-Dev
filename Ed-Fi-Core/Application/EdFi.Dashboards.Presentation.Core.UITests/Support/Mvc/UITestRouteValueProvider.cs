using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Presentation.Core.Tests.Routing.Support;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;
using Newtonsoft.Json;
using RestSharp;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.Mvc
{
    /// <summary>
    /// Provides names for the limited set of route values needed to satisfy the URL generation using the routing
    /// infrastructure in a disconnected mode.
    /// </summary>
    public class UITestRouteValueProvider : IRouteValueProvider
    {
        public bool CanProvideRouteValue(string key, Func<string, object> getValue)
        {
            return true;
        }

        public void ProvideRouteValue(string key, Func<string, object> getValue, Action<string, object> setValue)
        {
            switch (key)
            {
                case "localEducationAgency":
                    setValue("localEducationAgency", TestSessionContext.Current.Configuration.LocalEducationAgency);
                    break;
                case "metricName":
                    setValue("metricName", TestName.Metric); // Metric name is aesthetic only
                    break;
                case "school":
                    setValue("school", GetSchoolName((int) getValue("schoolId")));
                    break;
                case "staff":
                    setValue("staff", TestName.Staff); // Staff name is aesthetic only
                    break;
                case "student":
                    setValue("student", TestName.Student); // Student name is aesthetic only
                    break;
            }
        }

        private static List<SchoolCategoryModel.School> schools;

        private string GetSchoolName(int schoolId)
        {
            if (schools == null)
            {
                string schoolCategoryListUrl = Website.LocalEducationAgency.SchoolCategoryList(TestSessionContext.Current.Configuration.LocalEducationAgencyId);

                var request = new RestRequest(schoolCategoryListUrl.ToRelativeDashboardPath());
                var client = ScenarioContext.Current.GetRestClient();
                var response = client.Execute(request);

                IEnumerable<SchoolCategoryModel> schoolCategoryList;
                
                try
                {
                    schoolCategoryList = JsonConvert.DeserializeObject<IEnumerable<SchoolCategoryModel>>(response.Content);
                }
                catch (Exception ex)
                {
                    throw new Exception(string.Format("Unable to deserialize web service response as JSON: {0}", response.Content), ex);
                }

                schools =
                    (from sc in schoolCategoryList
                     from s in sc.Schools
                     select s)
                     .ToList();
            }

            // Find the school name
            var schoolName =
                (from s in schools
                 where s.SchoolId == schoolId
                 select s.Name)
                 .SingleOrDefault();

            return schoolName;
        }
    }
}
