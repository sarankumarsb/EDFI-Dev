// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel.Registration;
using Castle.Windsor.Installer;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Core.Providers.Context;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Queries;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resource.Models.Common;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Models.Search;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using Microsoft.IdentityModel.Tokens.Saml2;

namespace EdFi.Dashboards.Resources.Search
{
    [Flags]
    public enum SearchFilter
    {
        None = 0x00,
        Students = 0x01,
        Teachers = 0x02,
        Schools = 0x04,
        Staff = 0x08
    }
    
    public class SearchRequest
    {
        public string TextToFind { get; set; }
        public int RowCountToReturn { get; set; }
        public bool MatchContains { get; set; }
        public SearchFilter PageFilter { get; set; }
        public Dictionary<SearchFilter, HashSet<Tuple<int, Type>>> SearchFilters { get; set; }

        public static SearchRequest Create(string textToFind, int rowCountToReturn, bool matchContains, SearchFilter pageFilter, Dictionary<SearchFilter, HashSet<Tuple<int, Type>>> searchFilters)
        {
            return new SearchRequest
                              {
                                  TextToFind = textToFind,
                                  RowCountToReturn = rowCountToReturn,
                                  MatchContains = matchContains,
                                  PageFilter = pageFilter,
                                  SearchFilters = searchFilters,
                              };
        }
    }

    public interface ISearchService : IService<SearchRequest, SearchModel> {}
    
    public class SearchService : ISearchService
    {
        private readonly IRepository<SchoolInformation> schoolInformationRepository;
        private readonly IRepository<StudentInformation> studentInformationRepository;
        private readonly IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        private readonly IRepository<StaffInformation> staffInformationRepository;
        private readonly IRepository<StaffEducationOrgInformation> staffEducationOrganizationRepository;
        private readonly IRepository<StaffSectionCohortAssociation> staffSectionCohortAssociationRepository;
        private readonly IRepository<StaffIdentificationCode> staffIdentificationCodeRepository;
        private readonly IRepository<StudentIdentificationCode> studentIdentificationCodeRepository;        
        private readonly IStaffAreaLinks staffLinks;
        private readonly ISchoolAreaLinks schoolLinks;
        private readonly IAdminAreaLinks adminLinks;
        private readonly IStaffInformationLookupKeyProvider staffInformationLookupKeyProvider;
        private readonly StaffInformationAndAssociatedOrganizationsByUSIQuery staffInfoAndOrgQuery;
        private readonly ILocalEducationAgencySearchProvider searchProvider;

        public SearchService(
            IRepository<SchoolInformation> schoolInformationRepository,
            IRepository<StudentInformation> studentInformationRepository, 
            IRepository<StudentSchoolInformation> studentSchoolInformationRepository,
            StaffInformationAndAssociatedOrganizationsByUSIQuery staffInfoAndOrgQuery,  
            IRepository<StaffInformation> staffInformationRepository,
            IRepository<StaffEducationOrgInformation> staffEducationOrganizationRepository,
            IRepository<StaffSectionCohortAssociation> staffSectionCohortAssociationRepository,
            IRepository<StaffIdentificationCode> staffIdentificationCodeRepository,
            IRepository<StudentIdentificationCode> studentIdentificationCodeRepository,
            IStaffAreaLinks staffLinks, 
            ISchoolAreaLinks schoolLinks, 
            IAdminAreaLinks adminLinks,
            IStaffInformationLookupKeyProvider staffInformationLookupKeyProvider,
            ILocalEducationAgencySearchProvider searchProvider)
        {
            this.schoolInformationRepository = schoolInformationRepository;
            this.studentInformationRepository = studentInformationRepository;
            this.studentSchoolInformationRepository = studentSchoolInformationRepository;
            this.staffInfoAndOrgQuery = staffInfoAndOrgQuery;
            this.staffInformationRepository = staffInformationRepository;
            this.staffEducationOrganizationRepository = staffEducationOrganizationRepository;
            this.staffSectionCohortAssociationRepository = staffSectionCohortAssociationRepository;
            this.staffIdentificationCodeRepository = staffIdentificationCodeRepository;
            this.studentIdentificationCodeRepository = studentIdentificationCodeRepository;
            this.staffLinks = staffLinks;
            this.schoolLinks = schoolLinks;
            this.adminLinks = adminLinks;
            this.staffInformationLookupKeyProvider = staffInformationLookupKeyProvider;
            this.searchProvider = searchProvider;
        }
        
