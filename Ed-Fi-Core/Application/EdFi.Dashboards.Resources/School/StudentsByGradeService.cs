// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.School
{
    public class StudentsByGradeRequest
    {
        public int SchoolId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StudentsByGradeRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="StudentsByGradeRequest"/> instance.</returns>
        public static StudentsByGradeRequest Create(int schoolId) 
        {
            return new StudentsByGradeRequest { SchoolId = schoolId };
        }
    }

    public interface IStudentsByGradeService : IService<StudentsByGradeRequest, StudentsByGradeModel> { }

    public class StudentsByGradeService : IStudentsByGradeService
    {
        private readonly IRepository<StudentInformation> studentInformationRepository;
        private readonly IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        private readonly IUniqueListIdProvider uniqueListIdProvider;
        private readonly IStudentSchoolAreaLinks studentSchoolLinks;
        private readonly IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

        public StudentsByGradeService(IRepository<StudentInformation> studentInformationRepository,
                                            IRepository<StudentSchoolInformation> studentSchoolInformationRepository,
                                            IUniqueListIdProvider uniqueListIdProvider,
                                            IStudentSchoolAreaLinks studentSchoolLinks,
                                            IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider)
        {
            this.studentInformationRepository = studentInformationRepository;
            this.studentSchoolInformationRepository = studentSchoolInformationRepository;
            this.uniqueListIdProvider = uniqueListIdProvider;
            this.studentSchoolLinks = studentSchoolLinks;
            this.gradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public StudentsByGradeModel Get(StudentsByGradeRequest request)
        {
            var model = new StudentsByGradeModel {SchoolId = request.SchoolId};

            var uniqueListId = uniqueListIdProvider.GetUniqueId() + request.SchoolId;

            //Lets get the data that we need.
            var studentsInSchoolData = (from s in studentInformationRepository.GetAll()
                                        join ssi in studentSchoolInformationRepository.GetAll() on s.StudentUSI equals
                                            ssi.StudentUSI
                                        where ssi.SchoolId == request.SchoolId
                                        select new
                                                   {
                                                       s.StudentUSI,
                                                       s.FirstName,
                                                       s.MiddleName,
                                                       s.LastSurname,
                                                       s.FullName,
                                                       ssi.GradeLevel
                                                   }).ToList();

            var dataGroupedByGrade = (from s in studentsInSchoolData
                                      group s by s.GradeLevel
                                      into g
                                      select new StudentsByGradeModel.Grade
                                                 {
                                                     GradeLevel = g.Key,
                                                     Students = (from student in g
                                                                 orderby student.LastSurname , student.FirstName
                                                                 let fullName = Utilities.FormatPersonNameByLastName(
                                                                     student.FirstName,
                                                                     student.MiddleName,
                                                                     student.LastSurname)
                                                                 select
                                                                     new Student(
                                                                     student.StudentUSI)
                                                                         {
                                                                             FullName = fullName,
                                                                             Url =
                                                                                 studentSchoolLinks.Overview(
                                                                                     request.SchoolId,
                                                                                     student.StudentUSI,
                                                                                     student.FullName,
                                                                                     new {listContext = uniqueListId}),
                                                                         }).ToList()
                                                 }).ToList();

            //Lets order the grades and add them to the model.
            model.Grades = dataGroupedByGrade.OrderBy(x => x.GradeLevel, new SchoolGradeComparer(gradeLevelUtilitiesProvider)).ToList();

            return model;
        }

        /// <summary>
        /// Custom Comparer to be able to order grades.
        /// </summary>
        public class SchoolGradeComparer : IComparer<string>
        {
            private readonly IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

            public SchoolGradeComparer(IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider)
            {
                this.gradeLevelUtilitiesProvider = gradeLevelUtilitiesProvider;
            }

            public int Compare(string x, string y)
            {
                int xCompare = gradeLevelUtilitiesProvider.FormatGradeLevelForSorting(x);
                int yCompare = gradeLevelUtilitiesProvider.FormatGradeLevelForSorting(y);
                return xCompare.CompareTo(yCompare);
            }
        }
    }
}
