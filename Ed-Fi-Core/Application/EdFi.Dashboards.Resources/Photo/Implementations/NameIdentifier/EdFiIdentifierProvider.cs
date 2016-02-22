// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Images;
using EdFi.Dashboards.Resources.Photo.Models;

namespace EdFi.Dashboards.Resources.Photo.Implementations.NameIdentifier
{
    public class EdFiIdentifierProvider : ChainOfResponsibilityBase<IIdentifierProvider, IdentifierRequest, Identifier>, IIdentifierProvider
    {
        private readonly IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        private readonly IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository;

        // These are used to implement a poor man's cache since we are bypassing the interceptors that provide real caching. We have to 
        // go straight to the repository to bypass the security as well since the person uploading the photos may not have access to the
        // student records for which the photos are being uploaded. In addition, the services don't give us as much flexibility when
        // querying staff and students.
        private Dictionary<int, IEnumerable<StaffEducationOrgInformation>> cachedStaff = new Dictionary<int, IEnumerable<StaffEducationOrgInformation>>();
        private Dictionary<int, IEnumerable<StudentSchoolInformation>> cachedStudents = new Dictionary<int, IEnumerable<StudentSchoolInformation>>();

        public EdFiIdentifierProvider(IRepository<StudentSchoolInformation> studentSchoolInformationRepository,
            IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository,
            IIdentifierProvider next)
            : base(next)
        {
            this.studentSchoolInformationRepository = studentSchoolInformationRepository;
            this.staffEducationOrgInformationRepository = staffEducationOrgInformationRepository;
        }

        public Identifier Get(IdentifierRequest request)
        {
            return ProcessRequest(request);
        }

        private IEnumerable<StaffEducationOrgInformation> GetStaff(IdentifierRequest request)
        {
            IEnumerable<StaffEducationOrgInformation> staff;

            if (cachedStaff.TryGetValue(request.SchoolId, out staff))
                return staff;

            staff = staffEducationOrgInformationRepository.GetAll().Where(
                        seoi => seoi.EducationOrganizationId == request.SchoolId).ToList();

            cachedStaff.Add(request.SchoolId, staff);

            return staff;
        }

        /// <summary>
        /// Gets a collection of students for the school.
        /// </summary>
        /// <returns>An IEnumerable for the students at the school.</returns>
        private IEnumerable<StudentSchoolInformation> GetStudents(IdentifierRequest request)
        {
            IEnumerable<StudentSchoolInformation> students;

            if (cachedStudents.TryGetValue(request.SchoolId, out students))
                return students;

            students = studentSchoolInformationRepository.GetAll().Where(
                        seoi => seoi.SchoolId == request.SchoolId).ToList();

            cachedStudents.Add(request.SchoolId, students);

            return students;
        }

        protected override bool CanHandleRequest(IdentifierRequest request)
        {
            return request.OriginalPhoto is UniqueOriginalPhoto;
        }

        protected override Identifier HandleRequest(IdentifierRequest request)
        {
            var originalPhoto = (UniqueOriginalPhoto)request.OriginalPhoto;

            // If this is a staff member, then try to match on staff.
            if (originalPhoto.ImageType == ImageType.Staff)
            {
                return
                    GetStaff(request).Where(staff => staff.StaffUSI == originalPhoto.Id).Select(
                        staff => new Identifier {Id = staff.StaffUSI, ImageType = ImageType.Staff}).SingleOrDefault();
            }
            
            if (originalPhoto.ImageType == ImageType.Student)
            {
                return
                    GetStudents(request).Where(student => student.StudentUSI == originalPhoto.Id).Select(
                        student => new Identifier { Id = student.StudentUSI, ImageType = ImageType.Student }).SingleOrDefault();
            }

            return null;
        }
    }
}