        [CanBeAuthorizedBy(AuthorizationDelegate.Search)]
        [NoCache]
        public SearchModel Get(SearchRequest request)
        {
            var results = new SearchModel();
            IQueryable<SchoolSearchModel> schoolSearch = null;
            IQueryable<TeacherSearchModel> teacherSearch = null;
            IQueryable<SearchModel.StudentSearchData> studentSearch = null;
            IEnumerable<SearchModel.StaffSearchItem> staffSearch = null;

            if (String.IsNullOrWhiteSpace(request.TextToFind))
                return results;

            if (request.SearchFilters != null)
            {
                foreach (var value in request.SearchFilters.Where(value => request.PageFilter == SearchFilter.None || (request.PageFilter & value.Key) == value.Key))
                {
                    switch (value.Key)
                    {
                        case SearchFilter.Schools:
                            schoolSearch = GetSchoolsThatMatch(request.TextToFind, request.MatchContains, value.Value);
                            break;
                        case SearchFilter.Teachers:
                            teacherSearch = GetAllTeachersThatMatch(request.TextToFind, request.MatchContains, value.Value);
                            break;
                        case SearchFilter.Staff:
                            staffSearch = GetAllStaffThatMatch(request.TextToFind, request.MatchContains, value.Value);
                            break;
                        case SearchFilter.Students:
                            studentSearch = GetAllStudentsThatMatch(request.TextToFind, request.MatchContains, value.Value);
                            break;
                    }
                }
            }

            if (schoolSearch != null)
            {
                var schoolData = schoolSearch;
                var schoolDataList = schoolData.Take(request.RowCountToReturn).ToList();
                results.Schools = (from c in schoolDataList
                                   select new SearchModel.SearchItem
                                              {
                                                  Text = c.Name,
                                                  SchoolId = c.SchoolId,
                                                  Link = new Link { Href = schoolLinks.Overview(c.SchoolId, c.Name) }
                                              }).ToList();
                results.AbsoluteSchoolsCount = results.Schools.Count < request.RowCountToReturn ? results.Schools.Count : schoolData.Count();
            }

            if (teacherSearch != null)
            {
                var teacherData = teacherSearch;
                var teacherDataList = teacherData.Take(request.RowCountToReturn).ToList();
                results.Teachers = (from t in teacherDataList
                                    select new SearchModel.TeacherSearchItem
                                                   {
                                                       Text = Utilities.FormatPersonNameByLastName(t.FirstName, t.MiddleName, t.LastSurname),
                                                       StaffUSI = t.StaffUSI,
                                                       SchoolId = t.SchoolId,
                                                       School = t.School,
                                                       FirstName = t.FirstName,
                                                       MiddleName = t.MiddleName,
                                                       LastSurname = t.LastSurname,
                                                       Link = new Link { Href = staffLinks.Default(t.SchoolId, t.StaffUSI, t.FullName) }
                                                   }).ToList();
                results.AbsoluteTeachersCount = results.Teachers.Count < request.RowCountToReturn ? results.Teachers.Count : teacherData.Count();
            }

            if (staffSearch != null)
            {
                var staffData = staffSearch;
                results.Staff = staffData.Take(request.RowCountToReturn).ToList();
                results.AbsoluteStaffCount = results.Staff.Count < request.RowCountToReturn ? results.Staff.Count : staffData.Count();
            }

            /* 
             * Sigh. As you can see we cannot complete populating the student search results at this point. The reason
             * is at this immediate point in code execution the student filtering has not yet been applied. So the
             * absolute count cannot be calculated. 
             * 
             * The rest of this code is located in SearchPresenter.FinishStudentSearch().
             * 
             */
            if (studentSearch != null)
            {
                results.StudentQuery = studentSearch;
            }

            return results;
        }

