// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EdFi.Dashboards.Resources.Models.Student;
using NUnit.Framework;
using EdFi.Dashboards.Testing;

namespace EdFi.Dashboards.Resources.Models.Tests
{
    public class All_models_with_student_data_should_implement_student_interface : TestFixtureBase
    {
        private List<Type> modelTypesWithStudentInName = new List<Type>();
        private readonly List<MethodInfo> methodInfoWithStudentInNames = new List<MethodInfo>();

        protected override void ExecuteTest()
        {
            Type iStudent = typeof(IStudent);
            // get all types in EdFi.Dashboards.Resources.Models
            Assembly modelAssembly = Assembly.GetAssembly(iStudent);
            Type[] modelTypes = modelAssembly.GetTypes();
            
            // look for class names with Student
            modelTypesWithStudentInName = modelTypes.Where(x => !String.IsNullOrEmpty(x.FullName) && 
                                                                    x.FullName.ToLower().Contains("student") && 
                                                                    x != iStudent &&
                                                                    !x.IsEnum).ToList();

            // look for member names with Student
            foreach (Type modelType in modelTypes)
            {
                if (!modelType.GetInterfaces().Contains(iStudent) && modelType != iStudent)
                    methodInfoWithStudentInNames.AddRange(modelType.GetMethods().Where(x => !String.IsNullOrEmpty(x.Name) &&
                                                                                        x.Name.ToLower().Contains("student")));
            }
        }

        [Test]
        public void All_types_should_implement_student_interface()
        {
            var ignoreTypes = new List<string>
                                        {
                                            "EdFi.Dashboards.Resources.Models.School.Detail.StudentMetricListModel",
                                            "EdFi.Dashboards.Resources.Models.School.StudentsByGradeModel",
                                            "EdFi.Dashboards.Resources.Models.School.StudentsByGradeModel+Grade",
                                            "EdFi.Dashboards.Resources.Models.Student.Detail.CourseHistoryModel+Semester",
                                            "EdFi.Dashboards.Resources.Models.Student.StudentCopyInstanceProvider",
                                            "EdFi.Dashboards.Resources.Models.Common.StudentExportAllModel",
                                            "EdFi.Dashboards.Resources.Models.Common.StudentListIdentity",
                                            "EdFi.Dashboards.Resources.Models.Search.SearchModel+StudentSearchItem+GradeLevelItem",
                                            "EdFi.Dashboards.Resources.Models.School.StudentDemographicListModel",
                                            "EdFi.Dashboards.Resources.Models.School.StudentDemographicMenuModel",
                                            "EdFi.Dashboards.Resources.Models.LocalEducationAgency.StudentDemographicListModel",
                                            "EdFi.Dashboards.Resources.Models.LocalEducationAgency.StudentDemographicMenuModel",
                                            "EdFi.Dashboards.Resources.Models.LocalEducationAgency.StudentListModel",
                                            "EdFi.Dashboards.Resources.Models.LocalEducationAgency.StudentListMenuModel",
                                            "EdFi.Dashboards.Resources.Models.LocalEducationAgency.StudentDemographicListMetaModel",
                                            "EdFi.Dashboards.Resources.Models.LocalEducationAgency.StudentSchoolCategoryListModel",
                                            "EdFi.Dashboards.Resources.Models.LocalEducationAgency.StudentSchoolCategoryListMetaModel",
                                            "EdFi.Dashboards.Resources.Models.LocalEducationAgency.StudentSchoolCategoryMenuModel",
                                            "EdFi.Dashboards.Resources.Models.School.Detail.StudentMetricListMetaModel",
                                            "EdFi.Dashboards.Resources.Models.School.StudentDemographicListMetaModel",
                                            "EdFi.Dashboards.Resources.Models.School.StudentGradeListModel",
                                            "EdFi.Dashboards.Resources.Models.School.StudentGradeListMetaModel",
                                            "EdFi.Dashboards.Resources.Models.School.StudentGradeMenuModel",
                                            "EdFi.Dashboards.Resources.Models.Student.Accommodations",
											"EdFi.Dashboards.Resources.Models.Student.Detail.StudentMetricLearningStandardMetaDataModel",
											"EdFi.Dashboards.Resources.Models.Student.Detail.StudentMetricLearningStandardMetaDataModel+GradeModel",
											"EdFi.Dashboards.Resources.Models.Student.Detail.StudentMetricLearningStandardMetaDataModel+StandardModel",
											"EdFi.Dashboards.Resources.Models.Student.Detail.StudentObjectiveGradeStandardModel",
											"EdFi.Dashboards.Resources.Models.Student.Detail.StudentObjectiveGradeStandardModel+AssessmentModel",
											"EdFi.Dashboards.Resources.Models.Student.Detail.StudentObjectiveGradeStandardModel+BenchmarkAssessmentModel",
											"EdFi.Dashboards.Resources.Models.Student.Detail.StudentObjectiveGradeStandardModel+BenchmarkModel",
											"EdFi.Dashboards.Resources.Models.Student.Detail.StudentObjectiveGradeStandardModel+GradeModel",
											"EdFi.Dashboards.Resources.Models.Student.Detail.StudentObjectiveGradeStandardModel+StandardModel",
                                        };

            var problemTypes = new List<string>();

            Type iStudent = typeof (IStudent);
            foreach (Type t in modelTypesWithStudentInName)
            {
                if (!ignoreTypes.Contains(t.FullName) && !t.GetInterfaces().Contains(iStudent))
                    problemTypes.Add(t.FullName);
            }

            problemTypes.Sort();

            var sb = new StringBuilder();
            foreach(string s in problemTypes)
                sb.Append(s + Environment.NewLine);
            Assert.That(problemTypes.Count, Is.EqualTo(0), sb.ToString());
        }

