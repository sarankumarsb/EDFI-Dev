using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Models.Student;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.School
{
    public abstract class ExportStudentDemographicListServiceFixtureBase : TestFixtureBase
    {
        protected IRepository<StudentInformation> studentInformationRepository;
        protected IRepository<StudentSchoolInformation> studentSchoolInformationRepository;
        protected IRepository<StudentIndicator> studentIndicatorRepository;
        protected IRepository<MetricInstance> metricInstanceRepository;
        protected IRepository<StudentSchoolMetricInstanceSet> studentSchoolMetricInstanceSetRepository;
        protected IRootMetricNodeResolver rootMetricNodeResolver;
        protected IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;

        protected int suppliedLocalEducationAgencyId = 1000;
        protected int suppliedSchoolId1 = 2001;
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

        protected string suppliedUniqueListId = "suppliedUniqueListId";
        protected int suppliedMetadataListId = 6;
        protected List<MetadataColumnGroup> suppliedMetadataColumnGroupList = new List<MetadataColumnGroup>{new MetadataColumnGroup{ Title = "Supplied Metadata"}};
        protected List<StudentWithMetrics.Metric> suppliedAdditionalMetrics = new List<StudentWithMetrics.Metric>{ new StudentWithMetrics.Metric(123){DisplayValue = "Supplied Additional Metric"}};
        protected string suppliedDemographic;
        protected bool expectStudentInformationQuery;
        protected int expectedStudentUSI;

        protected StudentExportAllModel actualModel;

        protected override void EstablishContext()
        {
            studentInformationRepository = mocks.StrictMock<IRepository<StudentInformation>>();
            studentSchoolInformationRepository = mocks.StrictMock<IRepository<StudentSchoolInformation>>();
            studentIndicatorRepository = mocks.StrictMock<IRepository<StudentIndicator>>();
            metricInstanceRepository = mocks.StrictMock<IRepository<MetricInstance>>();
            studentSchoolMetricInstanceSetRepository = mocks.StrictMock<IRepository<StudentSchoolMetricInstanceSet>>();
            rootMetricNodeResolver = mocks.StrictMock<IRootMetricNodeResolver>();
            gradeLevelUtilitiesProvider = mocks.StrictMock<IGradeLevelUtilitiesProvider>();

            Expect.Call(studentInformationRepository.GetAll()).Repeat.Any().Return(GetStudentInformation());
            Expect.Call(studentSchoolInformationRepository.GetAll()).Repeat.Any().Return(GetStudentSchoolInformation());
            if (expectStudentInformationQuery)
                Expect.Call(studentIndicatorRepository.GetAll()).Repeat.Any().Return(GetStudentIndicator());
            Expect.Call(metricInstanceRepository.GetAll()).Return(GetMetricInstance());
            Expect.Call(studentSchoolMetricInstanceSetRepository.GetAll()).Return(GetStudentSchoolMetricInstanceSet());

            Expect.Call(rootMetricNodeResolver.GetRootMetricNodeForStudent(suppliedSchoolId1)).Return(GetSuppliedMetadataHierarchy().Children.First());

            base.EstablishContext();
        }

        protected IQueryable<StudentInformation> GetStudentInformation()
        {
            var list = new List<StudentInformation>
                           {
                               new StudentInformation{ StudentUSI = 1, FullName="Gender - Female", Gender = "Female", HispanicLatinoEthnicity = "NO", FirstName = "first1", LastSurname = "Last1", MiddleName = "Middle1"},
                               new StudentInformation{ StudentUSI = 2, FullName="Gender - Male", Gender = "Male", FirstName = "first2", LastSurname = "Last2", MiddleName = "Middle2"},
                               new StudentInformation{ StudentUSI = 3, FullName="Ethnicity - Hispanic/Latino", HispanicLatinoEthnicity = "YES", FirstName = "first3", LastSurname = "Last3", MiddleName = "Middle3"},
                               new StudentInformation{ StudentUSI = 4, FullName="Race - Apple Banana", Race = "Apple Banana", FirstName = "first4", LastSurname = "Last4", MiddleName = "Middle4"},
                               new StudentInformation{ StudentUSI = 5, FullName="Race - Two or More", Race = "Grape,Orange", FirstName = "first5", LastSurname = "Last5", MiddleName = "Middle5"},
                               new StudentInformation{ StudentUSI = 6, FullName="Late Enrollment", FirstName = "first6", LastSurname = "Last6", MiddleName = "Middle6"},
                               new StudentInformation{ StudentUSI = 7, FullName="Indicator - Gifted/Talented", FirstName = "first7", LastSurname = "Last7", MiddleName = "Middle7"},
                               new StudentInformation{ StudentUSI = 1 + 9990, FullName="Wrong LEA Gender - Female", Gender = "Female"},
                               new StudentInformation{ StudentUSI = 2 + 9990, FullName="Wrong LEA Gender - Male", Gender = "Male"},
                               new StudentInformation{ StudentUSI = 3 + 9990, FullName="Wrong LEA Ethnicity - Hispanic/Latino", HispanicLatinoEthnicity = "YES"},
                               new StudentInformation{ StudentUSI = 4 + 9990, FullName="Wrong LEA Race - Apple Banana", Race = "Apple Banana"},
                               new StudentInformation{ StudentUSI = 5 + 9990, FullName="Wrong LEA Race - Two or More", Race = "Grape,Orange"},
                               new StudentInformation{ StudentUSI = 6 + 9990, FullName="Wrong LEA Late Enrollment"},
                               new StudentInformation{ StudentUSI = 7 + 9990, FullName="Wrong LEA Indicator - Gifted/Talented"}
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
                               new StudentSchoolInformation{ StudentUSI = 7 + 9990, SchoolId = 999},
                           };
            return list.AsQueryable();
        }

        protected IQueryable<StudentIndicator> GetStudentIndicator()
        {
            var list = new List<StudentIndicator>
                           {
                               new StudentIndicator{ StudentUSI = 1, EducationOrganizationId = suppliedLocalEducationAgencyId, Name = "Gifted/Talented", Status = false},
                               new StudentIndicator{ StudentUSI = 7, EducationOrganizationId = suppliedLocalEducationAgencyId, Name = "Gifted/Talented", Status = true},
                               new StudentIndicator{ StudentUSI = 7 + 9990, EducationOrganizationId = suppliedLocalEducationAgencyId, Name = "Gifted/Talented", Status = true},
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
                               new MetricInstance{ MetricInstanceSetKey = suppliedStudent9997, MetricId = 3, Value = ".999"},
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
                               new StudentSchoolMetricInstanceSet{SchoolId = 999, StudentUSI = 7 + 9990, MetricInstanceSetKey = suppliedStudent9997},
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
        
        protected override void ExecuteTest()
        {
            var service = new ExportStudentDemographicListService(studentInformationRepository, studentSchoolInformationRepository, studentIndicatorRepository, studentSchoolMetricInstanceSetRepository, metricInstanceRepository, rootMetricNodeResolver, gradeLevelUtilitiesProvider);
            actualModel = service.Get(ExportStudentDemographicListRequest.Create(suppliedSchoolId1, suppliedDemographic));
        }

        [Test]
        public virtual void Should_correctly_select_students_matching_demographic()
        {
            var student = GetStudentInformation().Single(x => x.StudentUSI == expectedStudentUSI);
            var metric = GetMetricInstance().Single(x => x.MetricInstanceSetKey == GetStudentSchoolMetricInstanceSet().Single(y => y.StudentUSI == expectedStudentUSI).MetricInstanceSetKey);
            Assert.That(actualModel.Rows.Count(), Is.EqualTo(1));
            Assert.That(actualModel.Rows.ElementAt(0).StudentUSI, Is.EqualTo(expectedStudentUSI));
            Assert.That(actualModel.Rows.ElementAt(0).Cells.ElementAt(0).Key, Is.EqualTo("Student Name"));
            Assert.That(actualModel.Rows.ElementAt(0).Cells.ElementAt(0).Value, Is.EqualTo(Utilities.FormatPersonNameByLastName(student.FirstName, student.MiddleName, student.LastSurname)));
            Assert.That(actualModel.Rows.ElementAt(0).Cells.ElementAt(1).Key, Is.EqualTo("Grade Level"));
            Assert.That(actualModel.Rows.ElementAt(0).Cells.ElementAt(2).Key, Is.EqualTo("Root - Container 2 - Granular 3"));
            Assert.That(actualModel.Rows.ElementAt(0).Cells.ElementAt(2).Value, Is.EqualTo(metric.Value));
        }
    }

    [TestFixture]
    public class When_exporting_students_by_female_demographic : ExportStudentDemographicListServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedDemographic = "Female";
            expectedStudentUSI = 1;
            expectStudentInformationQuery = false;
            base.EstablishContext();
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.AtLeastOnce().Return("");

        }
    }

    [TestFixture]
    public class When_exporting_students_by_male_demographic : ExportStudentDemographicListServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedDemographic = "Male";
            expectedStudentUSI = 2;
            expectStudentInformationQuery = false;
            base.EstablishContext();
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.AtLeastOnce().Return("");
        }
    }

    [TestFixture]
    public class When_exporting_students_by_hispanic_demographic : ExportStudentDemographicListServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedDemographic = "Hispanic-Latino";
            expectedStudentUSI = 3;
            expectStudentInformationQuery = false;
            base.EstablishContext();
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.AtLeastOnce().Return("");
        }
    }

    [TestFixture]
    public class When_exporting_students_by_race_demographic : ExportStudentDemographicListServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedDemographic = "Apple-Banana";
            expectedStudentUSI = 4;
            expectStudentInformationQuery = true;
            base.EstablishContext();
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.AtLeastOnce().Return("");
        }
    }

    [TestFixture]
    public class When_exporting_students_by_two_or_more_races_demographic : ExportStudentDemographicListServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedDemographic = "Two-or-More";
            expectedStudentUSI = 5;
            expectStudentInformationQuery = false;
            base.EstablishContext();
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.AtLeastOnce().Return("");
        }
    }

    [TestFixture]
    public class When_exporting_students_by_late_enrollment_demographic : ExportStudentDemographicListServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedDemographic = "Late-Enrollment";
            expectedStudentUSI = 6;
            expectStudentInformationQuery = false;
            base.EstablishContext();
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.AtLeastOnce().Return("");
        }
    }

    [TestFixture]
    public class When_exporting_students_by_indicator_demographic : ExportStudentDemographicListServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedDemographic = "Gifted-Talented";
            expectedStudentUSI = 7;
            expectStudentInformationQuery = true;
            base.EstablishContext();
            Expect.Call(gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay("1")).IgnoreArguments().Repeat.AtLeastOnce().Return("");
        }
    }
}
