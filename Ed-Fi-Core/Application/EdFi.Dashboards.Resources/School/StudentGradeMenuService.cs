using System;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Support;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.School
{
    public class StudentGradeMenuRequest
    {
        public int SchoolId { get; set; }

        public static StudentGradeMenuRequest Create(int schoolId)
        {
            return new StudentGradeMenuRequest { SchoolId = schoolId };
        }
    }

    public interface IStudentGradeMenuService : IService<StudentGradeMenuRequest, StudentGradeMenuModel> { }

    public class StudentGradeMenuService : IStudentGradeMenuService
    {
        private readonly IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        private readonly ISchoolAreaLinks schoolAreaLinks;
        private readonly IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

        public StudentGradeMenuService(IRepository<StudentSchoolInformation> studentSchoolInformationRepository,
                                            ISchoolAreaLinks schoolAreaLinks,
                                            IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider)
        {
            this.studentSchoolInformationRepository = studentSchoolInformationRepository;
            this.schoolAreaLinks = schoolAreaLinks;
            this.gradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentGradeMenuModel Get(StudentGradeMenuRequest request)
        {
            var model = new StudentGradeMenuModel();

            var gradeData = (from studentSchool in studentSchoolInformationRepository.GetAll()
                            where studentSchool.SchoolId == request.SchoolId && studentSchool.GradeLevel != null
                            group studentSchool by studentSchool.GradeLevel
                            into uniqueGrades
                            select uniqueGrades.Key).ToList();

            var grades = gradeData.OrderBy(x => x, new StudentsByGradeService.SchoolGradeComparer(gradeLevelUtilitiesProvider)).ToList();

            model.Grades.Add(new AttributeItemWithUrl<decimal> { Attribute = StudentGradeMenuModel.AllGradesItemText, Url = schoolAreaLinks.StudentGradeList(request.SchoolId, null, "All") });
            foreach(var grade in grades)
                model.Grades.Add(new AttributeItemWithUrl<decimal> { Attribute = grade, Url = schoolAreaLinks.StudentGradeList(request.SchoolId, null, grade) });

            return model;
        }
    }

    public class StudentGradeMenuUserContextApplicator : IUserContextApplicator
    {
        private const string listContext = "listContext";
        private readonly IHttpRequestProvider httpRequestProvider;

        public StudentGradeMenuUserContextApplicator(IHttpRequestProvider httpRequestProvider)
        {
            this.httpRequestProvider = httpRequestProvider;
        }

        public Type ModelType
        {
            get { return typeof(StudentGradeMenuModel); }
        }

        public void ApplyUserContextToModel(object modelAsObject, object requestAsObject)
        {
            var model = modelAsObject as StudentGradeMenuModel;

            // Should never happen
            if (model == null)
                return;

            var requestListContext = httpRequestProvider.GetValue(listContext);

            // Skip processing if there's no list context to apply
            if (string.IsNullOrEmpty(requestListContext))
                return;

            requestListContext = listContext + "=" + requestListContext;

            // Add the list context to each link in the model
            foreach (var grades in model.Grades)
                grades.Url = grades.Url.AppendParameters(requestListContext);
        }
    }
}
