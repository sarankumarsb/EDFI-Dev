// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Presentation.Architecture.Providers;
using EdFi.Dashboards.Presentation.Core.Models.Shared;
using EdFi.Dashboards.Presentation.Web.Utilities;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Support;

namespace EdFi.Dashboards.Presentation.Core.Controllers
{
    public class PreviousNextController : Controller
    {
        private readonly ISessionStateProvider sessionStateProvider;
        private readonly IRouteValuesPreparer routeValuesPreparer;
        private readonly IRequestUrlBaseProvider requestUrlBaseProvider;

        public PreviousNextController(ISessionStateProvider sessionStateProvider, IRouteValuesPreparer routeValuesPreparer, IRequestUrlBaseProvider requestUrlBaseProvider)
        {
            this.sessionStateProvider = sessionStateProvider;
            this.routeValuesPreparer = routeValuesPreparer;
            this.requestUrlBaseProvider = requestUrlBaseProvider;
        }

        [ChildActionOnly]
        public ActionResult PreviousNext()
        {
            PreviousNextViewModel viewModel = null;

            var listContext = Request.QueryString["listContext"];

            //If we don't have a listContext then lets get out of here...
            if (string.IsNullOrEmpty(listContext))
                return View(viewModel);

            var resourceModel = sessionStateProvider[listContext] as PreviousNextDataModel;

            //If we don't have a model then lets get out of here...
            if (resourceModel == null)
                return View(viewModel);

            //Lets make sure the model has a list to traverse. If not lets get out of here...
            if (resourceModel.EntityIdArray.Length <= 0)
                return View(viewModel);


            //If we are still here we have enough info and so lets proceed.
            viewModel = new PreviousNextViewModel();
            var edfiContext = EdFiDashboardContext.Current;

            //Lets calculate the position of the current Entity in the array
            var entityId = new List<long>();
            
            foreach (string paramName in resourceModel.ParameterNames)
            {
                //entityId.Add(Convert.ToInt32(Request.QueryString[paramName]));
                switch (paramName)
                {
                    case "schoolId":
                        entityId.Add(edfiContext.SchoolId.Value);
                        break;

                    case "studentUSI":
                        entityId.Add(edfiContext.StudentUSI.Value);
                        break;

                    case "staffUSI":
                        entityId.Add(edfiContext.StaffUSI.Value);
                        break;

                    case "localEducationAgencyId":
                        entityId.Add(edfiContext.LocalEducationAgencyId.Value);
                        break;

                    default:
                        throw new Exception(string.Format("Cant handle parameter name {0}",paramName));
                }
            }

            var mostRelavantEntityIdIndex = GetMostSignificantParameterIndex(resourceModel.ParameterNames);
            var entityNumber = GetEntityPlaceOnTheList(entityId.ToArray(), resourceModel.EntityIdArray, mostRelavantEntityIdIndex);

            //Setting the student {0} of {1} label.
            viewModel.PositionText = String.Format("{0} of {1}", entityNumber, resourceModel.EntityIdArray.Length);

            //Setting the return URL
            viewModel.ListLink = BuildReturnUrl(resourceModel);

            //Setting previous and next links
            //prepare the URL

            //Calculate Entity indexes.
            var entityIndex = entityNumber - 1;
            var previousEntityIndex = entityIndex - 1;
            var nextEntityIndex = entityIndex + 1;

            //The list should be a circular one so
            if (previousEntityIndex < 0)
                previousEntityIndex = resourceModel.EntityIdArray.Length - 1;
            if (nextEntityIndex >= resourceModel.EntityIdArray.Length)
                nextEntityIndex = 0;

            // Get the current view context
            var currentViewContext = this.ControllerContext.ParentActionViewContext;

            // Find the original view context
            while (currentViewContext.IsChildAction)
                currentViewContext = currentViewContext.ParentActionViewContext;

            //Build the new routes.
            var currentRoute = currentViewContext.RouteData.Route as Route;
            var currentRouteValues = currentViewContext.RouteData.Values;
            var currentArea = currentRoute.GetArea();
            var currentHref = Request.Url.ToString();

            switch (currentArea)
            {
                //case "LocalEducationAgency":
                //    break;
                case "School":
                    var prevSchoolId = resourceModel.EntityIdArray[previousEntityIndex][0];
                    string prevUrl = BuildSchoolUrl(prevSchoolId, currentRoute, currentRouteValues);

                    var nextSchoolId = resourceModel.EntityIdArray[nextEntityIndex][0];
                    string nextUrl = BuildSchoolUrl(nextSchoolId, currentRoute, currentRouteValues);

                    viewModel.PreviousEntityLink = prevUrl.AppendParameters("listContext=" + listContext);
                    viewModel.NextEntityLink = nextUrl.AppendParameters("listContext=" + listContext);
                    break;

                case "StudentSchool":
                    var prevStudentSchoolSchoolUSI = resourceModel.EntityIdArray[previousEntityIndex][1];
                    var prevStudentSchoolStudentUSI = resourceModel.EntityIdArray[previousEntityIndex][0];

                    var nextStudentSchoolSchoolUSI = resourceModel.EntityIdArray[nextEntityIndex][1];
                    var nextStudentSchoolStudentUSI = resourceModel.EntityIdArray[nextEntityIndex][0];

                    var studentPrevUrl = BuildStudentSchoolUrl(prevStudentSchoolSchoolUSI, prevStudentSchoolStudentUSI, currentRoute, currentRouteValues);
                    var studentNextUrl = BuildStudentSchoolUrl(nextStudentSchoolSchoolUSI, nextStudentSchoolStudentUSI, currentRoute, currentRouteValues);

                    viewModel.PreviousEntityLink = studentPrevUrl.AppendParameters("listContext=" + listContext);
                    viewModel.NextEntityLink = studentNextUrl.AppendParameters("listContext=" + listContext);
                    break;

                case "Staff":
                    var prevSchoolStaffUSI = resourceModel.EntityIdArray[previousEntityIndex][1];
                    var prevStaffUSI = resourceModel.EntityIdArray[previousEntityIndex][0];

                    var nextSchoolStaffUSI = resourceModel.EntityIdArray[nextEntityIndex][1];
                    var nextStaffUSI = resourceModel.EntityIdArray[nextEntityIndex][0];

                    var staffPrevUrl = BuildSchoolStaffUrl((int)prevSchoolStaffUSI, prevStaffUSI, currentRoute, currentRouteValues);
                    var staffNextUrl = BuildSchoolStaffUrl((int)nextSchoolStaffUSI, nextStaffUSI, currentRoute, currentRouteValues);

                    if (!string.IsNullOrEmpty(edfiContext.SubjectArea))
                    {
                        viewModel.PreviousEntityLink = staffPrevUrl.AppendParameters(new string[] { "SubjectArea=" + edfiContext.SubjectArea, "listContext=" + listContext });
                        viewModel.NextEntityLink = staffNextUrl.AppendParameters(new string[] { "SubjectArea=" + edfiContext.SubjectArea, "listContext=" + listContext });
                    }
                    else
                    {
                        viewModel.PreviousEntityLink = staffPrevUrl.AppendParameters("listContext=" + listContext);
                        viewModel.NextEntityLink = staffNextUrl.AppendParameters("listContext=" + listContext);    
                    }
                    
                    break;

                default:
                    throw new Exception(string.Format("Can't handle area:{0}",currentArea));
            }

            viewModel.BackToListText = resourceModel.FromSearch ? "Back&nbsp;to&nbsp;search" : "Back&nbsp;to&nbsp;list";
            
            return PartialView(viewModel);
        }

