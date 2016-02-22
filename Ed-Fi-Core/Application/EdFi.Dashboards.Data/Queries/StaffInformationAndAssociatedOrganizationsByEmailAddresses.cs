// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;

namespace EdFi.Dashboards.Data.Queries
{
	public class StaffInformationAndAssociatedOrganizationsByEmailAddressesQuery
	{        
		#region Dependencies and Constructor

		private readonly IRepository<StaffInformation> staffInformationRepository;
		private readonly IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository;
		private readonly IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository;
		private readonly IRepository<SchoolInformation> schoolInformationRepository;

		public StaffInformationAndAssociatedOrganizationsByEmailAddressesQuery(
			IRepository<StaffInformation> staffInformationRepository,
			IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository,
			IRepository<LocalEducationAgencyInformation> localEducationAgencyInformationRepository,
			IRepository<SchoolInformation> schoolInformationRepository)
		{
			this.staffInformationRepository = staffInformationRepository;
			this.staffEducationOrgInformationRepository = staffEducationOrgInformationRepository;
			this.localEducationAgencyInformationRepository = localEducationAgencyInformationRepository;
			this.schoolInformationRepository = schoolInformationRepository;
		}

		#endregion

        public IEnumerable<StaffInformationAndAssociatedOrganizationsQueryResult> Execute(string[] emailAddresses)
		{
            if (emailAddresses == null || emailAddresses.Length == 0)
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
				where emailAddresses.Contains(staff.EmailAddress)
				select new
					{
						staff.FullName,
						staff.FirstName,
						staff.LastSurname,
						staff.StaffUSI,
						staff.EmailAddress,
						EducationOrganizationId = school != null ? (int?)school.SchoolId : (int?)lea.LocalEducationAgencyId,
						EducationOrganizationName = school != null ? school.Name : lea.Name,
						OrganizationCategory = school != null ? EducationOrganizationCategory.School.ToString() : EducationOrganizationCategory.LocalEducationAgency.ToString(),
						seo.StaffCategory,
						seo.PositionTitle,
                        LocalEducationAgencyId = school != null ? (int?)school.LocalEducationAgencyId : (int?)lea.LocalEducationAgencyId
					})
					.ToList();

			var orgsGroupedByStaff =
				(from u in userInfo
				 group u by new
								{
									u.FullName,
									u.FirstName,
									u.LastSurname,
									u.StaffUSI,
									u.EmailAddress
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
					 LastSurname = g.Key.LastSurname,
					 StaffUSI = g.Key.StaffUSI,
					 EmailAddress = g.Key.EmailAddress,
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

			// If returning the join, use a result like so:
			//        select new Result
			//            {
			//                StaffInformation = staff,
			//                LocalEducationAgencyInformation = lea,
			//                SchoolInformation = school,
			//                StaffEducationOrgInformation = seo,
			//            };
			//}
		}
	}
}