using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.LocalEducationAgency
{
    public abstract class ExportStudentListServiceFixtureBase : TestFixtureBase
    {
        protected int suppliedStaffCohortId = 111;
        protected int suppliedStaffCustomStudentListId = 120;
        protected int suppliedWrongStaffCustomStudentListId = 2;
        protected int suppliedWrongStaffCohortId = 4;
        protected int suppliedWrongStaffCohortId2 = 5;
        protected int suppliedLocalEducationAgencyId = 1000;
        protected int suppliedWrongLocalEducationAgencyId = 1001;
        protected int suppliedSchoolId1 = 2001;
        protected int suppliedStaffUSI = 99;
        protected int suppliedWrongStaffUSI = 999;

        protected Guid suppliedStudent1 = Guid.NewGuid();
        protected Guid suppliedStudent2 = Guid.NewGuid();
        protected Guid suppliedStudent3 = Guid.NewGuid();
        protected Guid suppliedStudent4 = Guid.NewGuid();
        protected Guid suppliedStudent5 = Guid.NewGuid();
        protected Guid suppliedStudent6 = Guid.NewGuid();
        protected Guid suppliedStudent7 = Guid.NewGuid();
        protected Guid suppliedStudent9991 = Guid.NewGuid();
        protected Guid suppliedStudent9992 = Guid.NewGuid();
        protected Guid suppliedStudent9993 = Guid.NewGuid();
        protected Guid suppliedStudent9994 = Guid.NewGuid();
        protected Guid suppliedStudent9995 = Guid.NewGuid();
        protected Guid suppliedStudent9996 = Guid.NewGuid();
        protected Guid suppliedStudent9997 = Guid.NewGuid();

        protected StudentExportAllModel actualModel;
        protected ExportStudentListService service;

        protected IRepository<StaffCohort> staffCohortRepository;
        protected IRepository<StaffStudentCohort> staffStudentCohortRepository;
        protected IRepository<StaffCustomStudentList> staffCustomStudentListRepository;
        protected IRepository<StaffCustomStudentListStudent> staffCustomStudentListStudentRepository;
        protected IRepository<SchoolInformation> schoolInformationRepository;
        protected IRepository<StudentInformation> studentInformationRepository;
        protected IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        protected IRepository<StudentIndicator> studentIndicatorRepository;
        protected IRepository<MetricInstance> metricInstanceRepository;
        protected IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository;
        protected IRootMetricNodeResolver rootMetricNodeResolver;
        protected IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;
        protected int expectedRowCount;

        protected override void EstablishContext()
        {
            staffCohortRepository = mocks.StrictMock<IRepository<StaffCohort>>();
            staffStudentCohortRepository = mocks.StrictMock<IRepository<StaffStudentCohort>>();
            staffCustomStudentListRepository = mocks.StrictMock<IRepository<StaffCustomStudentList>>();
            staffCustomStudentListStudentRepository = mocks.StrictMock<IRepository<StaffCustomStudentListStudent>>();
            schoolInformationRepository = mocks.StrictMock<IRepository<SchoolInformation>>();
            studentInformationRepository = mocks.StrictMock<IRepository<StudentInformation>>();
            studentSchoolInformationRepository = mocks.StrictMock<IRepository<StudentSchoolInformation>>();
            studentIndicatorRepository = mocks.StrictMock<IRepository<StudentIndicator>>();
            metricInstanceRepository = mocks.StrictMock<IRepository<MetricInstance>>();
            studentSchoolMetricInstanceSetRepository = mocks.StrictMock<IRepository<StudentSchoolMetricInstanceSet>>();
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();
            gradeLevelUtilitiesProvider = new GradeLevelUtilitiesProvider();

            Expect.Call(staffCohortRepository.GetAll()).Repeat.Any().Return(GetStaffCohortRepositoryInformation());
            Expect.Call(staffStudentCohortRepository.GetAll()).Repeat.Any().Return(GetStaffStudentCohortRepositoryInformation());
            Expect.Call(staffCustomStudentListRepository.GetAll()).Repeat.Any().Return(GetStaffCustomStudentListRepositoryInformation());
            Expect.Call(staffCustomStudentListStudentRepository.GetAll()).Repeat.Any().Return(GetStaffCustomStudentListStudentRepositoryInformation());
            Expect.Call(schoolInformationRepository.GetAll()).Repeat.Any().Return(GetSchoolInformation());
            Expect.Call(studentInformationRepository.GetAll()).Repeat.Any().Return(GetStudentInformation());
            Expect.Call(studentSchoolInformationRepository.GetAll()).Repeat.Any().Return(GetStudentSchoolInformation());
            Expect.Call(metricInstanceRepository.GetAll()).Return(GetMetricInstance());
            Expect.Call(studentSchoolMetricInstanceSetRepository.GetAll()).Return(GetStudentSchoolMetricInstanceSet());

            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForStudent(suppliedSchoolId1)).Repeat.Any().Return(GetSuppliedMetadataHierarchy().Children.First());

            base.EstablishContext();
        }

        protected IQueryable<StaffCohort> GetStaffCohortRepositoryInformation()
        {
            return (new List<StaffCohort>
            {
                new StaffCohort
                    {
                        CohortDescription = "Good Cohort that should be return in all or alone if selected",
                        CohortIdentifier = "Good", 
                        EducationOrganizationId = suppliedLocalEducationAgencyId,
                        StaffCohortId = suppliedStaffCohortId, 
                        StaffUSI = suppliedStaffUSI
                    },
                    new StaffCohort
                    {
                        CohortDescription = "Bad cohort, different LEA, different StaffCohortId and different STAFFUSI",
                        CohortIdentifier = "Bad By Staff",
                        EducationOrganizationId = suppliedWrongLocalEducationAgencyId,
                        StaffCohortId = suppliedWrongStaffCohortId,
                        StaffUSI = suppliedWrongStaffUSI
                    },
                    new StaffCohort
                    {
                        CohortDescription = "Bad cohort, different LEA, same staff id, should not be returned for all",
                        CohortIdentifier = "Bad By LEA",
                        EducationOrganizationId = suppliedWrongLocalEducationAgencyId,
                        StaffCohortId = suppliedWrongStaffCohortId2,
                        StaffUSI = suppliedStaffUSI
                    }
            }).AsQueryable();
        }

        protected IQueryable<StaffStudentCohort> GetStaffStudentCohortRepositoryInformation()
        {
            return (new List<StaffStudentCohort>
                    {
                        new StaffStudentCohort{ StudentUSI = 1, StaffCohortId = suppliedStaffCohortId },
                        new StaffStudentCohort{ StudentUSI = 2, StaffCohortId = suppliedStaffCohortId },
                        new StaffStudentCohort{ StudentUSI = 3, StaffCohortId = suppliedStaffCohortId },
                        new StaffStudentCohort{ StudentUSI = 4, StaffCohortId = suppliedStaffCohortId },
                        new StaffStudentCohort{ StudentUSI = 5, StaffCohortId = suppliedStaffCohortId },
                        new StaffStudentCohort{ StudentUSI = 6, StaffCohortId = suppliedStaffCohortId },
                        new StaffStudentCohort{ StudentUSI = 7, StaffCohortId = suppliedStaffCohortId },
                        new StaffStudentCohort{ StudentUSI = 9991, StaffCohortId = suppliedWrongStaffCohortId },
                        new StaffStudentCohort{ StudentUSI = 9992, StaffCohortId = suppliedWrongStaffCohortId },
                        new StaffStudentCohort{ StudentUSI = 9993, StaffCohortId = suppliedWrongStaffCohortId },
                        new StaffStudentCohort{ StudentUSI = 9995, StaffCohortId = suppliedWrongStaffCohortId },
                        new StaffStudentCohort{ StudentUSI = 9996, StaffCohortId = suppliedWrongStaffCohortId },
                        new StaffStudentCohort{ StudentUSI = 9997, StaffCohortId = suppliedWrongStaffCohortId },
                    }).AsQueryable();
        }

        protected IQueryable<StaffCustomStudentList> GetStaffCustomStudentListRepositoryInformation()
        {
            return (new List<StaffCustomStudentList>
            {
                new StaffCustomStudentList
                    {
                        CustomStudentListIdentifier = "Custom List 1",
                        EducationOrganizationId = suppliedLocalEducationAgencyId,
                        StaffCustomStudentListId = suppliedStaffCustomStudentListId,
                        StaffUSI = suppliedStaffUSI,
                    },
                new StaffCustomStudentList
                    {
                        CustomStudentListIdentifier = "Custom List 2",
                        EducationOrganizationId = suppliedWrongLocalEducationAgencyId,
                        StaffCustomStudentListId = suppliedWrongStaffCustomStudentListId,
                        StaffUSI = suppliedStaffUSI,
                    }
            }).AsQueryable();
        }

        protected IQueryable<StaffCustomStudentListStudent> GetStaffCustomStudentListStudentRepositoryInformation()
        {
            return (new List<StaffCustomStudentListStudent>
            {
                new StaffCustomStudentListStudent{ StaffCustomStudentListStudentId = 1, StaffCustomStudentListId = suppliedStaffCustomStudentListId, StudentUSI = 1 },
                new StaffCustomStudentListStudent{ StaffCustomStudentListStudentId = 2, StaffCustomStudentListId = suppliedStaffCustomStudentListId, StudentUSI = 2 },
                new StaffCustomStudentListStudent{ StaffCustomStudentListStudentId = 3, StaffCustomStudentListId = suppliedStaffCustomStudentListId, StudentUSI = 3 },
                new StaffCustomStudentListStudent{ StaffCustomStudentListStudentId = 4, StaffCustomStudentListId = suppliedStaffCustomStudentListId, StudentUSI = 4 },
                new StaffCustomStudentListStudent{ StaffCustomStudentListStudentId = 5, StaffCustomStudentListId = suppliedStaffCustomStudentListId, StudentUSI = 5 },
                new StaffCustomStudentListStudent{ StaffCustomStudentListStudentId = 6, StaffCustomStudentListId = suppliedStaffCustomStudentListId, StudentUSI = 6 },
                new StaffCustomStudentListStudent{ StaffCustomStudentListStudentId = 9, StaffCustomStudentListId = suppliedWrongStaffCustomStudentListId, StudentUSI = 9991 },
                new StaffCustomStudentListStudent{ StaffCustomStudentListStudentId = 10, StaffCustomStudentListId = suppliedWrongStaffCustomStudentListId, StudentUSI = 9992 },
                new StaffCustomStudentListStudent{ StaffCustomStudentListStudentId = 11, StaffCustomStudentListId = suppliedWrongStaffCustomStudentListId, StudentUSI = 9993 }
            }).AsQueryable();
        }

        protected IQueryable<SchoolInformation> GetSchoolInformation()
        {
            var list = new List<SchoolInformation>
                {
                    new SchoolInformation { SchoolId = suppliedSchoolId1, LocalEducationAgencyId = suppliedLocalEducationAgencyId },
                    new SchoolInformation { SchoolId = 999, LocalEducationAgencyId = 999 }
                };

            return list.AsQueryable();
        }

        protected IQueryable<StudentInformation> GetStudentInformation()
        {
            var list = new List<StudentInformation>
                           {
                               new StudentInformation{ StudentUSI = 1, FullName="Student 1", Gender = "Female", HispanicLatinoEthnicity = "NO", FirstName = "first1", LastSurname = "Last1", MiddleName = "Middle1"},
                               new StudentInformation{ StudentUSI = 2, FullName="Student 2", Gender = "Male", FirstName = "first2", LastSurname = "Last2", MiddleName = "Middle2"},
                               new StudentInformation{ StudentUSI = 3, FullName="Student 3", HispanicLatinoEthnicity = "YES", FirstName = "first3", LastSurname = "Last3", MiddleName = "Middle3"},
                               new StudentInformation{ StudentUSI = 4, FullName="Student 4", Race = "Apple Banana", FirstName = "first4", LastSurname = "Last4", MiddleName = "Middle4"},
                               new StudentInformation{ StudentUSI = 5, FullName="Student 5", Race = "Grape,Orange", FirstName = "first5", LastSurname = "Last5", MiddleName = "Middle5"},
                               new StudentInformation{ StudentUSI = 6, FullName="Student 6", FirstName = "first6", LastSurname = "Last6", MiddleName = "Middle6"},
                               new StudentInformation{ StudentUSI = 7, FullName="Student 7", FirstName = "first7", LastSurname = "Last7", MiddleName = "Middle7"},
                               new StudentInformation{ StudentUSI = 1 + 9990, FullName="Student 9991"},
                               new StudentInformation{ StudentUSI = 2 + 9990, FullName="Student 9992"},
                               new StudentInformation{ StudentUSI = 3 + 9990, FullName="Student 9993"},
                               new StudentInformation{ StudentUSI = 4 + 9990, FullName="Student 9994"},
                               new StudentInformation{ StudentUSI = 5 + 9990, FullName="Student 9995"},
                               new StudentInformation{ StudentUSI = 6 + 9990, FullName="Student 9996"},
                               new StudentInformation{ StudentUSI = 7 + 9990, FullName="Student 9997"}
                           };
            return list.AsQueryable();
        }

        protected IQueryable<StudentSchoolInformation> GetStudentSchoolInformation()
        {
            var list = new List<StudentSchoolInformation>
                           {
                               new StudentSchoolInformation{ StudentUSI = 1, SchoolId = suppliedSchoolId1, LateEnrollment = "NO"},
                               new StudentSchoolInformation{ StudentUSI = 2, SchoolId = suppliedSchoolId1},
                               new StudentSchoolInformation{ StudentUSI = 3, SchoolId = suppliedSchoolId1},
                               new StudentSchoolInformation{ StudentUSI = 4, SchoolId = suppliedSchoolId1},
                               new StudentSchoolInformation{ StudentUSI = 5, SchoolId = suppliedSchoolId1},
                               new StudentSchoolInformation{ StudentUSI = 6, SchoolId = suppliedSchoolId1, LateEnrollment = "YES"},
                               new StudentSchoolInformation{ StudentUSI = 7, SchoolId = suppliedSchoolId1},
                               new StudentSchoolInformation{ StudentUSI = 1 + 9990, SchoolId = 999},
                               new StudentSchoolInformation{ StudentUSI = 2 + 9990, SchoolId = 999},
                               new StudentSchoolInformation{ StudentUSI = 3 + 9990, SchoolId = 999},
                               new StudentSchoolInformation{ StudentUSI = 4 + 9990, SchoolId = 999},
                               new StudentSchoolInformation{ StudentUSI = 5 + 9990, SchoolId = 999},
                               new StudentSchoolInformation{ StudentUSI = 6 + 9990, SchoolId = 999, LateEnrollment = "YES"},
                               new StudentSchoolInformation{ StudentUSI = 7 + 9990, SchoolId = 999}
                           };
            return list.AsQueryable();
        }

        protected IQueryable<MetricInstance> GetMetricInstance()
        {
            var list = new List<MetricInstance>
                           {
                               new MetricInstance{ MetricInstanceSetKey = suppliedStudent1, MetricId = 3, Value = ".4"},
                               new MetricInstance{ MetricInstanceSetKey = suppliedStudent2, MetricId = 3, Value = ".5"},
                               new MetricInstance{ MetricInstanceSetKey = suppliedStudent3, MetricId = 3, Value = ".6"},
                               new MetricInstance{ MetricInstanceSetKey = suppliedStudent4, MetricId = 3, Value = ".7"},
                               new MetricInstance{ MetricInstanceSetKey = suppliedStudent5, MetricId = 3, Value = ".8"},
                               new MetricInstance{ MetricInstanceSetKey = suppliedStudent6, MetricId = 3, Value = ".9"},
                               new MetricInstance{ MetricInstanceSetKey = suppliedStudent7, MetricId = 3, Value = ".10"},
                               new MetricInstance{ MetricInstanceSetKey = suppliedStudent9991, MetricId = 3, Value = ".999"},
                               new MetricInstance{ MetricInstanceSetKey = suppliedStudent9992, MetricId = 3, Value = ".999"},
                               new MetricInstance{ MetricInstanceSetKey = suppliedStudent9993, MetricId = 3, Value = ".999"},
                               new MetricInstance{ MetricInstanceSetKey = suppliedStudent9994, MetricId = 3, Value = ".999"},
                               new MetricInstance{ MetricInstanceSetKey = suppliedStudent9995, MetricId = 3, Value = ".999"},
                               new MetricInstance{ MetricInstanceSetKey = suppliedStudent9996, MetricId = 3, Value = ".999"},
                               new MetricInstance{ MetricInstanceSetKey = suppliedStudent9997, MetricId = 3, Value = ".999"}
                           };
            return list.AsQueryable();
        }

        protected IQueryable<StudentSchoolMetricInstanceSet> GetStudentSchoolMetricInstanceSet()
        {
            var list = new List<StudentSchoolMetricInstanceSet>
                           {
                               new StudentSchoolMetricInstanceSet{SchoolId = suppliedSchoolId1, StudentUSI = 1, MetricInstanceSetKey = suppliedStudent1},
                               new StudentSchoolMetricInstanceSet{SchoolId = suppliedSchoolId1, StudentUSI = 2, MetricInstanceSetKey = suppliedStudent2},
                               new StudentSchoolMetricInstanceSet{SchoolId = suppliedSchoolId1, StudentUSI = 3, MetricInstanceSetKey = suppliedStudent3},
                               new StudentSchoolMetricInstanceSet{SchoolId = suppliedSchoolId1, StudentUSI = 4, MetricInstanceSetKey = suppliedStudent4},
                               new StudentSchoolMetricInstanceSet{SchoolId = suppliedSchoolId1, StudentUSI = 5, MetricInstanceSetKey = suppliedStudent5},
                               new StudentSchoolMetricInstanceSet{SchoolId = suppliedSchoolId1, StudentUSI = 6, MetricInstanceSetKey = suppliedStudent6},
                               new StudentSchoolMetricInstanceSet{SchoolId = suppliedSchoolId1, StudentUSI = 7, MetricInstanceSetKey = suppliedStudent7},
                               new StudentSchoolMetricInstanceSet{SchoolId = 999, StudentUSI = 1 + 9990, MetricInstanceSetKey = suppliedStudent9991},
                               new StudentSchoolMetricInstanceSet{SchoolId = 999, StudentUSI = 2 + 9990, MetricInstanceSetKey = suppliedStudent9992},
                               new StudentSchoolMetricInstanceSet{SchoolId = 999, StudentUSI = 3 + 9990, MetricInstanceSetKey = suppliedStudent9993},
                               new StudentSchoolMetricInstanceSet{SchoolId = 999, StudentUSI = 4 + 9990, MetricInstanceSetKey = suppliedStudent9994},
                               new StudentSchoolMetricInstanceSet{SchoolId = 999, StudentUSI = 5 + 9990, MetricInstanceSetKey = suppliedStudent9995},
                               new StudentSchoolMetricInstanceSet{SchoolId = 999, StudentUSI = 6 + 9990, MetricInstanceSetKey = suppliedStudent9996},
                               new StudentSchoolMetricInstanceSet{SchoolId = 999, StudentUSI = 7 + 9990, MetricInstanceSetKey = suppliedStudent9997}
                           };
            return list.AsQueryable();
        }

        protected MetricMetadataTree GetSuppliedMetadataHierarchy()
        {
            var tree = new TestMetricMetadataTree();

            tree.Children = new List<MetricMetadataNode>
            {
                new MetricMetadataNode(tree)
                {
                    MetricId = 1,
                    MetricNodeId = 10,
                    DisplayName = "Root",
                    MetricType = Dashboards.Metric.Resources.Models.MetricType.ContainerMetric,
                    Children = new List<MetricMetadataNode>
                                          {
                                              new MetricMetadataNode(tree)
                                                  {
                                                        MetricId = 2,
                                                        MetricNodeId = 20,
                                                        DisplayName = "Container 2",
                                                        MetricType = Dashboards.Metric.Resources.Models.MetricType.ContainerMetric,
                                                        Children = new List<MetricMetadataNode>
                                                                       {
                                                                            new MetricMetadataNode(tree)
                                                                            {
                                                                                MetricId = 3,
                                                                                MetricNodeId = 30,
                                                                                DisplayName = "Granular 3",
                                                                                MetricType = Dashboards.Metric.Resources.Models.MetricType.GranularMetric,
                                                                            }
                                                                       }

                                                  },

                    },
                }
            };

            foreach (var childMetric in tree.Children)
                setParents(childMetric, null);

            return tree;
        }

        //Method used to tag the parents to the hierarchy
        protected void setParents(MetricMetadataNode metric, MetricMetadataNode parentMetric)
        {
            metric.Parent = parentMetric;

            foreach (var childMetric in metric.Children)
                setParents(childMetric, metric);
        }

        [Test]
        public void Should_return_a_non_null_model()
        {
            Assert.That(actualModel, Is.Not.Null);
        }

        [Test]
        public void Should_return_a_model_with_a_non_null_rows_property()
        {
            Assert.That(actualModel.Rows, Is.Not.Null);
        }

        [Test]
        public void Should_return_a_model_with_data_in_the_rows_property()
        {
            Assert.That(actualModel.Rows.Count(), Is.GreaterThan(0));
        }

        [Test]
        public void Should_return_a_model_with_the_correct_number_of_rows()
        {
            Assert.That(actualModel.Rows.Count(), Is.EqualTo(expectedRowCount));
        }

        [Test]
        public void Should_return_a_model_with_correct_data()
        {
            //should have no metric values of .999 and no students with and id greater that 100
            foreach (var row in actualModel.Rows)
            {
                foreach (var keyValuePair in row.Cells)
                {
                    Assert.That(keyValuePair.Value, Is.Not.EqualTo(suppliedStudent9991));
                    Assert.That(keyValuePair.Value, Is.Not.EqualTo(suppliedStudent9992));
                    Assert.That(keyValuePair.Value, Is.Not.EqualTo(suppliedStudent9993));
                    Assert.That(keyValuePair.Value, Is.Not.EqualTo(suppliedStudent9994));
                    Assert.That(keyValuePair.Value, Is.Not.EqualTo(suppliedStudent9995));
                    Assert.That(keyValuePair.Value, Is.Not.EqualTo(suppliedStudent9996));
                    Assert.That(keyValuePair.Value, Is.Not.EqualTo(suppliedStudent9997));
                    Assert.That(keyValuePair.Value, Is.Not.EqualTo(.999));
                }
            }
        }
    }

    [TestFixture]
    public class When_calling_ExportStudentListService_get_to_export_a_cohort_list : ExportStudentListServiceFixtureBase
    {
        protected override void ExecuteTest()
        {
            service = new ExportStudentListService(staffCohortRepository, staffStudentCohortRepository, staffCustomStudentListRepository,
                                                        staffCustomStudentListStudentRepository, studentInformationRepository, studentSchoolInformationRepository,
                                                            studentSchoolMetricInstanceSetRepository, metricInstanceRepository, rootMetricNodeResolver, schoolInformationRepository,
                                                            gradeLevelUtilitiesProvider);

            var request = ExportStudentListRequest.Create(suppliedLocalEducationAgencyId, suppliedStaffUSI, suppliedStaffCohortId, "Cohort");
            actualModel = service.Get(request);

            expectedRowCount = 7;
        }
    }

    [TestFixture]
    public class When_calling_ExportStudentListService_get_to_export_a_custom_student_list : ExportStudentListServiceFixtureBase
    {

        protected override void ExecuteTest()
        {
            service = new ExportStudentListService(staffCohortRepository, staffStudentCohortRepository, staffCustomStudentListRepository,
                                                        staffCustomStudentListStudentRepository, studentInformationRepository, studentSchoolInformationRepository,
                                                            studentSchoolMetricInstanceSetRepository, metricInstanceRepository, rootMetricNodeResolver, schoolInformationRepository,
                                                            gradeLevelUtilitiesProvider);

            var request = ExportStudentListRequest.Create(suppliedLocalEducationAgencyId, suppliedStaffUSI, suppliedStaffCustomStudentListId, "CustomStudentList");
            actualModel = service.Get(request);
            expectedRowCount = 6;
        }
    }

    [TestFixture]
    public class When_calling_ExportStudentListService_get_to_export_all_custom_and_cohort_student_lists : ExportStudentListServiceFixtureBase
    {

        protected override void ExecuteTest()
        {
            service = new ExportStudentListService(staffCohortRepository, staffStudentCohortRepository, staffCustomStudentListRepository,
                                                        staffCustomStudentListStudentRepository, studentInformationRepository, studentSchoolInformationRepository,
                                                            studentSchoolMetricInstanceSetRepository, metricInstanceRepository, rootMetricNodeResolver, schoolInformationRepository,
                                                            gradeLevelUtilitiesProvider);

            var request = ExportStudentListRequest.Create(suppliedLocalEducationAgencyId, suppliedStaffUSI, 0, "All");
            actualModel = service.Get(request);
            //should start with a list of 13 students but should merge to distinct students
            expectedRowCount = 7;
        }
    }

    [TestFixture]
    public class When_calling_ExportStudentListService_get_to_export_the_list_that_will_be_shown : ExportStudentListServiceFixtureBase
    {
        //the application pulls the first cohort, or custom student list to be displayed as the default when entering the page
        //this will test if the correct one is returned
        protected override void ExecuteTest()
        {
            service = new ExportStudentListService(staffCohortRepository, staffStudentCohortRepository, staffCustomStudentListRepository,
                                                        staffCustomStudentListStudentRepository, studentInformationRepository, studentSchoolInformationRepository,
                                                            studentSchoolMetricInstanceSetRepository, metricInstanceRepository, rootMetricNodeResolver, schoolInformationRepository,
                                                            gradeLevelUtilitiesProvider);

            var request = ExportStudentListRequest.Create(suppliedLocalEducationAgencyId, suppliedStaffUSI, 0, "None");
            actualModel = service.Get(request);
            expectedRowCount = 7;
        }
    }
}