        private string BuildSchoolUrl(long entityId, Route currentRoute, RouteValueDictionary currentRouteValues)
        {
            var newRouteValues = new RouteValueDictionary(currentRouteValues);
            newRouteValues.Remove("school");
            newRouteValues["schoolId"] = entityId;
            routeValuesPreparer.PrepareRouteValues(currentRoute, newRouteValues);
            return GetRoutedUrl(currentRoute, newRouteValues, ControllerContext.RequestContext);
        }

        private string BuildStudentSchoolUrl(long schoolId, long studentUSI, Route currentRoute, RouteValueDictionary currentRouteValues)
        {
            var newRouteValues = new RouteValueDictionary(currentRouteValues);

            newRouteValues.Remove("school");
            newRouteValues.Remove("student");

            newRouteValues["schoolId"] = schoolId;
            newRouteValues["studentUSI"] = studentUSI;

            routeValuesPreparer.PrepareRouteValues(currentRoute, newRouteValues);
            return GetRoutedUrl(currentRoute, newRouteValues, ControllerContext.RequestContext);
        }

        private string BuildSchoolStaffUrl(int schoolId, long staffUSI, Route currentRoute, RouteValueDictionary currentRouteValues)
        {
            var newRouteValues = new RouteValueDictionary(currentRouteValues);

            newRouteValues.Remove("school");
            newRouteValues.Remove("staff");
            //The next teacher will not have the same section.
            newRouteValues.Remove("studentlistType"); 
            newRouteValues.Remove("sectionOrCohortId");

            newRouteValues["schoolId"] = schoolId;
            newRouteValues["staffUSI"] = staffUSI;
            newRouteValues["studentlistType"] = "None";//The next teacher will not have the same section.

            routeValuesPreparer.PrepareRouteValues(currentRoute, newRouteValues);
            return GetRoutedUrl(currentRoute, newRouteValues, ControllerContext.RequestContext);
        }

