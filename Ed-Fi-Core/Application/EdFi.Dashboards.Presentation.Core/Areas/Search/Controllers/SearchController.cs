using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Models.Search;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Search;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using log4net;

namespace EdFi.Dashboards.Presentation.Core.Areas.Search.Controllers
{
    public class SearchController : Controller
    {
        private const string logFormat = "{0} ran advanced search for \"{1}\" with a rowCountToReturnOf:{2} and returned [Schools: {3}, Teachers: {4}, Students: {5}] results.";
        private readonly IService<SearchRequest, SearchModel> service;
        private readonly IStudentSchoolAreaLinks studentSchoolLinks;
        private readonly ICurrentUserClaimInterrogator currentUserClaimInterrogator;
        private readonly IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;
        private static readonly ILog searchLogger = LogManager.GetLogger("SearchLogger");

        public SearchController(IService<SearchRequest, SearchModel> service, IStudentSchoolAreaLinks studentSchoolLinks, ICurrentUserClaimInterrogator currentUserClaimInterrogator, IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider)
        {
            this.service = service;
            this.studentSchoolLinks = studentSchoolLinks;
            this.currentUserClaimInterrogator = currentUserClaimInterrogator;
            this.gradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider;
        }

        [HttpPost]
        public JsonResult Get(string textToFind, int rowCountToReturn, bool matchContains, string filter, int localEducationAgencyId)
        {
            Dictionary<SearchFilter, HashSet<Tuple<int, Type>>> searchFilters = GetCurrentUserSearchClaimEducationOrganizationAssociations(UserInformation.Current.AssociatedOrganizations);
            
            var searchRequest = new SearchRequest
            {
                TextToFind = textToFind,
                RowCountToReturn = rowCountToReturn,
                MatchContains = matchContains,
                SearchFilters = searchFilters,
            };

            if (!String.IsNullOrEmpty(filter) && String.Compare(filter, "All", StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                SearchFilter pageFilter;
                Enum.TryParse(filter, true, out pageFilter);
                searchRequest.PageFilter = pageFilter;
            }

            var res = service.Get(searchRequest);

            FinishStudentSearch(res, rowCountToReturn);

            LogSearchResults(textToFind, rowCountToReturn, res);

            return Json(res);
        }
        private void FinishStudentSearch(SearchModel model, int rowCountToReturn)
        {
            if (model.StudentQuery == null)
                return;

            var studentDataList = model.StudentQuery.Take(rowCountToReturn).ToList();
            model.Students = (from s in studentDataList
                              select new SearchModel.StudentSearchItem(s.StudentUSI)
                              {
                                  Text = Resources.Utilities.FormatPersonNameByLastName(s.FirstName, s.MiddleName, s.LastSurname),
                                  FirstName = s.FirstName,
                                  MiddleName = s.MiddleName,
                                  LastSurname = s.LastSurname,
                                  GradeLevel = new SearchModel.StudentSearchItem.GradeLevelItem { DV = s.GradeLevel, V = gradeLevelUtilitiesProvider.FormatGradeLevelForSorting(s.GradeLevel) },
                                  SchoolId = s.SchoolId,
                                  School = s.School,
                                  Link = new Link { Href = studentSchoolLinks.Overview(s.SchoolId, s.StudentUSI, s.FullName).Resolve() },
                                  IdentificationSystem = s.IdentificationSystem,
                                  IdentificationCode = s.StudentIdentificationCode 
                              }).ToList();
            model.AbsoluteStudentsCount = model.Students.Count < rowCountToReturn ? model.Students.Count : model.StudentQuery.Count();
            model.StudentQuery = null;
        }

        private static void LogSearchResults(string textToFind, int rowCountToReturn, SearchModel actualResults)
        {
            //Logging search results for future enhancements...
            var userName = Thread.CurrentPrincipal.Identity.Name;

            //[User Name] simple search for [Search Terms] returned [Count of Results] results.
            var messageToLog = string.Format(logFormat,
                userName, textToFind, rowCountToReturn, actualResults.AbsoluteSchoolsCount, actualResults.AbsoluteTeachersCount, actualResults.AbsoluteStudentsCount);

            searchLogger.Info(messageToLog);
        }


        private Dictionary<SearchFilter, HashSet<Tuple<int, Type>>> GetCurrentUserSearchClaimEducationOrganizationAssociations(IEnumerable<UserInformation.EducationOrganization> educationOrganizationClaims)
        {
            var results = new Dictionary<SearchFilter, HashSet<Tuple<int, Type>>>();
            foreach (var educationOrganization in educationOrganizationClaims)
            {
                foreach (var claimType in educationOrganization.ClaimTypes)
                {
                    switch (claimType)
                    {
                        case EdFiClaimTypes.ViewAllTeachers:
                            if(!results.ContainsKey(SearchFilter.Teachers))
                            {
                                results.Add(SearchFilter.Teachers, new HashSet<Tuple<int, Type>>());
                            }
                            results[SearchFilter.Teachers].Add(Tuple.Create(educationOrganization.EducationOrganizationId, educationOrganization.GetType()));
                            break;
                        case EdFiClaimTypes.AdministerDashboard:
                            if (!results.ContainsKey(SearchFilter.Staff))
                            {
                                results.Add(SearchFilter.Staff, new HashSet<Tuple<int, Type>>());
                            }
                            results[SearchFilter.Staff].Add(Tuple.Create(educationOrganization.EducationOrganizationId, educationOrganization.GetType()));
                            break;
                        case EdFiClaimTypes.ViewAllMetrics:
                            if (!results.ContainsKey(SearchFilter.Schools))
                            {
                                results.Add(SearchFilter.Schools, new HashSet<Tuple<int, Type>>());
                            }
                            results[SearchFilter.Schools].Add(Tuple.Create(educationOrganization.EducationOrganizationId, educationOrganization.GetType()));
                            break;
                        case EdFiClaimTypes.ViewMyStudents:
                        case EdFiClaimTypes.ViewAllStudents:
                            if (!results.ContainsKey(SearchFilter.Students))
                            {
                                results.Add(SearchFilter.Students, new HashSet<Tuple<int, Type>>());
                            }
                            results[SearchFilter.Students].Add(Tuple.Create(educationOrganization.EducationOrganizationId, educationOrganization.GetType()));
                            break;
                    }
                }
            }
            return results.Count > 0 ? results : null;
        }

    }
}