        private IQueryable<SchoolSearchModel> GetSchoolsThatMatch(string searchText, bool matchContains, IEnumerable<Tuple<int, Type>> educationOrganization)
        {
            var state = GetEducationOrganizationsOfType(educationOrganization, typeof(UserInformation.StateAgency));
            var schools = GetEducationOrganizationsOfType(educationOrganization, typeof(UserInformation.School));
            var leas = GetEducationOrganizationsOfType(educationOrganization, typeof(UserInformation.LocalEducationAgency));

            if (!(state.Any() || schools.Any() || leas.Any()))
                return null;
            
            searchText = searchText.Replace(",", "").Replace(".", "");

            var baseQuery = (from c in schoolInformationRepository.GetAll()
                         select c);

            if (matchContains)
                baseQuery = from s in baseQuery
                        where s.Name.Contains(searchText.Replace(" ", "%"))
                        select s;
            else
                baseQuery = from s in baseQuery
                        where s.Name.StartsWith(searchText)
                        select s;

            if (!state.Any())
            {
                if (schools.Any() && leas.Any())
                    baseQuery = from s in baseQuery
                                where schools.Contains(s.SchoolId) || leas.Contains(s.LocalEducationAgencyId)
                                select s;
                else if (schools.Any())
                    baseQuery = baseQuery.Where(x => schools.Contains(x.SchoolId));
                else if (leas.Any())
                    baseQuery = baseQuery.Where(x => leas.Contains(x.LocalEducationAgencyId));
            }
            return (from s in baseQuery 
                    orderby s.Name 
                    select new SchoolSearchModel
                                   {
                                       SchoolId = s.SchoolId,
                                       Name = s.Name
                                   }); 
        }

