using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Staff;
using NUnit.Framework;
using Rhino.Mocks;
using System.Collections.Generic;
using System.Linq;

namespace EdFi.Dashboards.Resources.Tests.Staff
{
	public class StaffServiceBaseFixture
	{
		[Test]
		public void StaffServiceBase_GetStudentListIdentity_InvalidStudentListType()
		{
			//arrage
			long staffUSI = 1;
			int schoolId = 2;
			int teacherSectionId = 3;

			IRepository<TeacherSection> teacherSectionRepository = MockRepository.GenerateStrictMock<IRepository<TeacherSection>>();
			teacherSectionRepository.Stub(repo => repo.GetAll()).IgnoreArguments().Return(GetTeacherSection(staffUSI, schoolId, teacherSectionId));

			StaffServiceBase objUnderTest = MockRepository.GeneratePartialMock<StaffServiceBase>();
			objUnderTest.TeacherSectionRepository = teacherSectionRepository;
			
			//act
			StudentListIdentity returnedIdentity = objUnderTest.GetStudentListIdentity(staffUSI, schoolId, "foo", teacherSectionId);

			//assert
			Assert.IsNotNull(returnedIdentity);
			Assert.AreEqual(teacherSectionId, returnedIdentity.Id, "returned Id does not match teacherSectionId");
			Assert.AreEqual(StudentListType.Section, returnedIdentity.StudentListType, "returned StudentListType does not expected value of Section");
		}

		[Test]
		public void StaffServiceBase_GetStudentListIdentity_ValidStudentListType_Cohort()
		{
			//arrage
			long staffUSI = 1;
			int schoolId = 2;
			int teacherSectionId = 3;

			IRepository<TeacherSection> teacherSectionRepository = MockRepository.GenerateStrictMock<IRepository<TeacherSection>>();
			teacherSectionRepository.Stub(repo => repo.GetAll()).IgnoreArguments().Return(GetTeacherSection(staffUSI, schoolId, teacherSectionId));

			StaffServiceBase objUnderTest = MockRepository.GeneratePartialMock<StaffServiceBase>();
			objUnderTest.TeacherSectionRepository = teacherSectionRepository;

			//act
			StudentListIdentity returnedIdentity = objUnderTest.GetStudentListIdentity(staffUSI, schoolId, StudentListType.Cohort.ToString(), teacherSectionId);

			//assert
			Assert.IsNotNull(returnedIdentity);
			Assert.AreEqual(teacherSectionId, returnedIdentity.Id, "returned Id does not match teacherSectionId");
			Assert.AreEqual(StudentListType.Cohort, returnedIdentity.StudentListType, "returned StudentListType does not expected value of Section");
		}

		protected IQueryable<TeacherSection> GetTeacherSection(long staffUSI, int schoolId, int teacherSectionId)
		{
			var data = new List<TeacherSection>
                           {
                                new TeacherSection 
								{
									StaffUSI = staffUSI, 
									SchoolId = schoolId, 
									TeacherSectionId=teacherSectionId, 
									SubjectArea="subject area", 
									ClassPeriod="class period", 
									CourseTitle="course title", 
									GradeLevel="grade level", 
									LocalCourseCode="local course code", 
									TermType="term type"},
                           };
			return data.AsQueryable();
		}

		
	}
}
