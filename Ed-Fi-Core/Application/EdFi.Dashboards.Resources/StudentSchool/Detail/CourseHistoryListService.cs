// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using AutoMapper;
using System.Linq;
using System.Collections.Generic;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Student.Detail.CourseHistory;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.StudentSchool.Detail
{
    public class CourseHistoryListRequest
    {
        public long StudentUSI { get; set; }
        public int SchoolId { get; set; }

        [AuthenticationIgnore("SubjectArea does not affect the results of the request in a way requiring it to be secured.")]
        public string SubjectArea { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="CourseHistoryListRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="CourseHistoryListRequest"/> instance.</returns>
        public static CourseHistoryListRequest Create(long studentUSI, int schoolId, string subjectArea = null) 
		{
			return new CourseHistoryListRequest { StudentUSI = studentUSI, SchoolId = schoolId, SubjectArea = subjectArea };
		}
	}

    public abstract class CourseHistoryListServiceBase<TRequest, TResponse, TSubjectArea, TCourse, TSemester, TGrade> : IService<TRequest,TResponse>
        where TRequest : CourseHistoryListRequest
        where TResponse : CourseHistoryModel, new()
        where TSubjectArea : SubjectArea, new()
        where TCourse : Course, new()
        where TSemester : Semester, new()
        where TGrade : Grade, new()
    {
        public IRepository<StudentRecordCourseHistory> Repository { get; set; }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public virtual TResponse Get(TRequest request)
        {
            var coreModel = new TResponse
                            {
                                StudentUSI = request.StudentUSI
                            };

            var courseHistoryData = Repository.GetAll().Where(x => x.StudentUSI == request.StudentUSI && (String.IsNullOrEmpty(request.SubjectArea) || x.SubjectArea == request.SubjectArea)).ToList();
            var subjects = courseHistoryData.GroupBy(x => x.SubjectArea).OrderBy(y => y.Key, new SubjectAreaComparer());

            InitializeMappings();

            foreach (var subject in subjects)
            {
                var courses = from c in subject.AsQueryable()
                              orderby c.SchoolYear, c.TermType, c.CourseTitle
                              select c;

                var sa = CreateSubjectArea(request.StudentUSI, subject.Key, courses);
                coreModel.SubjectAreas.Add(sa);
            }

            coreModel.CumulativeCreditsEarned = coreModel.SubjectAreas.Sum(x => x.TotalCreditsEarned);

            OnModelMapped(coreModel, courseHistoryData);
            return coreModel;
        }

        private TSubjectArea CreateSubjectArea(long studentUSI, string subjectArea, IEnumerable<StudentRecordCourseHistory> dataModel)
        {
            var sa = new TSubjectArea{ StudentUSI = studentUSI,  Name = subjectArea };

            //Lets try to auto map this model.
            var sampleRow = dataModel.FirstOrDefault();
            if (sampleRow != null)
            {
                sa = Mapper.Map<TSubjectArea>(dataModel.First());
                sa.Name = subjectArea;
            }

            foreach (var dCourse in dataModel)
            {
                var c = Mapper.Map<TCourse>(dCourse);

                var semester = Mapper.Map<TSemester>(dCourse);
                OnSemesterMapped(semester, dCourse);
                c.ActualSemester = semester;

                var grade = Mapper.Map<TGrade>(dCourse);
                grade.Value= (dCourse.FinalNumericGradeEarned != null) ? dCourse.FinalNumericGradeEarned.Value.ToString() : dCourse.FinalLetterGradeEarned;
                OnGradeMapped(grade, dCourse);
                c.FinalGrade = grade;

                OnCourseMapped(c, dCourse);
                sa.Courses.Add(c);
            }
            sa.TotalCreditsEarned = sa.Courses.Sum(x => x.CreditsEarned);

            OnSubjectAreaMapped(sa, dataModel);
            return sa;
        }

        protected virtual void InitializeMappings()
        {
            AutoMapperHelper.EnsureMapping<StudentRecordCourseHistory, SubjectArea, TSubjectArea>
                (Repository,
                 ignore => ignore.Name,
                 ignore => ignore.TotalCreditsEarned,
                 ignore => ignore.Courses
                );

            AutoMapperHelper.EnsureMapping<StudentRecordCourseHistory, Course, TCourse>
                (Repository,
                 ignore => ignore.ActualSemester,
                 ignore => ignore.FinalGrade);
                      
           AutoMapperHelper.EnsureMapping<StudentRecordCourseHistory, Semester, TSemester>(Repository);

           AutoMapperHelper.EnsureMapping<StudentRecordCourseHistory, Grade, TGrade>(Repository, ignore => ignore.Value);
        }

        protected virtual void OnModelMapped(TResponse model, IEnumerable<StudentRecordCourseHistory> data)
        {
        }

        protected virtual void OnSubjectAreaMapped(TSubjectArea subjectArea, IEnumerable<StudentRecordCourseHistory> data)
        {
        }

        protected virtual void OnCourseMapped(TCourse course, StudentRecordCourseHistory data)
        {
        }

        protected virtual void OnSemesterMapped(TSemester semester, StudentRecordCourseHistory data)
        {
        }

        protected virtual void OnGradeMapped(TGrade grade, StudentRecordCourseHistory data)
        {
        }
    }


    public interface ICourseHistoryListService : IService<CourseHistoryListRequest, CourseHistoryModel> { }

    //Concrete implementation.
    public sealed class CourseHistoryListListService : CourseHistoryListServiceBase<CourseHistoryListRequest, CourseHistoryModel, SubjectArea, Course, Semester, Grade>, ICourseHistoryListService { }

    public class SubjectAreaComparer : IComparer<string>
    {
        private const string allOtherSubjects = "All Other Subjects";

        public int Compare(string x, string y)
        {
            int result = String.Compare(x, y);

            // put All Other Subjects last
            if (x == allOtherSubjects && result != 0)
                result = 1;

            if (y == allOtherSubjects && result != 0)
                result = -1;

            return result;
        }
    }
}
