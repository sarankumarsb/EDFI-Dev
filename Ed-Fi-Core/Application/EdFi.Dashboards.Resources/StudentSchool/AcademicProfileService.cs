// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Models.Student.Detail.AssessmentHistory;
using EdFi.Dashboards.Resources.Models.Student.Detail.CourseHistory;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Models.Student.AcademicProfile;
using EdFi.Dashboards.Resources.Models.Student.Detail.CurrentCourses;
using EdFi.Dashboards.Resources.StudentSchool.Detail;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool
{
    public class AcademicProfileRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }

        public static AcademicProfileRequest Create(long studentUSI, int schoolId)
        {
            var request = new AcademicProfileRequest {SchoolId = schoolId, StudentUSI = studentUSI};

            return request;
        }
    }

    public abstract class AcademicProfileServiceBase<TRequest, TResponse, TCurrentCourses, TCourseHistory, TAssessmentHistory> : IService<TRequest, TResponse>
        where TRequest:AcademicProfileRequest
        where TResponse : AcademicProfileModel
        where TCurrentCourses : CurrentCoursesModel
        where TCourseHistory : CourseHistoryModel
        where TAssessmentHistory : AssessmentHistoryModel
    {
        // To optimize for the extensibility experience, we inject the dependencies through properties.
        // Note the use of the IService<,> - used to ensure extended services are wired up
        //public ICurrentCoursesListService CurrentCourseService { get; set; }
        public IService<CurrentCoursesListRequest, CurrentCoursesModel> CurrentCourseService { get; set; }
        public IService<CourseHistoryListRequest, CourseHistoryModel> CourseHistoryService { get; set; }
        public IService<AssessmentHistoryRequest, AssessmentHistoryModel> AssessmentHistoryService { get; set; }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public virtual TResponse Get(TRequest request)
        {
            var model = (TResponse)Activator.CreateInstance(typeof(TResponse));

            model.StudentUSI = request.StudentUSI;
            
            var currentCoursesRequest = new CurrentCoursesListRequest { StudentUSI = request.StudentUSI, SchoolId = request.SchoolId};
            model.CurrentCourses = CurrentCourseService.Get(currentCoursesRequest);

            var courseHistoryRequest = new CourseHistoryListRequest {StudentUSI = request.StudentUSI};
            model.CourseHistory = CourseHistoryService.Get(courseHistoryRequest);

            var assessmentHistoryRequest = new AssessmentHistoryRequest {StudentUSI = request.StudentUSI};
            model.AssessmentHistory = AssessmentHistoryService.Get(assessmentHistoryRequest);

            return model;
        }
    }

    public interface IAcademicProfileService : IService<AcademicProfileRequest, AcademicProfileModel> { }

    //This is the concrete implementation of the base.
    public sealed class AcademicProfileService : AcademicProfileServiceBase<AcademicProfileRequest, AcademicProfileModel, CurrentCoursesModel, CourseHistoryModel, AssessmentHistoryModel>, IAcademicProfileService { }
}
