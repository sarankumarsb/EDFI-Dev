using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.Search;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Search;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Resource.Models.Common;
using log4net;

namespace EdFi.Dashboards.Presentation.Core.Areas.Search.Controllers
{
    public class QuickSearchController : Controller
    {

        private const string logFormat = "{0} entered \"{1}\"into the simple search text field.";
        private readonly IService<QuickSearchRequest, QuickSearchModel> service;
        private readonly IRootMetricNodeResolver rootMetricNodeResolver;
        private readonly IStudentSchoolAreaLinks studentSchoolLinks;
        private static readonly ILog searchLogger = LogManager.GetLogger("SearchLogger");

        public QuickSearchController(IService<QuickSearchRequest, QuickSearchModel> service, IRootMetricNodeResolver rootMetricNodeResolver, IStudentSchoolAreaLinks studentSchoolLinks, ICurrentUserClaimInterrogator currentUserClaimInterrogator)
        {
            this.service = service;
            this.rootMetricNodeResolver = rootMetricNodeResolver;
            this.studentSchoolLinks = studentSchoolLinks;
        }

        [HttpPost]
        public JsonResult Get(string textToFind, int? rowCountToReturn, bool? matchContains, int localEducationAgencyId)
        {
            Dictionary<SearchFilter, HashSet<Tuple<int, Type>>> searchFilters = GetCurrentUserSearchClaimEducationOrganizationAssociations(UserInformation.Current.AssociatedOrganizations);

            var quickSearchRequest = new QuickSearchRequest
            {
                TextToFind = textToFind,
                RowCountToReturn = rowCountToReturn ?? 6,
                MatchContains = matchContains ?? false,
                SearchFilters = searchFilters,
            };

            QuickSearchModel res = service.Get(quickSearchRequest);

            FinishStudentSearch(res, rowCountToReturn ?? 6);

            LogSimpleSearch(textToFind);

            //return Json(presenter.Initialize(textToFind, rowCountToReturn, matchContains));
            return Json(res);
        }

        private void FinishStudentSearch(QuickSearchModel model, int rowCountToReturn)
        {
            if (model.StudentQuery == null)
                return;

            var studentDataList = model.StudentQuery.Take(rowCountToReturn).ToList();
            model.Students = (from s in studentDataList
                              select new QuickSearchModel.StudentSearchItem(s.StudentUSI)
                              {
                                  Id = s.StudentUSI,
                                  Text = Resources.Utilities.FormatPersonNameByLastName(s.FirstName, s.MiddleName, s.LastSurname),
                                  Link = new Link { Href = studentSchoolLinks.Overview(s.SchoolId, s.StudentUSI, s.FullName).Resolve(), },
                                  IdentificationCode = s.StudentIdentificationCode
                              }).ToList();
            model.StudentQuery = null;
        }

        private static void LogSimpleSearch(string textToFind)
        {
            var userName = String.Empty;

            var currentUser = Thread.CurrentPrincipal;
            if (currentUser != null)
                userName = currentUser.Identity.Name;

            //[User Name] simple search for [Search Terms] returned [Count of Results] results.
            searchLogger.InfoFormat(logFormat, userName, textToFind);
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
                            if (!results.ContainsKey(SearchFilter.Teachers))
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
