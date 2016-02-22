using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Data.Queries
{
    public class QEduLoginInformationAndAssociatedOrganizationsByUserNameQuery
    {

        #region Dependencies and Constructor

        // Common
        private readonly IRepository<LoginDetails> loginDetailsRepository;
        private readonly IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository;
        private readonly IRepository<SchoolInformation> schoolInformationRepository;

        // For Staff
        private readonly IRepository<StaffInformation> staffInformationRepository;
        private readonly IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository;
        private readonly IRepository<StaffIdentificationCode> staffIdentificationCodeRepository;

        // For Student
        private readonly IRepository<StudentInformation> studentInformationRepository;
        private readonly IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        private readonly IRepository<StudentIdentificationCode> studentIdentificationCodeRepository;

        public QEduLoginInformationAndAssociatedOrganizationsByUserNameQuery(
            IRepository<LoginDetails> loginDetailsRepository,
            IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository,
            IRepository<SchoolInformation> schoolInformationRepository,
            IRepository<StaffInformation> staffInformationRepository,
            IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository,
            IRepository<StaffIdentificationCode> staffIdentificationCodeRepository,
            IRepository<StudentInformation> studentInformationRepository,
            IRepository<StudentSchoolInformation> studentSchoolInformationRepository,
            IRepository<StudentIdentificationCode> studentIdentificationCodeRepository)
        {
            this.loginDetailsRepository = loginDetailsRepository;
            this.localEducationAgencyInformationRepository = localEducationAgencyInformationRepository;
            this.schoolInformationRepository = schoolInformationRepository;

            this.staffInformationRepository = staffInformationRepository;
            this.staffEducationOrgInformationRepository = staffEducationOrgInformationRepository;
            this.staffIdentificationCodeRepository = staffIdentificationCodeRepository;

            this.studentInformationRepository = studentInformationRepository;
            this.studentSchoolInformationRepository = studentSchoolInformationRepository;
            this.studentIdentificationCodeRepository = studentIdentificationCodeRepository;
        }

        #endregion

        public IEnumerable<StaffInformationAndAssociatedOrganizationsQueryResult> Execute(string[] username)
        {
            if (username.Length == 0)
                return new List<StaffInformationAndAssociatedOrganizationsQueryResult>();

            //var loginDetails = loginDetailsRepository.GetAll().Where(cond => USIs.Equals(cond.USI)).FirstOrDefault();
            var loginDetails = (from login in loginDetailsRepository.GetAll()
                                where username.Contains(login.LoginId)
                                select new { login.USI, login.LoginId, login.UserType}).FirstOrDefault(); // VIN25112015

            long USI = (long)loginDetails.USI;

            if (loginDetails.UserType == 1)
                return GetStaffInfoDetails(USI, 1);
            else if (loginDetails.UserType == 2)
                return GetStudentInfoDetails(USI, 2);
            else
                return new List<StaffInformationAndAssociatedOrganizationsQueryResult>();
        }

        private IEnumerable<StaffInformationAndAssociatedOrganizationsQueryResult> GetStaffInfoDetails(long USIs, int userType)
        {
            var userInfo =
                (from staff in staffInformationRepository.GetAll()
                 from seo in staffEducationOrgInformationRepository.GetAll()
                     .Where(x => x.StaffUSI == staff.StaffUSI)
                     .DefaultIfEmpty()
                 from lea in localEducationAgencyInformationRepository.GetAll()
                     .Where(x => x.LocalEducationAgencyId == seo.EducationOrganizationId)
                     .DefaultIfEmpty()
                 from school in schoolInformationRepository.GetAll()
                     .Where(x => x.SchoolId == seo.EducationOrganizationId)
                     .DefaultIfEmpty()
                 from staffId in staffIdentificationCodeRepository.GetAll()
                     .Where(x => x.StaffUSI == staff.StaffUSI)
                     .DefaultIfEmpty()
                 where staff.StaffUSI.Equals(USIs)
                 //where staffUSIs.Contains(staff.StaffUSI)
                 select new
                 {
                     staff.FullName,
                     staff.FirstName,
                     staff.MiddleName,
                     staff.LastSurname,
                     staff.StaffUSI,
                     staff.EmailAddress,
                     EducationOrganizationId = school != null ? (int?)school.SchoolId : (int?)lea.LocalEducationAgencyId,
                     EducationOrganizationName = school != null ? school.Name : lea.Name,
                     OrganizationCategory = school != null ? EducationOrganizationCategory.School.ToString() : EducationOrganizationCategory.LocalEducationAgency.ToString(),
                     seo.StaffCategory,
                     seo.PositionTitle,
                     LocalEducationAgencyId = school != null ? (int?)school.LocalEducationAgencyId : (int?)lea.LocalEducationAgencyId,
                     IdentificationSystem = staffId != null ? staffId.StaffIdentificationSystemType : null,
                     IdentificationCode = staffId != null ? staffId.IdentificationCode : null
                 })
                    .ToList();

            var orgsGroupedByStaff =
                (from u in userInfo
                 group u by new
                 {
                     u.FullName,
                     u.FirstName,
                     u.MiddleName,
                     u.LastSurname,
                     u.StaffUSI,
                     u.EmailAddress,
                     u.IdentificationSystem,
                     u.IdentificationCode
                 } into g
                 select new
                 {
                     g.Key,
                     Orgs =
                         from o in g
                         select new
                         {
                             o.EducationOrganizationId,
                             o.OrganizationCategory,
                             o.StaffCategory,
                             o.EducationOrganizationName,
                             o.PositionTitle,
                             o.LocalEducationAgencyId,
                         }
                 });

            return
                (from g in orgsGroupedByStaff
                 select new StaffInformationAndAssociatedOrganizationsQueryResult
                 {
                     FullName = g.Key.FullName,
                     FirstName = g.Key.FirstName,
                     MiddleName = g.Key.MiddleName,
                     LastSurname = g.Key.LastSurname,
                     StaffUSI = g.Key.StaffUSI,
                     EmailAddress = g.Key.EmailAddress,
                     IdentificationSystem = g.Key.IdentificationSystem,
                     IdentificationCode = g.Key.IdentificationCode,
                     AssociatedOrganizations =
                         (from u in g.Orgs
                          let category = (EducationOrganizationCategory)Enum.Parse(typeof(EducationOrganizationCategory), u.OrganizationCategory)
                          select new StaffInformationAndAssociatedOrganizationsQueryResult.EducationOrganization
                          {
                              EducationOrganizationId = u.EducationOrganizationId.Value,
                              OrganizationCategory = category,
                              StaffCategory = u.StaffCategory,
                              Name = u.EducationOrganizationName,
                              PositionTitle = u.PositionTitle,
                              LocalEducationAgencyId = u.LocalEducationAgencyId,
                          }).ToList(),
                     UserType = userType
                 }).ToList();
        }

        private IEnumerable<StaffInformationAndAssociatedOrganizationsQueryResult> GetStudentInfoDetails(long USIs, int userType)
        {
            var userInfo =
               (from student in studentInformationRepository.GetAll()
                from seo in studentSchoolInformationRepository.GetAll()
                    .Where(x => x.StudentUSI == student.StudentUSI)
                    .DefaultIfEmpty()
                from school in schoolInformationRepository.GetAll()
                    .Where(x => x.SchoolId == seo.SchoolId)
                    .DefaultIfEmpty()
                from studentId in studentIdentificationCodeRepository.GetAll()
                    .Where(x => x.StudentUSI == student.StudentUSI)
                    .DefaultIfEmpty()
                where student.StudentUSI.Equals(USIs)
                //where staffUSIs.Contains(staff.StaffUSI)
                select new
                {
                    student.FullName,
                    student.FirstName,
                    student.MiddleName,
                    student.LastSurname,
                    student.StudentUSI,
                    student.EmailAddress,
                    EducationOrganizationId = school != null ? (int?)school.SchoolId : 0,
                    EducationOrganizationName = school != null ? school.Name : "",
                    OrganizationCategory = school != null ? EducationOrganizationCategory.School.ToString() : EducationOrganizationCategory.LocalEducationAgency.ToString(),
                    seo.StudentCategory,
                    seo.PositionTitle,
                    LocalEducationAgencyId = school != null ? (int?)school.LocalEducationAgencyId : 0,
                    IdentificationSystem = studentId != null ? studentId.StudentIdentificationSystemType : null,
                    IdentificationCode = studentId != null ? studentId.IdentificationCode : null
                })
                   .ToList();

            var orgsGroupedByStaff =
                (from u in userInfo
                 group u by new
                 {
                     u.FullName,
                     u.FirstName,
                     u.MiddleName,
                     u.LastSurname,
                     u.StudentUSI,
                     u.EmailAddress,
                     u.IdentificationSystem,
                     u.IdentificationCode
                 } into g
                 select new
                 {
                     g.Key,
                     Orgs =
                         from o in g
                         select new
                         {
                             o.EducationOrganizationId,
                             o.OrganizationCategory,
                             o.StudentCategory,
                             o.EducationOrganizationName,
                             o.PositionTitle,
                             o.LocalEducationAgencyId,
                         }
                 });

            return
                (from g in orgsGroupedByStaff
                 select new StaffInformationAndAssociatedOrganizationsQueryResult
                 {
                     FullName = g.Key.FullName,
                     FirstName = g.Key.FirstName,
                     MiddleName = g.Key.MiddleName,
                     LastSurname = g.Key.LastSurname,
                     StaffUSI = g.Key.StudentUSI,
                     EmailAddress = g.Key.EmailAddress,
                     IdentificationSystem = g.Key.IdentificationSystem,
                     IdentificationCode = g.Key.IdentificationCode,
                     AssociatedOrganizations =
                         (from u in g.Orgs
                          let category = (EducationOrganizationCategory)Enum.Parse(typeof(EducationOrganizationCategory), u.OrganizationCategory)
                          select new StaffInformationAndAssociatedOrganizationsQueryResult.EducationOrganization
                          {
                              EducationOrganizationId = u.EducationOrganizationId.Value,
                              OrganizationCategory = category,
                              StaffCategory = u.StudentCategory,
                              Name = u.EducationOrganizationName,
                              PositionTitle = u.PositionTitle,
                              LocalEducationAgencyId = u.LocalEducationAgencyId,
                          }).ToList(),
                     UserType = userType
                 }).ToList();
        }

    }
}
