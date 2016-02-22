// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.Resources.Models.Student.Detail.CurrentCourses;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public class CurrentCoursesListRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentCoursesListRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="CurrentCoursesListRequest"/> instance.</returns>
        public static CurrentCoursesListRequest Create(long studentUSI, int schoolId) 
		{
			return new CurrentCoursesListRequest { StudentUSI = studentUSI, SchoolId = schoolId };
		}
	}

    public abstract class CurrentCoursesListServiceBase<TRequest, TResponse, TSemester, TCourse, TGrade> : IService<TRequest, TResponse>
        where TRequest : CurrentCoursesListRequest
        where TResponse : CurrentCoursesModel, new()
        where TSemester : Semester, new()
        where TCourse : Course, new()
        where TGrade : Grade, new()
    {
        private const string fallSemesterTermName = "Fall Semester";
        private const string springSemesterTermName = "Spring Semester";

        public IRepository<StudentRecordCurrentCourse> Repository { get; set; }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public virtual TResponse Get(TRequest request)
        {
            var coreModel = new TResponse { StudentUSI = request.StudentUSI};
            var baseDataQuery = (from data in Repository.GetAll()
                                 where data.StudentUSI == request.StudentUSI && data.SchoolId == request.SchoolId
                                 select data);

            InitializeMappings();
            
            //Group by semester
            var semestersQuery = (from data in baseDataQuery
                                  group data by data.TermTypeId
                                  into semester
                                  select new
                                             {
                                                 SemesterTerm = semester.Key,
                                                 SemesterInfo = semester
                                             });
            var semesters = semestersQuery.ToList();

            var semesterGroupings = from s in semesters
                                   select new SemesterGrouping {TermTypeId = s.SemesterTerm, Grouping = s.SemesterInfo};
            
            //Semesters in data
            foreach (var semesterGrouping in semesterGroupings)
            {
                var semesterName = String.Empty;
                if (semesterGrouping.TermTypeId == 1)
                    semesterName = fallSemesterTermName;
                else if (semesterGrouping.TermTypeId == 2)
                    semesterName = springSemesterTermName;

                var currentSemester = new TSemester { StudentUSI = request.StudentUSI, Term = semesterName };

                var gradingPeriods = (from c in semesterGrouping.Grouping
                                     group c by c.GradingPeriod
                                     into gp
                                     select (GradingPeriod) gp.Key).OrderBy(x => x).ToList();
                currentSemester.AvailablePeriods = gradingPeriods;

                var courses = from c in semesterGrouping.Grouping
                                group c by c.LocalCourseCode
                                into lc
                                select new CourseGrouping { LocalCourseCode = lc.Key, Grouping = lc };

                //Courses in semester data
                MapSemesterInfo(courses, currentSemester, currentSemester.Term);
                PopulateCoursesInSemester(request.StudentUSI, courses, currentSemester, gradingPeriods);
                OnSemesterMapped(currentSemester, semesterGrouping);

                coreModel.Semesters.Add(currentSemester);
                OnSemesterMapped(currentSemester, semesterGrouping);
            }
            
            //Prepare grades for display.
            //Add placeholders for missing grades
            coreModel.Semesters = coreModel.Semesters.OrderBy(x => x.Term, new SemesterComparer()).ToList();
            foreach (var term in coreModel.Semesters)
                term.Courses = term.Courses.OrderBy(x => x.CourseTitle).ToList();

            OnModelMapped(coreModel, semesterGroupings);

            return coreModel;
        }

        #region Supporting Methods and Types

        private static void MapSemesterInfo(IEnumerable<CourseGrouping> courseData, TSemester semester, string semesterTermName)
        {
            foreach (var c in courseData)
            {
                foreach (var s in c.Grouping)
                {
                    semester = Mapper.Map<TSemester>(s);
                    break;
                }
                break;
            }

            semester.Term = semesterTermName;

        }

        protected class SemesterGrouping
        {
            public int TermTypeId { get; set; }
            public IGrouping<int, StudentRecordCurrentCourse> Grouping { get; set; }
        }

        /// <summary>
        /// Occurs when the model has finished mapping.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="data">Contains the data from the <see cref="StudentRecordCurrentCourse"/> entity type, grouped by academic semester term.</param>
        /// <remarks>
        /// If you have extended the StudentRecordCurrentCourse entity, the data will contain the extension concrete entity type which 
        /// you'll need to downcast in order to access the extended properties.
        /// </remarks>
        protected virtual void OnModelMapped(TResponse model, IEnumerable<SemesterGrouping> data)
        {
        }

        /// <summary>
        /// Occurs each time a semester gets mapped.
        /// </summary>
        /// <param name="semesters"></param>
        /// <param name="data">Contains the data from the <see cref="StudentRecordCurrentCourse"/> entity type, grouped by academic semester term.</param>
        /// <remarks>
        /// If you have extended the StudentRecordCurrentCourse entity, the data will contain the extension concrete entity type which 
        /// you'll need to downcast in order to access the extended properties.
        /// </remarks>
        protected virtual void OnSemesterMapped(TSemester semesters, SemesterGrouping data)
        {
        }

        protected class CourseGrouping
        {
            public string LocalCourseCode { get; set; }
            public IGrouping<string, StudentRecordCurrentCourse> Grouping { get; set; }
        }

        protected class StudentRecordCurrentCourseGrouping : List<StudentRecordCurrentCourse>, IGrouping<string, StudentRecordCurrentCourse>
        {
            public string Key { get; set; }
        }

        /// <summary>
        /// Occurs each time a course is mapped.
        /// </summary>
        /// <param name="course"></param>
        /// <param name="data">Contains the data from the <see cref="StudentRecordCurrentCourse"/> entity type, grouped by local course code.</param>
        /// <remarks>
        /// If you have extended the StudentRecordCurrentCourse entity, the data will contain the extension concrete entity type which 
        /// you'll need to downcast in order to access the extended properties.
        /// </remarks>
        protected virtual void OnCourseMapped(TCourse course, CourseGrouping data)
        {
        }

        /// <summary>
        /// Occurs each time a grade is mapped.
        /// </summary>
        /// <param name="grade"></param>
        /// <param name="data">Contains the data from the <see cref="StudentRecordCurrentCourse"/> entity type.</param>
        /// <remarks>
        /// If you have extended the StudentRecordCurrentCourse entity, the data will contain the extension concrete entity type which 
        /// you'll need to downcast in order to access the extended properties.
        /// </remarks>
        protected virtual void OnGradeMapped(TGrade grade, StudentRecordCurrentCourse data)
        {
        }

        private bool isMappingInitialized;

        private void InitializeMappings()
        {
            if (isMappingInitialized) 
                return;

            AutoMapperHelper.EnsureMapping<StudentRecordCurrentCourse, Semester, TSemester>
                (Repository, 
                ignore => ignore.Term,
                ignore => ignore.Courses,
                ignore => ignore.AvailablePeriods);

            AutoMapperHelper.EnsureMapping<StudentRecordCurrentCourse, Course, TCourse>
                (Repository,
                 ignore => ignore.Grades);

            AutoMapperHelper.EnsureMapping<StudentRecordCurrentCourse, Grade, TGrade>
                (Repository,
                 ignore => ignore.Value,
                 ignore => ignore.GradePeriod);

            isMappingInitialized = true;
        }

        #endregion

        protected virtual void PopulateCoursesInSemester(long studentUSI, IEnumerable<CourseGrouping> courses, TSemester semester, List<GradingPeriod> gradingPeriods)
        {
            foreach (var course in courses)
            {
                var grades = new List<Grade>();
                TCourse currentCourse = null;

                foreach (var gradingPeriod in gradingPeriods)
                {
                    //Grades or Scores in Course Data (Big Flat Object/Table)
                    var grade = course.Grouping.FirstOrDefault(x => x.GradingPeriod == (int)gradingPeriod);
                    
                    TGrade gradeInstance;
                    if (grade == null)
                    {
                        gradeInstance = Mapper.Map<TGrade>(new StudentRecordCurrentCourse {StudentUSI = studentUSI});
                        gradeInstance.GradePeriod = gradingPeriod;
                    }
                    else
                    {
                        if (currentCourse == null)
                        {
                            currentCourse = Mapper.Map<TCourse>(grade);
                            currentCourse.Grades = grades;
                        }

                        gradeInstance = Mapper.Map<TGrade>(grade);
                        gradeInstance.GradePeriod = (GradingPeriod)grade.GradingPeriod;
                        gradeInstance.Value = (grade.NumericGradeEarned != null) ? grade.NumericGradeEarned.ToString() : grade.LetterGradeEarned;
                    }

                    OnGradeMapped(gradeInstance, grade);

                    grades.Add(gradeInstance);

                    if (String.IsNullOrWhiteSpace(semester.Term))
                        semester.Term = grade.TermType;
                }

                OnCourseMapped(currentCourse, course);
                semester.Courses.Add(currentCourse);
            }
        }
    }

    public class SemesterComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            int val1 = Convert(x);
            int val2 = Convert(y);
            if (val1 < val2)
                return -1;
            if (val1 > val2)
                return 1;
            if (val1 != 5 && val2 != 5)
                return 0;
            return String.Compare(x, y);
        }

        private static int Convert(string x)
        {
            switch (x)
            {
                case "Fall Semester":
                    return 1;
                case "Spring Semester":
                    return 2;
                case "Summer Semester":
                    return 3;
                case "Year Round":
                    return 4;
            }
            return 5;
        }
    }

    public interface ICurrentCoursesListService : IService<CurrentCoursesListRequest, CurrentCoursesModel> {}

    public sealed class CurrentCoursesListService : CurrentCoursesListServiceBase<CurrentCoursesListRequest, CurrentCoursesModel, Semester, Course, Grade>, ICurrentCoursesListService { }
}