        private IQueryable<TeacherSearchModel> GetAllTeachersThatMatch(string searchText, bool matchContains, IEnumerable<Tuple<int, Type>> educationOrganization)
        {
            var state = GetEducationOrganizationsOfType(educationOrganization, typeof(UserInformation.StateAgency));
            var schools = GetEducationOrganizationsOfType(educationOrganization, typeof(UserInformation.School));
            var leas = GetEducationOrganizationsOfType(educationOrganization, typeof(UserInformation.LocalEducationAgency));
            if (!(state.Any() || schools.Any() || leas.Any()))
                return null;
            
            searchText = searchText.Replace(",", "").Replace(".", "");
            var baseQuery = (
                            from t in staffInformationRepository.GetAll()
                            join ts in staffSectionCohortAssociationRepository.GetAll() on t.StaffUSI equals ts.StaffUSI
                            join seoi in staffEducationOrganizationRepository.GetAll() on new { ts.StaffUSI, ts.EducationOrganizationId } equals new { seoi.StaffUSI, seoi.EducationOrganizationId }
                            join org in schoolInformationRepository.GetAll() on ts.EducationOrganizationId equals org.SchoolId
							join sic in staffIdentificationCodeRepository.GetAll() on t.StaffUSI equals sic.StaffUSI
                            select new {t,ts,seoi,org,sic}
                        );

            if (!state.Any())
            {
                if (schools.Any() && leas.Any())
                    baseQuery = from s in baseQuery
                                where schools.Contains(s.org.SchoolId) || leas.Contains(s.org.LocalEducationAgencyId)
                                select s;
                else if (schools.Any())
                    baseQuery = baseQuery.Where(x => schools.Contains(x.org.SchoolId));
                else if (leas.Any())
                    baseQuery = baseQuery.Where(x => leas.Contains(x.org.LocalEducationAgencyId));
            }

            var revisedBaseQuery = from s in baseQuery
                                   group s by new
                                                  {
                                                      s.t.StaffUSI,
													  s.sic.IdentificationCode,
                                                      s.t.FullName,
                                                      s.t.FirstName,
                                                      s.t.MiddleName,
                                                      s.t.LastSurname,
                                                      s.ts.EducationOrganizationId,
                                                      s.org.Name
                                                  }
                                   into g
                                   select
                                       new
                                           {
                                               g.Key.StaffUSI,
											   g.Key.IdentificationCode,
                                               g.Key.FullName,
                                               g.Key.FirstName,
                                               g.Key.MiddleName,
                                               g.Key.LastSurname,
                                               g.Key.EducationOrganizationId,
                                               g.Key.Name
                                           };



            //Choose the search option to apply.
            //1) for starts with
            //2) for contains
            //3) for 2 words ([first]|[last]) and starts with.
            int searchOptionToUse = GetSearchOption(searchText, matchContains);
            var wordsToSearch = searchText.Split(' ');
            int inputUsi;
            bool isNumericInput = int.TryParse(searchText.Trim(), out inputUsi);
            var startsWithQueryFilter = isNumericInput ? revisedBaseQuery.Where(x => x.StaffUSI.ToString().StartsWith(searchText) ||
				x.IdentificationCode.ToString().StartsWith(searchText.Replace(" ", "%"))) :
                revisedBaseQuery.Where(x => x.FullName.StartsWith(searchText));
	        var containsQueryFilter = isNumericInput
		        ? revisedBaseQuery.Where(
			        x =>
				        x.StaffUSI.ToString().Contains(searchText.Replace(" ", "%")) ||
				        x.IdentificationCode.ToString().Contains(searchText.Replace(" ", "%")))
		        : revisedBaseQuery.Where(x => x.FullName.Contains(searchText.Replace(" ", "%")));
            var startsWithFirstAndLastQueryFilter = revisedBaseQuery.Where(x => (x.FirstName.StartsWith(wordsToSearch[0]) && x.LastSurname.StartsWith(wordsToSearch[1])) || (x.FirstName.StartsWith(wordsToSearch[1]) && x.LastSurname.StartsWith(wordsToSearch[0])));
            var startsWithFirstMiddlAndLastQueryFilter = revisedBaseQuery.Where(x => (x.FirstName.StartsWith(wordsToSearch[0]) && x.MiddleName.StartsWith(wordsToSearch[1]) && x.LastSurname.StartsWith(wordsToSearch[2])) || (x.FirstName.StartsWith(wordsToSearch[1]) && x.MiddleName.StartsWith(wordsToSearch[2]) && x.LastSurname.StartsWith(wordsToSearch[0])));

            var teachersFilteredByName =
                (searchOptionToUse == 1) ? startsWithQueryFilter :
                (searchOptionToUse == 2) ? containsQueryFilter :
                (searchOptionToUse == 3) ? startsWithFirstAndLastQueryFilter : startsWithFirstMiddlAndLastQueryFilter;

            var q = (from t in teachersFilteredByName
                     orderby t.LastSurname, t.FirstName, t.MiddleName, t.Name
                     select new TeacherSearchModel
                     {
                         StaffUSI = t.StaffUSI,
                         FirstName = t.FirstName,
                         MiddleName = t.MiddleName,
                         LastSurname = t.LastSurname,
                         FullName = t.FullName,
                         SchoolId = t.EducationOrganizationId,
                         School = t.Name,
                     }
                    ).Distinct();

            return q;
        }

