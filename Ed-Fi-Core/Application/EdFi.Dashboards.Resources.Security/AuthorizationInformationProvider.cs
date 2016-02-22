// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Queries;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security
{
	public class AuthorizationInformationProvider : IAuthorizationInformationProvider
	{
		private readonly StaffInformationAndAssociatedOrganizationsByEmailAddressesQuery staffInfoAndOrgQuery;
		private readonly IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository;
		private readonly IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
		private readonly IRepository<TeacherSection> teacherSectionRepository;
        private readonly IRepository<StaffCohort> staffCohortRepository;
        private readonly IRepository<StaffCustomStudentList> staffCustomStudentListRepository;
	    private readonly IRepository<SchoolInformation> schoolInformationRepository;
	    private readonly IRepository<StaffStudentCohort> staffStudentCohortRepository;
        private readonly IRepository<TeacherStudentSection> teacherStudentSectionRepository;
        private readonly IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository;
	    protected readonly IRepository<MetricBasedWatchList> metricBasedWatchListRepository;

	    private readonly IRepository<StaffStudentAssociation> staffStudentAssociationRepository;

        public AuthorizationInformationProvider(
            StaffInformationAndAssociatedOrganizationsByEmailAddressesQuery staffInfoAndOrgQuery,
            IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository,
            IRepository<StudentSchoolInformation> studentSchoolInformationRepository,
            IRepository<TeacherSection> teacherSectionRepository,
            IRepository<StaffCohort> staffCohortRepository,
            IRepository<StaffStudentCohort> staffStudentCohortRepository,
            IRepository<TeacherStudentSection> teacherStudentSectionRepository,
            IRepository<StaffStudentAssociation> staffStudentAssociationRepository, 
            IRepository<StaffCustomStudentList> staffCustomStudentListRepository,
            IRepository<SchoolInformation> schoolInformationRepository,
            IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository,
            IRepository<MetricBasedWatchList> metricBasedWatchListRepository
            )
		{
			this.staffInfoAndOrgQuery = staffInfoAndOrgQuery;
			this.staffEducationOrgInformationRepository = staffEducationOrgInformationRepository;
			this.studentSchoolInformationRepository = studentSchoolInformationRepository;
			this.teacherSectionRepository = teacherSectionRepository;
			this.staffCohortRepository = staffCohortRepository;
			this.staffStudentCohortRepository = staffStudentCohortRepository;
			this.teacherStudentSectionRepository = teacherStudentSectionRepository;
		    this.staffStudentAssociationRepository = staffStudentAssociationRepository;
		    this.staffCustomStudentListRepository = staffCustomStudentListRepository;
		    this.schoolInformationRepository = schoolInformationRepository;
            this.localEducationAgencyInformationRepository = localEducationAgencyInformationRepository;
            this.metricBasedWatchListRepository = metricBasedWatchListRepository;
		}

        // TODO: Consider renaming this to GetCurrentUsersStudentUSIs, and expanding cache attribute to support custom cache key generators
        // We'd then need this method to not accept any arguments, and a custom cache key generator that supplied the cache key based on the 
        // current users's staffUSI.
        public virtual HashSet<long> GetAllStaffStudentUSIs(long staffUSI)
        {
            UserInformation user = UserInformation.Current;

            // We need the staffUSI on the signature for the cache key generation, but really we need to use the current user's claims 
            // as the basis for finding all the school-level associated students.
            if (user.StaffUSI != staffUSI)
                throw new ArgumentException("The implementation of the method is dependent on the current user, but for method result caching purposes the 'staffUSI' argument is present on the method signature.  However, it must match the current user's staff USI.");

            var studentUSIs = new HashSet<long>();

            // EDFIDB-139
            if (user.UserType == 1)
            {
                var studentsFromLEAViewAllStudentsClaims = GetIQueryableForStudentUSIsAtLocalEducationAgencyLevelClaims(user.StaffUSI);

                foreach (long studentUSI in studentsFromLEAViewAllStudentsClaims)
                    studentUSIs.Add(studentUSI);

                var studentsFromSchoolViewAllStudentsClaims = GetPrincipalStudentUSIs(user.StaffUSI);

                foreach (long studentUSI in studentsFromSchoolViewAllStudentsClaims)
                    studentUSIs.Add(studentUSI);

                var studentsFromViewMyStudentsClaims = GetTeacherStudentUSIs(user.StaffUSI);

                foreach (long studentUSI in studentsFromViewMyStudentsClaims)
                    studentUSIs.Add(studentUSI);

                var cohortStudentsFromViewMyStudentsClaims = GetStaffCohortStudentUSIs(user.StaffUSI);

                foreach (long studentUSI in cohortStudentsFromViewMyStudentsClaims)
                    studentUSIs.Add(studentUSI);
            }
            else
            {
                studentUSIs.Add(user.StaffUSI);
            }

            return studentUSIs;
        }

		public List<long> GetPrincipalStudentUSIs(long staffUSI)
		{
			return (GetIQueryableForPrincipalStudentUSIs(staffUSI)).ToList();
		}

        [NoCache]
        public virtual IQueryable<long> GetIQueryableForPrincipalStudentUSIs(long staffUSI)
        {
            //return from staff in staffEducationOrgInformationRepository.GetAll()
            //       join student in studentSchoolInformationRepository.GetAll() on staff.EducationOrganizationId equals student.SchoolId
            //       where staff.StaffUSI == staffUSI
            //       select student.StudentUSI;

            // Get schools where current user has ViewAllStudents claims
            var schoolIds =
                from s in UserInformation.Current.AssociatedSchools
                where s.ClaimTypes.Contains(EdFiClaimTypes.ViewAllStudents)
                select s.SchoolId;

            if (!schoolIds.Any())
                return new List<long>().AsQueryable();

            // Return all student Ids for students in those schools
            return from student in studentSchoolInformationRepository.GetAll()
                   where schoolIds.Contains(student.SchoolId)
                   select student.StudentUSI;
        }

        [NoCache]
        public virtual IQueryable<long> GetIQueryableForStudentUSIsAtLocalEducationAgencyLevelClaims(long staffUSI)
        {
            // Get LEAs where current user has ViewAllStudents claims
            var localEducationAgencyIds =
                from s in UserInformation.Current.AssociatedLocalEducationAgencies
                where s.ClaimTypes.Contains(EdFiClaimTypes.ViewAllStudents)
                select s.LocalEducationAgencyId;

            if (!localEducationAgencyIds.Any())
                return new List<long>().AsQueryable();

            // Return all student Ids for students in those LEAs
            return from student in studentSchoolInformationRepository.GetAll()
                   join school in schoolInformationRepository.GetAll() on student.SchoolId equals school.SchoolId
                   where localEducationAgencyIds.Contains(school.LocalEducationAgencyId)
                   select student.StudentUSI;
        }

		public List<long> GetSchoolSectionIds(int schoolId)
		{
			return
				(from ts in teacherSectionRepository.GetAll()
				 where ts.SchoolId == schoolId 
				 select (long)ts.TeacherSectionId).ToList();
		}

        public List<long> GetEducationOrganizationCohortIds(int educationOrganizationId)
		{
			return (from ts in staffCohortRepository.GetAll()
                    where ts.EducationOrganizationId == educationOrganizationId
				    select ts.StaffCohortId).ToList();
		}

        public List<long> GetEducationOrganizationCustomStudentListIds(int educationOrganizationId)
        {
            return (from ts in staffCustomStudentListRepository.GetAll()
                    where ts.EducationOrganizationId == educationOrganizationId
                    select (long)ts.StaffCustomStudentListId).ToList();
        }

	    public List<long> GetEducationOrganizationCustomMetricsBasedWatchListIds(int educationOrganizationId)
	    {
	        return (from ts in metricBasedWatchListRepository.GetAll()
	            where ts.EducationOrganizationId == educationOrganizationId
	            select (long) ts.MetricBasedWatchListId).ToList();
	    }

		public List<long> GetSchoolStudentUSIs(int schoolId)
		{
			return
				(from info in studentSchoolInformationRepository.GetAll()
				 where info.SchoolId == schoolId 
				 select info.StudentUSI).ToList();
		}

		public List<long> GetTeacherSectionIds(long staffUSI)
		{
			return
				(from ts in teacherSectionRepository.GetAll()
				 where ts.StaffUSI == staffUSI 
				 select (long)ts.TeacherSectionId).ToList();
		}

		public List<long> GetStaffCohortIds(long staffUSI)
		{
			return
				(from ts in staffCohortRepository.GetAll()
				 where ts.StaffUSI == staffUSI
				 select ts.StaffCohortId).ToList();
		}
        
        public List<long> GetStaffCustomStudentListIds(long staffUSI)
        {
            return (from ts in staffCustomStudentListRepository.GetAll()
                     where ts.StaffUSI == staffUSI
                     select (long)ts.StaffCustomStudentListId).ToList();
        }

	    public List<long> GetStaffCustomMetricsBasedWatchListIds(long staffUSI)
	    {
	        return (from ts in metricBasedWatchListRepository.GetAll()
	            where ts.StaffUSI == staffUSI
	            select (long) ts.MetricBasedWatchListId).ToList();
	    } 

        public virtual List<long> GetTeacherStudentUSIs(long staffUSI)
        {
            return
                (from ts in teacherSectionRepository.GetAll()
                 where ts.StaffUSI == staffUSI
                 from tss in teacherStudentSectionRepository.GetAll()
                 where tss.TeacherSectionId == ts.TeacherSectionId
                 select tss.StudentUSI)
                    .ToList();
        }

        public virtual List<long> GetSchoolStaffUSIs(int schoolId)
        {
            return
                (from s in schoolInformationRepository.GetAll()
                 where s.SchoolId == schoolId
                 from so in staffEducationOrgInformationRepository.GetAll()
                 where so.EducationOrganizationId == s.SchoolId || so.EducationOrganizationId == s.LocalEducationAgencyId
                 select so.StaffUSI)
                    .ToList();
        }

        public virtual List<long> GetLocalEducationAgencyStaffUSIs(int localEducationAgencyId)
        {
            return
                (from lea in localEducationAgencyInformationRepository.GetAll()
                 where lea.LocalEducationAgencyId == localEducationAgencyId
                 from so in staffEducationOrgInformationRepository.GetAll()
                 where so.EducationOrganizationId == lea.LocalEducationAgencyId
                 select so.StaffUSI)
                    .ToList();
        }

        public virtual List<long> GetStaffCohortStudentUSIs(long staffUSI)
        {
            return
                (from sc in staffCohortRepository.GetAll()
                 where sc.StaffUSI == staffUSI
                 from ssc in staffStudentCohortRepository.GetAll()
                 where sc.StaffCohortId == ssc.StaffCohortId
                 select ssc.StudentUSI)
                    .ToList();
        }

        public virtual List<int> GetStudentSchoolIds(long studentUSI)
        {
            return ((from info in studentSchoolInformationRepository.GetAll()
                     where info.StudentUSI == studentUSI
                     select info.SchoolId))
                     .OrderBy(i => i)
                     .ToList();
        }

        /// <summary>
        /// Returns a list ids of all the Education Organization the staff member is associated with.
        /// </summary>
        /// <param name="staffUsi"></param>
        /// <returns></returns>
        public virtual List<int> GetAssociatedEducationOrganizations(long staffUsi)
        {
            return 
                (from s in staffEducationOrgInformationRepository.GetAll()
                 where s.StaffUSI == staffUsi
                 select s.EducationOrganizationId)
                 .ToList();
        }

        public virtual bool IsStudentAssociatedWithStaffAtSchool(long studentUSI, long staffUSI, int schoolId)
        {
            var found = (from ssa in staffStudentAssociationRepository.GetAll()
                         where ((ssa.StudentUSI == studentUSI) && (ssa.StaffUSI == staffUSI) && (ssa.SchoolId == schoolId))
                         select ssa)
                         .FirstOrDefault();

            return (found != null);
        }

        public virtual bool IsStudentAssociatedWithSchool(long studentUSI, int schoolId)
        {
            var found = (from info in studentSchoolInformationRepository.GetAll()
                         where info.SchoolId == schoolId && info.StudentUSI == studentUSI
                         select info).FirstOrDefault();
            return (found != null);
        }
    }
}