        [Test]
        public void All_members_should_return_student_interface()
        {
            var ignoreMembers = new List<string>
                                    {                                        
                                        "get_StudentDemographics, EdFi.Dashboards.Resources.Models.School.Information.InformationModel",
                                        "get_StudentsByProgram, EdFi.Dashboards.Resources.Models.School.Information.InformationModel",
                                        "get_TotalNumberOfStudents, EdFi.Dashboards.Resources.Models.School.Information.GradePopulationItem",
                                        "get_TotalNumberOfStudentsByGrade, EdFi.Dashboards.Resources.Models.School.Information.GradePopulationItem",
                                        "get_AbsoluteStudentsCount, EdFi.Dashboards.Resources.Models.Search.SearchModel",
                                        "get_StudentTeacherRatios, EdFi.Dashboards.Resources.Models.LocalEducationAgency.Information.InformationModel",
                                        "get_LateEnrollmentStudents, EdFi.Dashboards.Resources.Models.LocalEducationAgency.Information.InformationModel",
                                        "get_StudentDemographics, EdFi.Dashboards.Resources.Models.LocalEducationAgency.Information.InformationModel",
                                        "get_StudentIndicatorPopulation, EdFi.Dashboards.Resources.Models.LocalEducationAgency.Information.InformationModel",
                                        "get_StudentsByProgram, EdFi.Dashboards.Resources.Models.LocalEducationAgency.Information.InformationModel",
                                        "get_StudentTeacherRatios, EdFi.Dashboards.Resources.Models.LocalEducationAgency.Information.InformationModel",
                                        "get_AbsoluteStudentsCount, EdFi.Dashboards.Resources.Models.Search.TranscriptSearchModel",
                                        "get_StudentListType, EdFi.Dashboards.Resources.Models.Common.StudentListIdentity",
										"get_AddedStudentIds, EdFi.Dashboards.Resources.Models.Common.MetricsBasedWatchListNotificationStudentModel", 
										"get_DroppedStudentIds, EdFi.Dashboards.Resources.Models.Common.MetricsBasedWatchListNotificationStudentModel", 
										"get_MetricBasedWatchListNotificationStudentId, EdFi.Dashboards.Resources.Models.Notification.MetricBasedWatchListNotificationStudentModel", 
                                        "get_StudentListType, EdFi.Dashboards.Resources.Models.Common.StudentListIdentity",
                                    };

            var problemTypes = new List<string>();

            Type iStudent = typeof(IStudent);
            Type voidType = typeof(void);
            foreach (MethodInfo mi in methodInfoWithStudentInNames)
            {
                Type returnType = mi.ReturnType;
                var methodNameAndClassFullName = mi.Name + ", " + mi.DeclaringType.FullName;
                if (returnType == voidType || ignoreMembers.Contains(methodNameAndClassFullName))
                    continue;

                if (returnType.IsGenericType)
                {
                    Type[] genericArguments = returnType.GetGenericArguments();
                    Assert.That(genericArguments.Length, Is.EqualTo(1), mi.Name + ", " + mi.DeclaringType.FullName);
                    if (!genericArguments[0].GetInterfaces().Contains(iStudent))
                        problemTypes.Add(mi.Name + ", " + mi.DeclaringType.FullName + " - " + returnType.FullName);
                }
                else if (!returnType.GetInterfaces().Contains(iStudent))
                    problemTypes.Add(mi.Name + ", " + mi.DeclaringType.FullName + " - " + returnType.FullName);
            }

            problemTypes.Sort();

            var sb = new StringBuilder();
            foreach (string s in problemTypes)
                sb.Append(s + Environment.NewLine);
            Assert.That(problemTypes.Count, Is.EqualTo(0), sb.ToString());
            
        }
    }
}
