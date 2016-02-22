using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    public class StudentListMenuRequest
    {
        public int LocalEducationAgencyId { get; set; }
        public long StaffUSI { get; set; }
        public long SectionOrCohortId { get; set; }
        public string StudentListType { get; set; }

        public static StudentListMenuRequest Create(int localEducationAgencyId, long staffUSI, long sectionOrCohortId, string studentListType)
        {
            return new StudentListMenuRequest { LocalEducationAgencyId = localEducationAgencyId, StaffUSI = staffUSI, SectionOrCohortId = sectionOrCohortId, StudentListType = studentListType };
        }
    }

    public interface IStudentListMenuService : IService<StudentListMenuRequest, IEnumerable<StudentListMenuModel>> {}

    public class StudentListMenuService : IStudentListMenuService
    {
        private const string seeAllStudents = "All Students";

        private readonly IRepository<StaffCohort> staffCohortRepository;
        private readonly IRepository<StaffCustomStudentList> staffCustomStudentListRepository;
        private readonly ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks;

        public StudentListMenuService(IRepository<StaffCohort> staffCohortRepository, IRepository<StaffCustomStudentList> staffCustomStudentListRepository, ILocalEducationAgencyAreaLinks localEducationAgencyAreaLinks)
        {
            this.staffCohortRepository = staffCohortRepository;
            this.staffCustomStudentListRepository = staffCustomStudentListRepository;
            this.localEducationAgencyAreaLinks = localEducationAgencyAreaLinks;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public IEnumerable<StudentListMenuModel> Get(StudentListMenuRequest request)
        {
            long staffUSI = request.StaffUSI;
            int localEducationAgencyId = request.LocalEducationAgencyId;
            long sectionOrCohortId = request.SectionOrCohortId;
            string studentListType = request.StudentListType;

            var model = new List<StudentListMenuModel>();

            model.Add(new StudentListMenuModel
                                          {
                                               Description = seeAllStudents,
                                               ListType = StudentListType.All,
                                               SectionId = 0,
                                               Href = localEducationAgencyAreaLinks.StudentList(localEducationAgencyId, staffUSI, null, null, StudentListType.All.ToString()),
                                               Selected = studentListType == StudentListType.All.ToString()
                                           });

            var cohorts = (from c in staffCohortRepository.GetAll()
                           where c.StaffUSI == staffUSI && c.EducationOrganizationId == localEducationAgencyId
                           orderby c.CohortDescription, c.StaffCohortId
                          select c).ToList();

            foreach (var cohort in cohorts)
            {
                model.Add(new StudentListMenuModel
                                               {
                                                   Description = cohort.CohortDescription, 
                                                   ListType = StudentListType.Cohort, 
                                                   SectionId = cohort.StaffCohortId, 
                                                   Href = localEducationAgencyAreaLinks.StudentList(localEducationAgencyId, staffUSI, null, cohort.StaffCohortId, StudentListType.Cohort.ToString()),
                                                   Selected = sectionOrCohortId == cohort.StaffCohortId && studentListType == StudentListType.Cohort.ToString()
                                               });
            }

            var customStudentLists = (from w in staffCustomStudentListRepository.GetAll()
                              where w.StaffUSI == staffUSI && w.EducationOrganizationId == localEducationAgencyId
                              orderby w.CustomStudentListIdentifier, w.StaffCustomStudentListId
                             select w).ToList();

            foreach (var customStudentList in customStudentLists)
            {
                model.Add(new StudentListMenuModel
                                                    {
                                                        Description = customStudentList.CustomStudentListIdentifier,
                                                        ListType = StudentListType.CustomStudentList,
                                                        SectionId = customStudentList.StaffCustomStudentListId,
                                                        Href = localEducationAgencyAreaLinks.StudentList(localEducationAgencyId, staffUSI, null, customStudentList.StaffCustomStudentListId, StudentListType.CustomStudentList.ToString()),
                                                        Selected = sectionOrCohortId == customStudentList.StaffCustomStudentListId && studentListType == StudentListType.CustomStudentList.ToString()
                                                    });
            }

            if (!model.Any(x => x.Selected))
            {
                if (model.Count == 1)
                    model[0].Selected = true;
                else
                    model[1].Selected = true;
            }


            return model;
        }

    }
}
