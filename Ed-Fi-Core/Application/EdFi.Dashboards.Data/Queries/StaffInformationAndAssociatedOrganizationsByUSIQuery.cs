using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;

namespace EdFi.Dashboards.Data.Queries
{
    public class StaffInformationAndAssociatedOrganizationsByUSIQuery
    {        
		#region Dependencies and Constructor

		private readonly IRepository<StaffInformation> staffInformationRepository;
		private readonly IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository;
		private readonly IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository;
		private readonly IRepository<SchoolInformation> schoolInformationRepository;
		private readonly IRepository<StaffIdentificationCode> staffIdentificationCodeRepository;

        public StaffInformationAndAssociatedOrganizationsByUSIQuery(
			IRepository<StaffInformation> staffInformationRepository,
			IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository,
			IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository,
			IRepository<SchoolInformation> schoolInformationRepository,
            IRepository<StaffIdentificationCode> staffIdentificationCodeRepository)
		{
			this.staffInformationRepository = staffInformationRepository;
			this.staffEducationOrgInformationRepository = staffEducationOrgInformationRepository;
			this.localEducationAgencyInformationRepository = localEducationAgencyInformationRepository;
			this.schoolInformationRepository = schoolInformationRepository;
            this.staffIdentificationCodeRepository = staffIdentificationCodeRepository;
		}

		#endregion

        public IEnumerable<StaffInformationAndAssociatedOrganizationsQueryResult> Execute(long[] staffUSIs)
		{
            if (staffUSIs == null || staffUSIs.Length == 0)
                return new List<StaffInformationAndAssociatedOrganizationsQueryResult>();

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
                    .Where(x => x.StaffUSI  == staff.StaffUSI)
                    .DefaultIfEmpty()
                 where staffUSIs.Contains(staff.StaffUSI)
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
				 }).ToList();
		}
    }
}