        private IEnumerable<SearchModel.StaffSearchItem> GetAllStaffThatMatch(string searchText, bool matchContains, IEnumerable<Tuple<int, Type>> educationOrganization)
        {
            var state = GetEducationOrganizationsOfType(educationOrganization, typeof(UserInformation.StateAgency));
            var leas = GetEducationOrganizationsOfType(educationOrganization, typeof(UserInformation.LocalEducationAgency));
            if (!(state.Any() || leas.Any()))
                return null;

            searchText = searchText.Replace(",", "").Replace(".", "");

            var baseQuery = (
                from t in staffInformationRepository.GetAll()
                from ts in System.Linq.Queryable.Where(staffEducationOrganizationRepository.GetAll(), x => x.StaffUSI == t.StaffUSI)
                from org in System.Linq.Queryable.Where(schoolInformationRepository.GetAll(), x=> 1 == 1)
                from sid in System.Linq.Queryable.Where(staffIdentificationCodeRepository.GetAll(), x => x.StaffUSI == t.StaffUSI).DefaultIfEmpty()
                where ts.EducationOrganizationId == org.SchoolId || ts.EducationOrganizationId == org.LocalEducationAgencyId
                orderby t.FullName
                select new { t, sid, org }
                );

            if (!state.Any())
            {
                 if (leas.Any())
                    baseQuery = baseQuery.Where(x => leas.Contains(x.org.LocalEducationAgencyId));
            }

             var revisedBaseQuery = from s in baseQuery
                        group s by new
                            {
                                s.t.StaffUSI,
                                s.t.FullName,
                                s.t.FirstName,
                                s.t.MiddleName,
                                s.t.LastSurname,
                                s.t.EmailAddress,
                                s.sid.StaffIdentificationSystemType,
                                s.sid.IdentificationCode
                            } into g
                            select new { g.Key.StaffUSI, g.Key.FullName, g.Key.FirstName, g.Key.MiddleName, g.Key.LastSurname, g.Key.EmailAddress, g.Key.StaffIdentificationSystemType, g.Key.IdentificationCode };

            //Choose the search option to apply.
            //1) for starts with
            //2) for contains
            //3) for 2 words ([first]|[last]) and starts with.
            int searchOptionToUse = GetSearchOption(searchText, matchContains);
            var wordsToSearch = searchText.Split(' ');
            int inputUsi;
            bool isNumericInput = int.TryParse(searchText.Trim(), out inputUsi);
            var startsWithQueryFilter = isNumericInput ? revisedBaseQuery.Where(x => x.StaffUSI.ToString().StartsWith(searchText) || x.IdentificationCode.ToString().StartsWith(searchText)) :
                revisedBaseQuery.Where(x => x.FullName.StartsWith(searchText) || x.IdentificationCode.StartsWith(searchText));
            var containsQueryFilter = isNumericInput ? revisedBaseQuery.Where(x => x.StaffUSI.ToString().Contains(searchText.Replace(" ", "%")) || x.IdentificationCode.ToString().Contains(searchText.Replace(" ", "%"))) :
                revisedBaseQuery.Where(x => x.FullName.Contains(searchText.Replace(" ", "%")) || x.IdentificationCode.Contains(searchText.Replace(" ", "%"))); 
            var startsWithFirstAndLastQueryFilter = revisedBaseQuery.Where(x => (x.FirstName.StartsWith(wordsToSearch[0]) && x.LastSurname.StartsWith(wordsToSearch[1])) || (x.FirstName.StartsWith(wordsToSearch[1]) && x.LastSurname.StartsWith(wordsToSearch[0])));
            var startsWithFirstMiddlAndLastQueryFilter = revisedBaseQuery.Where(x => (x.FirstName.StartsWith(wordsToSearch[0]) && x.MiddleName.StartsWith(wordsToSearch[1]) && x.LastSurname.StartsWith(wordsToSearch[2])) || (x.FirstName.StartsWith(wordsToSearch[1]) && x.MiddleName.StartsWith(wordsToSearch[2]) && x.LastSurname.StartsWith(wordsToSearch[0])));

            var staffFilteredByName =
                (searchOptionToUse == 1) ? startsWithQueryFilter :
                (searchOptionToUse == 2) ? containsQueryFilter :
                (searchOptionToUse == 3) ? startsWithFirstAndLastQueryFilter : startsWithFirstMiddlAndLastQueryFilter;
            //TODO:Review this: This is doing the same logic as the DashboardUserClaimsInformationProvider for obtaining the login information.
            //This introduces an issue in a MultiLEA in one DB scenario wherein if an Email address is non-unique between the LEAs all users sharing the 
            //email address are returned regardless of their educational organization. NOTE: Impersonation of non-associated EdOrg users is NOT possible.
            var staffUSIs = Enumerable.ToArray((from staff in staffFilteredByName
                                      select staff.StaffUSI));

            var matchingStaffInfo = staffInfoAndOrgQuery.Execute(staffUSIs);

            // must reapply the filter to the results for staff identification code filtering
            switch (searchOptionToUse)
            {
                case 1: // startsWith
                    matchingStaffInfo = isNumericInput ? matchingStaffInfo.Where(x => x.StaffUSI.ToString().StartsWith(searchText) || x.IdentificationCode.ToString().StartsWith(searchText)).ToList() :
                                                         matchingStaffInfo.Where(x => x.FullName.StartsWith(searchText, StringComparison.InvariantCultureIgnoreCase) || x.IdentificationCode.StartsWith(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
                    break;
                case 2: // contains
                    matchingStaffInfo = isNumericInput ? matchingStaffInfo.Where(x => x.StaffUSI.ToString().Contains(searchText.Replace(" ", "%")) || x.IdentificationCode.ToString().Contains(searchText.Replace(" ", "%"))).ToList() :
                                                         matchingStaffInfo.Where(x => x.FullName.IndexOf(searchText.Replace(" ", "%"), StringComparison.InvariantCultureIgnoreCase) > -1 || x.IdentificationCode.IndexOf(searchText.Replace(" ", "%"), StringComparison.InvariantCultureIgnoreCase) > -1).ToList();
                    break;
                case 3: // startsWithFirstAndLast
                    matchingStaffInfo = matchingStaffInfo.Where(x => (x.FirstName.StartsWith(wordsToSearch[0], StringComparison.InvariantCultureIgnoreCase) && x.LastSurname.StartsWith(wordsToSearch[1], StringComparison.InvariantCultureIgnoreCase)) 
                        || (x.FirstName.StartsWith(wordsToSearch[1], StringComparison.InvariantCultureIgnoreCase) && x.LastSurname.StartsWith(wordsToSearch[0], StringComparison.InvariantCultureIgnoreCase))).ToList();
                    break;
                default: // startsWithFirstMiddleAndLast
                    matchingStaffInfo = matchingStaffInfo.Where(x => (x.FirstName.StartsWith(wordsToSearch[0], StringComparison.InvariantCultureIgnoreCase) && x.MiddleName.StartsWith(wordsToSearch[1], StringComparison.InvariantCultureIgnoreCase) && x.LastSurname.StartsWith(wordsToSearch[2], StringComparison.InvariantCultureIgnoreCase)) 
                        || (x.FirstName.StartsWith(wordsToSearch[1], StringComparison.InvariantCultureIgnoreCase) && x.MiddleName.StartsWith(wordsToSearch[2], StringComparison.InvariantCultureIgnoreCase) && x.LastSurname.StartsWith(wordsToSearch[0], StringComparison.InvariantCultureIgnoreCase))).ToList();
                    break;
            }

            if (matchingStaffInfo == null || !matchingStaffInfo.Any())
                return new List<SearchModel.StaffSearchItem>();

            //creating tempPreResults to not create a List<StaffInformationAndAssociatedOrganizationsQueryResult> object if it is not necessary
            IEnumerable<StaffInformationAndAssociatedOrganizationsQueryResult> tempPreResults;
            //get the staff info key
            var staffInfoLookupKey = staffInformationLookupKeyProvider.GetStaffInformationLookupKey();
            var staffInformationProperty = typeof(StaffInformationAndAssociatedOrganizationsQueryResult).GetProperty(staffInfoLookupKey);

            //if the InfoLookupKey doesnt exist in the StaffInformationAndAssociatedOrganizationsQueryResult type then do not do the for loop check, just use all the results in matchingStaffInfo
            if (staffInformationProperty == null)
                tempPreResults = matchingStaffInfo;
            else
            {
                //This for loop appears to do a check that the lookup value for the staff person is populated.
                //this would remove potential problem staff member from being added to this list it the login as button
                //this does not remove all problems that can occur with staff log ins
                var tempMatchingStaffInfo = new List<StaffInformationAndAssociatedOrganizationsQueryResult>();

                foreach (var matchingStaffInfoValue in matchingStaffInfo)
                {
                    var lookupValueObj = staffInformationProperty.GetValue(matchingStaffInfoValue, null);

                    if (lookupValueObj == null)
                        continue;

                    if (!string.IsNullOrEmpty(lookupValueObj.ToString()))
                        tempMatchingStaffInfo.Add(matchingStaffInfoValue);
                }
                tempPreResults = tempMatchingStaffInfo;
            }

            var results = from userInfo in tempPreResults
                    orderby userInfo.LastSurname, userInfo.FirstName, userInfo.FullName
                    select new SearchModel.StaffSearchItem
                    {
                        StaffUSI = userInfo.StaffUSI,
                        LeaId = userInfo.AssociatedOrganizations.First().LocalEducationAgencyId.Value,
                        Text = Utilities.FormatPersonNameByLastName(userInfo.FirstName, string.Empty, userInfo.LastSurname),
                        Email = userInfo.EmailAddress,
                        Schools =
                            userInfo.AssociatedOrganizations.Select(x => x.Name)
                                .Distinct()
                                .OrderBy(x => x)
                                .ToFormattedString(),
                        PositionTitle =
                            userInfo.AssociatedOrganizations.Select(x => x.PositionTitle)
                                .Distinct()
                                .OrderBy(x => x)
                                .ToFormattedString(),
                        IdentificationSystem = userInfo.IdentificationSystem,
                        IdentificationCode = userInfo.IdentificationCode,
                        //NOTE: For a statewide implementation this needs handle for state users if impersonation is desired at that level.
                        Link =
                            new Link
                            {
                                Href =
                                    adminLinks.LogInAs(
                                        userInfo.AssociatedOrganizations.First().LocalEducationAgencyId.Value,
                                        new {userInfo.StaffUSI})
                            }
                    };

            results = results.Where(x => searchProvider.GetEnabledStaffIdentificationSystems(x.LeaId).Contains(x.IdentificationSystem) || x.IdentificationSystem == null);

            return results.ToList();
        }

        private IQueryable<SearchModel.StudentSearchData> GetAllStudentsThatMatch(string searchText, bool matchContains, IEnumerable<Tuple<int, Type>> educationOrganization)
        {
            var schools = GetEducationOrganizationsOfType(educationOrganization, typeof(UserInformation.School));
            int leaId = EdFiDashboardContext.Current.LocalEducationAgencyId.GetValueOrDefault();

            var availIdSystems = searchProvider.GetEnabledStudentIdentificationSystems(leaId);

            searchText = searchText.Replace(",", "").Replace(".", "");

            var baseQuery = (from s in studentInformationRepository.GetAll()
                from ssi in
                    System.Linq.Queryable.Where(studentSchoolInformationRepository.GetAll(),
                        x => x.StudentUSI == s.StudentUSI)
                from c in
                    System.Linq.Queryable.Where(schoolInformationRepository.GetAll(), x => x.SchoolId == ssi.SchoolId)
                from sid in
                    System.Linq.Queryable.Where(studentIdentificationCodeRepository.GetAll(),
                        x => x.StudentUSI == s.StudentUSI).DefaultIfEmpty()

                orderby s.LastSurname, s.FirstName, s.MiddleName, c.Name

                                     select new SearchModel.StudentSearchData
                                     {
                                         StudentUSI = s.StudentUSI,
                                         FullName = s.FullName,
                                         FirstName = s.FirstName,
                                         MiddleName = s.MiddleName,
                                         LastSurname = s.LastSurname,
                                         GradeLevel = ssi.GradeLevel,
                                         SchoolId = ssi.SchoolId,
                                         School = c.Name,
                                         IdentificationSystem = sid.StudentIdentificationSystemType,
                                         StudentIdentificationCode = sid.IdentificationCode
                                     });

            if (schools.Any())
                baseQuery = baseQuery.Where(x => schools.Contains(x.SchoolId));

            // check for student id search config
            if (availIdSystems.Any())
                baseQuery = baseQuery.Where(x => availIdSystems.Contains(x.IdentificationSystem) || x.IdentificationSystem == null);
            
            //Choose the search option to apply.
            //1) for starts with
            //2) for contains
            //3) for 2 words ([first]|[last]) and starts with.
            int searchOptionToUse = GetSearchOption(searchText, matchContains);
            var wordsToSearch = searchText.Split(' ');
            int inputUsi;
            bool isNumericInput = int.TryParse(searchText.Trim(), out inputUsi);
            var startsWithQueryFilter = isNumericInput ? baseQuery.Where(x => x.StudentUSI.ToString().StartsWith(searchText) || x.StudentIdentificationCode.ToString().StartsWith(searchText)) : 
                baseQuery.Where(x => x.FullName.StartsWith(searchText) || x.StudentIdentificationCode.StartsWith(searchText));
            var containsQueryFilter = isNumericInput ? baseQuery.Where(x => x.StudentUSI.ToString().Contains(searchText.Replace(" ", "%")) || x.StudentIdentificationCode.ToString().Contains(searchText.Replace(" ", "%"))) :
                baseQuery.Where(x => x.FullName.Contains(searchText.Replace(" ", "%")) || x.StudentIdentificationCode.Contains(searchText.Replace(" ", "%")));
            var startsWithFirstAndLastQueryFilter = baseQuery.Where(x => (x.FirstName.StartsWith(wordsToSearch[0]) && x.LastSurname.StartsWith(wordsToSearch[1])) || (x.FirstName.StartsWith(wordsToSearch[1].Trim()) && x.LastSurname.StartsWith(wordsToSearch[0])));
            var startsWithFirstMiddleAndLastQueryFilter = baseQuery.Where(x => (x.FirstName.StartsWith(wordsToSearch[0]) && x.MiddleName.StartsWith(wordsToSearch[1]) && x.LastSurname.StartsWith(wordsToSearch[2])) || (x.FirstName.StartsWith(wordsToSearch[2].Trim()) && x.MiddleName.StartsWith(wordsToSearch[1]) && x.LastSurname.StartsWith(wordsToSearch[0])));

            var baseQueryFilteredByName = (searchOptionToUse == 1) ? startsWithQueryFilter :
                                            (searchOptionToUse == 2) ? containsQueryFilter :
                                            (searchOptionToUse == 3) ? startsWithFirstAndLastQueryFilter : startsWithFirstMiddleAndLastQueryFilter;

            return baseQueryFilteredByName;
        }

        //Choose the search option to apply.
        //1) for starts with
        //2) for contains
        //3) for 2 words ([first]|[last]) and starts with.
        //4) for 3 words ([first]|[middle]|[last]) and starts with.
        private static int GetSearchOption(string textToSearch, bool matchContains)
        {
            if (!matchContains)
                return 1;

            var wordsToSearch = textToSearch.Split(' ');
            if (wordsToSearch.Length == 1)
                return 2;
            
            if (wordsToSearch.Length == 2)
                return 3;

            if (wordsToSearch.Length == 3)
                return 4;

            //Default or fallback is to use contains option
            return 2;
        }

        private class TeacherSearchModel
        {
            public long StaffUSI { get; set; }
            public string FullName { get; set; }
            public string FirstName { get; set; }
            public string MiddleName { get; set; }
            public string LastSurname { get; set; }
            public int SchoolId { get; set; }
            public string School { get; set; }
        }

        private class SchoolSearchModel
        {
            public int SchoolId { get; set; }
            public string Name { get; set; }
        }

        private static int[] GetEducationOrganizationsOfType(IEnumerable<Tuple<int, Type>> educationOrgs, Type typeRequested)
        {
            return (educationOrgs.Where(x => x.Item2 == typeRequested)).Select(edOrg => edOrg.Item1).ToArray();
        }
    }
}