        private string GetRoutedUrl(RouteBase route, RouteValueDictionary routeValues, RequestContext requestContext)
        {
            string routedVirtualPath = route.GetVirtualPath(requestContext, routeValues).VirtualPath; // i.e. "AllenISD"

            // Get the base URL with the trailing slash, if it exists
            string urlBase = requestUrlBaseProvider.GetRequestUrlBase(requestContext.HttpContext.Request);

            // Construct the URL that has been regenerated (i.e. a request for https://app/AllenISD/Overview will be shortened 
            // to https://app/AllenISD if "Overview" is defaulted, thereby making it possible to match with generated URLs on 
            // the menu items for determining correct menu selection)
            var returnValue = (new Uri(urlBase + routedVirtualPath)).AbsolutePath;

            return returnValue;
        }

        private static string BuildReturnUrl(PreviousNextDataModel model)
        {
            var listToExpand = "";
            var anchorName = "";

            if (!string.IsNullOrEmpty(model.MetricId))
            {
                listToExpand = "tdId=" + model.ListType + model.MetricId;
                anchorName = "#m" + model.MetricId;
            }

            //If the original URL has already a TDID value then we have to replace it with the new one.
            if (model.ListUrl.IndexOf("tdId") > -1)
            {
                var firstPartOfUrl = model.ListUrl.Substring(0, model.ListUrl.IndexOf("?") + 1);
                var queryStrings = model.ListUrl.Substring(model.ListUrl.IndexOf("?") + 1);
                var qsArray = queryStrings.Split('&');
                for (int i = 0; i < qsArray.Length; i++)
                {
                    if (qsArray[i].Contains("tdId"))
                        qsArray[i] = listToExpand;
                }

                if (model.ListUrl.Contains("#"))
                    model.ListUrl = model.ListUrl.Substring(0, model.ListUrl.IndexOf("#"));

                //Rebuild string				
                model.ListUrl = firstPartOfUrl + string.Join("&", qsArray) + anchorName;
            }
            else
            {
                if (model.ListUrl.Contains("#"))
                    model.ListUrl = model.ListUrl.Substring(0, model.ListUrl.IndexOf("#"));

                if (string.IsNullOrEmpty(listToExpand))
                    model.ListUrl += anchorName;
                else
                {
                    if(model.ListUrl.Contains("?"))
                        model.ListUrl += "&" + listToExpand + anchorName;
                    else
                        model.ListUrl += "?" + listToExpand + anchorName;
                }

            }

            return model.ListUrl;
        }

        private static int GetEntityPlaceOnTheList(long[] entityIdToFind, IEnumerable<long[]> entityIdList, int mostRelavantEntityIdIndex)
        {
            int i = 1;

            foreach (var entityId in entityIdList)
            {
                bool entityFound = true;
                for (int j = 0; j < entityId.Length; j++)
                {
                    if (entityId[j] != entityIdToFind[j])
                    {
                        entityFound = false;
                        break;
                    }
                }

                if (entityFound)
                    return i;

                i++;
            }

            // entity was not found
            //lets try to look for the entity by a least specific search.
            //In general the most significant Id is in the last cell so.
            i = 1;
            foreach (var entityId in entityIdList)
            {
                bool entityFound = true;

                if (entityId[mostRelavantEntityIdIndex] != entityIdToFind[mostRelavantEntityIdIndex])
                    entityFound = false;

                if (entityFound)
                    return i;

                i++;
            }

            //If both fail then entity was not found.
            return 0;
        }

        protected virtual int GetMostSignificantParameterIndex(string[] parameterNames)
        {
            string[] parameterSignificance = {"studentUSI","staffUSI","schoolId","localEducationAgencyId"};

            foreach (string significantParameter in parameterSignificance)
                if (parameterNames.Contains(significantParameter))
                    for (int j = 0; j < parameterNames.Length; j++)
                        if (parameterNames[j].Equals(significantParameter, StringComparison.OrdinalIgnoreCase))
                            return j;

            return -1;
        }
    }
}
