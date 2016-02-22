using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Application.Resources.Models.Staff;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Application.Resources.Staff
{
    public class CustomStudentListGetRequest
    {
        public int LocalEducationAgencyId { get; set; }

        public int? SchoolId { get; set; }

        public long StaffUSI { get; set; }

        public static CustomStudentListGetRequest Create(int localEducationAgency, int? schoolId, long staffUSI)
        {
            return new CustomStudentListGetRequest { LocalEducationAgencyId = localEducationAgency, SchoolId = schoolId, StaffUSI = staffUSI };
        }
    }

    public class CustomStudentListPostRequest
    {
        public int LocalEducationAgencyId { get; set; }

        public int? SchoolId { get; set; }

        public long StaffUSI { get; set; }

        public int? CustomStudentListId { get; set; }

        [AuthenticationIgnore("Not related to security")]
        public PostAction Action { get; set; }

        [AuthenticationIgnore("Irrelevant")]
        public string Description { get; set; }

        [AuthenticationIgnore("Irrelevant")]
        public IEnumerable<int> StudentUSIs { get; set; }

        public static CustomStudentListPostRequest Create(int localEducationAgency, int? schoolId, long staffUSI, int? customStudentListId, PostAction action, string description, IEnumerable<int> studentUSIs)
        {
            return new CustomStudentListPostRequest { LocalEducationAgencyId = localEducationAgency, SchoolId = schoolId, StaffUSI = staffUSI, CustomStudentListId = customStudentListId, Action = action, Description = description, StudentUSIs = studentUSIs };
        }
    }

    public interface ICustomStudentListService : IService<CustomStudentListGetRequest, IEnumerable<CustomStudentListModel>>,
                                                 IPostHandler<CustomStudentListPostRequest, string> { }

    public class CustomStudentListService : ICustomStudentListService
    {
        private readonly ICacheProvider cacheProvider;
        private readonly IPersistingRepository<StaffCustomStudentList> staffCustomStudentListRepository;
        private readonly IPersistingRepository<StaffCustomStudentListStudent> staffCustomStudentListStudentRepository;
        private readonly ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks;
        private readonly IStaffAreaLinks staffAreaLinks;

        public CustomStudentListService(IPersistingRepository<StaffCustomStudentList> staffCustomStudentListRepository, IPersistingRepository<StaffCustomStudentListStudent> staffCustomStudentListStudentRepository, ICacheProvider cacheProvider, ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks, IStaffAreaLinks staffAreaLinks)
        {
            this.staffCustomStudentListRepository = staffCustomStudentListRepository;
            this.staffCustomStudentListStudentRepository = staffCustomStudentListStudentRepository;
            this.cacheProvider = cacheProvider;
            this.localEducationAgencyAreaLinks = localEducationAgencyAreaLinks;
            this.staffAreaLinks = staffAreaLinks;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public IEnumerable<CustomStudentListModel> Get(CustomStudentListGetRequest request)
        {
            var localEducationAgencyId = request.LocalEducationAgencyId;
            var staffUSI = request.StaffUSI;
            var schoolId = request.SchoolId;

            var studentLists = (from w in staffCustomStudentListRepository.GetAll()
                                where w.EducationOrganizationId == localEducationAgencyId
                                      && w.StaffUSI == staffUSI
                                select w).ToList();
            var lists = studentLists.Select(studentList => new CustomStudentListModel
                                                               {
                                                                   StaffUSI = studentList.StaffUSI,
                                                                   CustomStudentListId = studentList.StaffCustomStudentListId,
                                                                   Description = studentList.CustomStudentListIdentifier,
                                                                   EducationOrganizationId = studentList.EducationOrganizationId,
                                                                   Href = localEducationAgencyAreaLinks.StudentList(localEducationAgencyId, staffUSI, null, studentList.StaffCustomStudentListId, StudentListType.CustomStudentList.ToString())
                                                               }).ToList();

            if (schoolId.HasValue)
            {
                var schoolStudentLists = (from w in staffCustomStudentListRepository.GetAll()
                                          where w.EducationOrganizationId == schoolId.Value
                                                && w.StaffUSI == staffUSI
                                          select w).ToList();
                var schoolLists = schoolStudentLists.Select(studentList => new CustomStudentListModel
                                                                               {
                                                                                   StaffUSI = studentList.StaffUSI,
                                                                                   CustomStudentListId = studentList.StaffCustomStudentListId,
                                                                                   Description = studentList.CustomStudentListIdentifier,
                                                                                   EducationOrganizationId = studentList.EducationOrganizationId,
                                                                                   Href = staffAreaLinks.Default(schoolId.Value, staffUSI, null, studentList.StaffCustomStudentListId, StudentListType.CustomStudentList.ToString())
                                                                               }).ToList();
                lists.AddRange(schoolLists);
            }

            return lists;
        }

        /// <summary>
        /// Updates the collection of student identifiers associated with a specific staff watch list.
        /// </summary>
        /// <param name="request">The request to process.</param>
        /// <returns>A true if the operation is successful; otherwise throws an exception.</returns>
        [NoCache]
        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public string Post(CustomStudentListPostRequest request)
        {
            var educationOrganizationId = request.SchoolId ?? request.LocalEducationAgencyId;
            var customStudentListId = request.CustomStudentListId;
            var staffUSI = request.StaffUSI;
            var studentUSIs = request.StudentUSIs;
            var description = request.Description;

            bool modifiedData = false;

            switch (request.Action)
            {
                case PostAction.Remove:
                    var removedStudents = RemoveStudents(educationOrganizationId, staffUSI, customStudentListId, studentUSIs);
                    var deletedList = DeleteCustomStudentListIfEmpty(educationOrganizationId, staffUSI, customStudentListId);
                    modifiedData = removedStudents || deletedList;
                    break;

                case PostAction.Delete:
                    modifiedData = DeleteCustomStudentList(educationOrganizationId, staffUSI, customStudentListId);
                    break;

                case PostAction.Set:
                    modifiedData = RenameCustomStudentList(educationOrganizationId, staffUSI, customStudentListId, description);
                    break;

                case PostAction.Add:
                    if (studentUSIs != null && studentUSIs.Any())
                    {
                        customStudentListId = CreateCustomStudentList(educationOrganizationId, staffUSI, customStudentListId, description);
                        modifiedData = AddStudents(staffUSI, customStudentListId, studentUSIs);
                    }
                    break;
            }

            // only clear the cache if we've actually made changes
            if (modifiedData)
                ClearRelevantCaches(customStudentListId, request.LocalEducationAgencyId, request.SchoolId, staffUSI);

            StaffCustomStudentList customStudentList = null;
            if (customStudentListId.HasValue)
                customStudentList = staffCustomStudentListRepository.GetAll().FirstOrDefault(x => x.StaffUSI == staffUSI && x.StaffCustomStudentListId == customStudentListId.Value);

            if (customStudentList != null && customStudentList.EducationOrganizationId == request.LocalEducationAgencyId)
                return localEducationAgencyAreaLinks.StudentList(customStudentList.EducationOrganizationId, staffUSI, null, customStudentListId, StudentListType.CustomStudentList.ToString());
            if (customStudentList != null)
                return staffAreaLinks.Default(customStudentList.EducationOrganizationId, staffUSI, null, customStudentListId, StudentListType.CustomStudentList.ToString());
            if (request.SchoolId.HasValue)
                return staffAreaLinks.Default(request.SchoolId.Value, staffUSI);
            return localEducationAgencyAreaLinks.StudentList(request.LocalEducationAgencyId, staffUSI);
        }

        private void ClearRelevantCaches(int? customStudentListId, int localEducationAgencyId, int? schoolId, long staffUSI)
        {
            // Clear all cache keys associated with the modified SectionOrCohortId
            if (customStudentListId.HasValue)
                cacheProvider.RemoveCachedObjects(string.Format("SectionOrCohortId:{0},", customStudentListId));

            //remove district level cached objects if needed
            var cacheKey = string.Format("CustomStudentListService.Get$LocalEducationAgencyId:{0},StaffUSI:{1}", localEducationAgencyId, staffUSI);
            cacheProvider.RemoveCachedObjects(cacheKey);

            //remove district level cached objects if needed
            cacheKey = string.Format("StudentListService.Get$LocalEducationAgencyId:{0},StaffUSI:{1}", localEducationAgencyId, staffUSI);
            cacheProvider.RemoveCachedObjects(cacheKey);

            // Student Lists at the School use StudentListService
            cacheKey = string.Format("StudentListService.Get$LocalEducationAgencyId:{0},SchoolId:{1},StaffUSI:{2}", localEducationAgencyId, schoolId, staffUSI);
            cacheProvider.RemoveCachedObjects(cacheKey);

            // Student Lists at the School use CustomStudentListService
            cacheKey = string.Format("CustomStudentListService.Get$LocalEducationAgencyId:{0},SchoolId:{1},StaffUSI:{2}", localEducationAgencyId, schoolId, staffUSI);
            cacheProvider.RemoveCachedObjects(cacheKey);

            // Student List Drop List on default view
            cacheKey = string.Format("StaffService.Get$StaffUSI:{0},SchoolId:{1}", staffUSI, schoolId);
            cacheProvider.RemoveCachedObjects(cacheKey);

            // Clear the default section for the staff
            cacheKey = string.Format("DefaultSectionService.Get$SchoolId:{1},StaffUSI:{0}", staffUSI, schoolId);
            cacheProvider.RemoveCachedObjects(cacheKey);

            if (schoolId.HasValue) return;
            // Student Lists at the LEA use the StudentListMenuService
            cacheKey = string.Format("StudentListMenuService.Get$LocalEducationAgencyId:{0},StaffUSI:{1}", localEducationAgencyId, staffUSI);
            cacheProvider.RemoveCachedObjects(cacheKey);
        }

        private int CreateCustomStudentList(int educationOrganizationId, long staffUSI, int? customStudentListId, string description)
        {
            if (!customStudentListId.HasValue || !staffCustomStudentListRepository.GetAll().Any(x => x.StaffCustomStudentListId == customStudentListId.Value && x.StaffUSI == staffUSI))
            {
                if (!string.IsNullOrEmpty(description) && description.Length > 100)
                    description = description.Substring(0, 100);

                var customStudentList = new StaffCustomStudentList
                                    {
                                        EducationOrganizationId = educationOrganizationId,
                                        StaffUSI = staffUSI,
                                        CustomStudentListIdentifier = description,
                                    };

                staffCustomStudentListRepository.Save(customStudentList);
                return customStudentList.StaffCustomStudentListId;
            }

            return customStudentListId.Value;
        }

        private bool AddStudents(long staffUSI, int? customStudentListId, IEnumerable<int> studentUSIs)
        {
            if (!customStudentListId.HasValue)
                return false;

            if (!staffCustomStudentListRepository.GetAll().Any(x => x.StaffUSI == staffUSI && x.StaffCustomStudentListId == customStudentListId.Value))
                return false;

            var addedStudents = false;
            if (studentUSIs != null && studentUSIs.Any())
            {
                //add items to student repo
                foreach (var studentUSI in studentUSIs)
                {
                    if (!staffCustomStudentListStudentRepository.GetAll().Any(x => x.StudentUSI == studentUSI && x.StaffCustomStudentListId == customStudentListId.Value))
                    {
                        var student = new StaffCustomStudentListStudent
                                          {
                                              StaffCustomStudentListId = customStudentListId.Value,
                                              StudentUSI = studentUSI
                                          };

                        staffCustomStudentListStudentRepository.Save(student);
                        addedStudents = true;
                    }
                }
            }
            return addedStudents;
        }

        private bool RemoveStudents(int educationOrganizationId, long staffUSI, int? customStudentListId, IEnumerable<int> studentUSIs)
        {
            if (!customStudentListId.HasValue)
                return false;

            if (!staffCustomStudentListRepository.GetAll().Any(x => x.EducationOrganizationId == educationOrganizationId && x.StaffUSI == staffUSI && x.StaffCustomStudentListId == customStudentListId.Value))
                return false;

            bool deletedStudents = false;
            if (studentUSIs != null && studentUSIs.Any())
            {
                //delete specified students from student repo
                foreach (var studentUSI in studentUSIs)
                {
                    staffCustomStudentListStudentRepository.Delete(x => x.StaffCustomStudentListId == customStudentListId.Value && x.StudentUSI == studentUSI);
                    deletedStudents = true;
                }
            }
            return deletedStudents;
        }

        private bool DeleteCustomStudentListIfEmpty(int educationOrganizationId, long staffUSI, int? customStudentListId)
        {
            if (!customStudentListId.HasValue)
                return false;

            if (!staffCustomStudentListRepository.GetAll().Any(x => x.EducationOrganizationId == educationOrganizationId && x.StaffUSI == staffUSI && x.StaffCustomStudentListId == customStudentListId.Value))
                return false;

            if (!staffCustomStudentListStudentRepository.GetAll().Any(x => x.StaffCustomStudentListId == customStudentListId.Value))
            {
                staffCustomStudentListRepository.Delete(x => x.StaffCustomStudentListId == customStudentListId.Value);
                return true;
            }
            return false;
        }

        private bool DeleteCustomStudentList(int educationOrganizationId, long staffUSI, int? customStudentListId)
        {
            if (!customStudentListId.HasValue)
                return false;

            if (!staffCustomStudentListRepository.GetAll().Any(x => x.EducationOrganizationId == educationOrganizationId && x.StaffUSI == staffUSI && x.StaffCustomStudentListId == customStudentListId.Value))
                return false;

            staffCustomStudentListStudentRepository.Delete(x => x.StaffCustomStudentListId == customStudentListId.Value);
            staffCustomStudentListRepository.Delete(x => x.StaffCustomStudentListId == customStudentListId.Value);
            return true;
        }

        private bool RenameCustomStudentList(int educationOrganizationId, long staffUSI, int? customStudentListId, string description)
        {
            if (!customStudentListId.HasValue)
                return false;

            var customStudentList = (from csl in staffCustomStudentListRepository.GetAll()
                                     where csl.EducationOrganizationId == educationOrganizationId
                                           && csl.StaffUSI == staffUSI
                                           && csl.StaffCustomStudentListId == customStudentListId.Value
                                     select csl).FirstOrDefault();

            if (customStudentList == null)
                return false;

            if (!string.IsNullOrEmpty(description) && description.Length > 100)
                description = description.Substring(0, 100);

            customStudentList.CustomStudentListIdentifier = description;
            staffCustomStudentListRepository.Save(customStudentList);
            return true;
        }
    }
}
