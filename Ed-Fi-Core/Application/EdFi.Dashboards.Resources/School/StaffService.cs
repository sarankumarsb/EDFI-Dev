// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.School
{
    public class StaffRequest
    {
        public int SchoolId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaffRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="StaffRequest"/> instance.</returns>
        public static StaffRequest Create(int schoolId) 
        {
            return new StaffRequest { SchoolId = schoolId };
        }
    }

    public interface IStaffService : IService<StaffRequest, StaffModel> { }

    public class StaffService : IStaffService
    {
        private readonly IRepository<StaffInformation> staffInformationRepository;
        private readonly IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository;
        private readonly IStaffAreaLinks staffLinks;

        public StaffService(IRepository<StaffInformation> staffInformationRepository, IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository, 
            IStaffAreaLinks staffLinks)
        {
            this.staffInformationRepository = staffInformationRepository;
            this.staffEducationOrgInformationRepository = staffEducationOrgInformationRepository;
            this.staffLinks = staffLinks;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.AccessOrganization)]
        public StaffModel Get(StaffRequest request)
        {
            //Subsonic's restrictions of doing projections...
            var staffData = (from t in staffInformationRepository.GetAll()
                             join tc in staffEducationOrgInformationRepository.GetAll() on t.StaffUSI equals tc.StaffUSI
                             where tc.EducationOrganizationId == request.SchoolId
                             select new
                                        {
                                            t.StaffUSI,
                                            Name = Utilities.FormatPersonNameByLastName(t.FirstName, t.MiddleName, t.LastSurname),
                                            ProfileThumbnail = staffLinks.ListImage(request.SchoolId, t.StaffUSI, t.Gender),
                                            t.EmailAddress,
                                            t.DateOfBirth,
                                            t.Gender,
                                            tc.StaffCategory,
                                            t.YearsOfPriorProfessionalExperience,
                                            t.HighestLevelOfEducationCompleted,
                                            HighlyQualifiedTeacher = t.HighlyQualifiedTeacher.ToYesNo(),
                                        }).ToList();

            var model = new StaffModel
                            {
                                Staff = (from t in staffData
                                        select new StaffModel.StaffMember
                                                   {
                                                       StaffUSI = t.StaffUSI,
                                                       Name = t.Name,
                                                       ProfileThumbnail = t.ProfileThumbnail,
                                                       EmailAddress = t.EmailAddress,
                                                       DateOfBirth = t.DateOfBirth,
                                                       Gender = t.Gender,
                                                       StaffCategory = t.StaffCategory,
                                                       YearsOfPriorProfessionalExperience = t.YearsOfPriorProfessionalExperience,
                                                       HighestLevelOfEducationCompleted = t.HighestLevelOfEducationCompleted,
                                                       HighlyQualifiedTeacher = t.HighlyQualifiedTeacher
                                                   }).ToList()
                            };

            return model;
        }
    }
}

