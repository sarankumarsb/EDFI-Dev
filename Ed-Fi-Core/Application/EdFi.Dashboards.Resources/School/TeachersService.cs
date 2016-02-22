// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.School
{
    public class TeachersRequest
    {
        public int SchoolId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TeachersRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="TeachersRequest"/> instance.</returns>
        public static TeachersRequest Create(int schoolId) 
        {
            return new TeachersRequest { SchoolId = schoolId };
        }
    }

    public interface ITeachersService : IService<TeachersRequest, TeachersModel> { }

    public class TeachersService : ITeachersService
    {
        private readonly IRepository<StaffInformation> staffInformationRepository;
        private readonly IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository;
        private readonly IRepository<StaffSectionCohortAssociation> staffSectionCohortAssociationRepository;
        private readonly IUniqueListIdProvider uniqueListIdProvider;
        private readonly IStaffAreaLinks staffLinks;

        public TeachersService(IRepository<StaffInformation> staffInformationRepository, IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository, 
            IRepository<StaffSectionCohortAssociation> staffSectionCohortAssociationRepository, IUniqueListIdProvider uniqueListIdProvider, IStaffAreaLinks staffLinks)
        {
            this.staffInformationRepository = staffInformationRepository;
            this.staffEducationOrgInformationRepository = staffEducationOrgInformationRepository;
            this.staffSectionCohortAssociationRepository = staffSectionCohortAssociationRepository;
            this.uniqueListIdProvider = uniqueListIdProvider;
            this.staffLinks = staffLinks;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllTeachers)]
        public TeachersModel Get(TeachersRequest request)
        {
            int schoolId = request.SchoolId;

            var allTeachers = (from si in staffInformationRepository.GetAll()
                              join c in staffEducationOrgInformationRepository.GetAll() on si.StaffUSI equals c.StaffUSI
                              join ssca in staffSectionCohortAssociationRepository.GetAll() on new { c.StaffUSI, c.EducationOrganizationId } equals new { ssca.StaffUSI, ssca.EducationOrganizationId }
                              where c.EducationOrganizationId == schoolId
                              orderby si.FullName
                              select si).Distinct().ToList();

            string uniqueListId = uniqueListIdProvider.GetUniqueId();

            return new TeachersModel
                       {
                           Teachers = (from t in allTeachers
                                      select new TeachersModel.Teacher
                                                 {
                                                     StaffUSI = t.StaffUSI,
                                                     Name = Utilities.FormatPersonNameByLastName(t.FirstName, t.MiddleName, t.LastSurname),
                                                     Url = staffLinks.Default(schoolId, t.StaffUSI, t.FullName, null, null, new { listContext = uniqueListId }),
                                                     ProfileThumbnail = staffLinks.ListImage(request.SchoolId, t.StaffUSI, t.Gender).Resolve(),
                                                     EmailAddress = t.EmailAddress,
                                                     DateOfBirth = t.DateOfBirth,
                                                     Gender = t.Gender,
                                                     YearsOfPriorProfessionalExperience = t.YearsOfPriorProfessionalExperience,
                                                     HighestLevelOfEducationCompleted = t.HighestLevelOfEducationCompleted,
                                                     HighlyQualifiedTeacher = t.HighlyQualifiedTeacher.ToYesNo(),
                                                 }).ToList()
                       };
        }
    }
}
