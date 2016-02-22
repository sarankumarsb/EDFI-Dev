// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool
{
    public class BriefRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }

        public static BriefRequest Create(long studentUSI, int schoolId)
        {
            return new BriefRequest { StudentUSI = studentUSI, SchoolId = schoolId };
        }
    }

    public interface IBriefService : IService<BriefRequest, BriefModel> { }

    public class BriefService : IBriefService
    {
        private readonly IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        private readonly IRepository<StudentInformation> studentInformationRepository;
        private readonly IAccommodationProvider accommodationProvider;
        private readonly IStudentSchoolAreaLinks studentSchoolAreaLinks;

        public BriefService(IRepository<StudentInformation> studentInformationRepository,
                                      IRepository<StudentSchoolInformation> studentSchoolInformationRepository,
                                      IAccommodationProvider accommodationProvider,
                                      IStudentSchoolAreaLinks studentSchoolAreaLinks)
        {
            this.accommodationProvider = accommodationProvider;
            this.studentInformationRepository = studentInformationRepository;
            this.studentSchoolInformationRepository = studentSchoolInformationRepository;
            this.studentSchoolAreaLinks = studentSchoolAreaLinks;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public BriefModel Get(BriefRequest request)
        {
            var studentData = studentInformationRepository.GetAll().SingleOrDefault(x => x.StudentUSI == request.StudentUSI);
            var studentSchoolData = studentSchoolInformationRepository.GetAll().SingleOrDefault(x => x.SchoolId == request.SchoolId && x.StudentUSI == request.StudentUSI);

            //Create empty one if student not found.
            var ss = new BriefModel
                            {
                                StudentUSI = -1,
                                FullName = "No student found.",
                                ProfileThumbnail = studentSchoolAreaLinks.ProfileThumbnail(request.SchoolId, request.StudentUSI, "Male"),
                                GradeLevel = "",
                                Homeroom = ""
                            };

            //If we find the student then we fill him in
            if (studentData == null)
                return ss;

            ss = new BriefModel
                        {
                            StudentUSI = studentData.StudentUSI,
                            FullName = studentData.FullName,
                            ProfileThumbnail = studentSchoolAreaLinks.ProfileThumbnail(request.SchoolId, request.StudentUSI, studentData.Gender, studentData.FullName),
                            Race = studentData.Race,
                            Gender = studentData.Gender
                        };

            //If we have school info then...
            if (studentSchoolData != null)
            {
                ss.GradeLevel = studentSchoolData.GradeLevel;
                ss.Homeroom = studentSchoolData.Homeroom;
            }

            //The student's Accommodations
            ss.Accommodations = accommodationProvider.GetAccommodations(request.StudentUSI, request.SchoolId);

            return ss;
        }
    }
}
