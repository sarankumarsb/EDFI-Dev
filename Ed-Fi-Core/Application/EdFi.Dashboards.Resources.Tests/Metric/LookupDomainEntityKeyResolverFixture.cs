// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.MetricInstanceSetKeyResolvers;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Metric
{
    [TestFixture]
    public abstract class When_invoking_the_Lookup_Metric_Instance_Set_Key_Resolver : TestFixtureBase
    {
        //The Service to test.
        protected LocalEducationAgencyLookupMetricInstanceSetKeyResolver localEducationAgencyService;
        protected SchoolLookupMetricInstanceSetKeyResolver schoolService;
        protected StudentSchoolLookupMetricInstanceSetKeyResolver studentService;

        //The Injected Dependencies.
        protected IRepository<StudentSchoolMetricInstanceSet> StudentSchoolMetricInstanceSetRepository;
        protected IRepository<SchoolMetricInstanceSet> SchoolMetricInstanceSetRepository;
        protected IRepository<LocalEducationAgencyMetricInstanceSet> localEducationAgencyRepository;

        //The Actual Model.
        protected Guid actualModel;

        //The supplied Data models.
        protected int suppliedStudentUSI = 1;
        protected int suppliedSchoolId = 10;
        protected int suppliedLocalEducationAgencyId = 100;
        protected IQueryable<StudentSchoolMetricInstanceSet> suppliedStudentSchoolMetricInstanceSetData;
        protected IQueryable<SchoolMetricInstanceSet> suppliedSchoolMetricInstanceSetData;
        protected IQueryable<LocalEducationAgencyMetricInstanceSet> suppliedLocalEducationAgencyData;

        protected override void EstablishContext()
        {
            //Prepare supplied data collections
            suppliedStudentSchoolMetricInstanceSetData = GetSuppliedStudentSchoolMetricInstanceSetData();
            suppliedSchoolMetricInstanceSetData = GetSuppliedSchoolMetricInstanceSetData();
            suppliedLocalEducationAgencyData = GetSuppliedLocalEducationAgencyData();

            //Set up the mocks
            StudentSchoolMetricInstanceSetRepository = mocks.StrictMock<IRepository<StudentSchoolMetricInstanceSet>>();
            SchoolMetricInstanceSetRepository = mocks.StrictMock<IRepository<SchoolMetricInstanceSet>>();
            localEducationAgencyRepository = mocks.StrictMock<IRepository<LocalEducationAgencyMetricInstanceSet>>();

            //Set expectations
            //Expect.Call().Return();
        }

        protected IQueryable<StudentSchoolMetricInstanceSet> GetSuppliedStudentSchoolMetricInstanceSetData()
        {
            return (new List<StudentSchoolMetricInstanceSet>
                        {
                            new StudentSchoolMetricInstanceSet{ MetricInstanceSetKey = new Guid("11111111-1111-1111-1111-111111111111"), SchoolId = suppliedSchoolId, StudentUSI = suppliedStudentUSI },
                            new StudentSchoolMetricInstanceSet{ MetricInstanceSetKey = new Guid("22222222-2222-2222-2222-222222222222"), SchoolId = suppliedSchoolId, StudentUSI = 99 },
                            new StudentSchoolMetricInstanceSet{ MetricInstanceSetKey = new Guid("33333333-3333-3333-3333-333333333333"), SchoolId = 99, StudentUSI = suppliedStudentUSI },
                            new StudentSchoolMetricInstanceSet{ MetricInstanceSetKey = new Guid("44444444-4444-4444-4444-444444444444"), SchoolId = 99, StudentUSI = 99 },
                        }).AsQueryable();
        }

        protected IQueryable<SchoolMetricInstanceSet> GetSuppliedSchoolMetricInstanceSetData()
        {
            return (new List<SchoolMetricInstanceSet>
                        {
                            new SchoolMetricInstanceSet{ MetricInstanceSetKey = new Guid("11111111-1111-1111-1111-111111111111"), SchoolId = 99 },
                            new SchoolMetricInstanceSet{ MetricInstanceSetKey = new Guid("55555555-5555-5555-5555-555555555555"), SchoolId = suppliedSchoolId },
                        }).AsQueryable();
        }

        protected IQueryable<LocalEducationAgencyMetricInstanceSet> GetSuppliedLocalEducationAgencyData()
        {
            return (new List<LocalEducationAgencyMetricInstanceSet>
                        {
                            new LocalEducationAgencyMetricInstanceSet{ MetricInstanceSetKey = new Guid("66666666-6666-6666-6666-666666666666"), LocalEducationAgencyId = suppliedLocalEducationAgencyId},
                            new LocalEducationAgencyMetricInstanceSet{ MetricInstanceSetKey = new Guid("22222222-2222-2222-2222-222222222222"), LocalEducationAgencyId = 99},
                        }).AsQueryable();
        }

        protected override void ExecuteTest()
        {
            localEducationAgencyService = new LocalEducationAgencyLookupMetricInstanceSetKeyResolver(localEducationAgencyRepository);
            schoolService = new SchoolLookupMetricInstanceSetKeyResolver(SchoolMetricInstanceSetRepository);
            studentService = new StudentSchoolLookupMetricInstanceSetKeyResolver(StudentSchoolMetricInstanceSetRepository);
        }

        [Test]
        public void Should_return_a_Guid()
        {
            Assert.That(actualModel, Is.Not.Null);
        }

    }

    [TestFixture]
    public class When_invoking_the_Lookup_Metric_Instance_Set_Key_Resolver_to_get_a_Student_Domain_Entity_Key : When_invoking_the_Lookup_Metric_Instance_Set_Key_Resolver
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            //Set expectations
            Expect.Call(StudentSchoolMetricInstanceSetRepository.GetAll()).Return(suppliedStudentSchoolMetricInstanceSetData);
        }

        protected override void ExecuteTest()
        {
            base.ExecuteTest();
            actualModel = studentService.GetMetricInstanceSetKey(StudentSchoolMetricInstanceSetRequest.Create(suppliedSchoolId, suppliedStudentUSI, 0));
        }

        [Test]
        public void Should_return_the_correct_student_Guid()
        {
            var suppliedGuid = suppliedStudentSchoolMetricInstanceSetData.Single(x => x.SchoolId == suppliedSchoolId && x.StudentUSI == suppliedStudentUSI).MetricInstanceSetKey;
            Assert.That(actualModel, Is.EqualTo(suppliedGuid));
        }
    }

    [TestFixture]
    public class When_invoking_the_Lookup_Metric_Instance_Set_Key_Resolver_to_get_a_School_Domain_Entity_Key : When_invoking_the_Lookup_Metric_Instance_Set_Key_Resolver
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            //Set expectations
            Expect.Call(SchoolMetricInstanceSetRepository.GetAll()).Return(suppliedSchoolMetricInstanceSetData);
        }

        protected override void ExecuteTest()
        {
            base.ExecuteTest();
            actualModel = schoolService.GetMetricInstanceSetKey(SchoolMetricInstanceSetRequest.Create(suppliedSchoolId, 0));
        }

        [Test]
        public void Should_return_the_correct_school_Guid()
        {
            var suppliedGuid = suppliedSchoolMetricInstanceSetData.Single(x => x.SchoolId == suppliedSchoolId).MetricInstanceSetKey;
            Assert.That(actualModel, Is.EqualTo(suppliedGuid));
        }
    }

    [TestFixture]
    public class When_invoking_the_Lookup_Metric_Instance_Set_Key_Resolver_to_get_a_LEA_Domain_Entity_Key : When_invoking_the_Lookup_Metric_Instance_Set_Key_Resolver
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            //Set expectations
            Expect.Call(localEducationAgencyRepository.GetAll()).Return(suppliedLocalEducationAgencyData);
        }

        protected override void ExecuteTest()
        {
            base.ExecuteTest();
            actualModel = localEducationAgencyService.GetMetricInstanceSetKey(LocalEducationAgencyMetricInstanceSetRequest.Create(suppliedLocalEducationAgencyId, 0));
        }

        [Test]
        public void Should_return_the_correct_Local_Education_Agency_Guid()
        {
            var suppliedGuid = suppliedLocalEducationAgencyData.Single(x => x.LocalEducationAgencyId == suppliedLocalEducationAgencyId).MetricInstanceSetKey;
            Assert.That(actualModel, Is.EqualTo(suppliedGuid));
        }
    }

    [TestFixture]
    public class When_invoking_the_Lookup_Metric_Instance_Set_Key_Resolver_to_get_a_Domain_Entity_Key : When_invoking_the_Lookup_Metric_Instance_Set_Key_Resolver
    {
        private Guid actualStudentSchoolMetricInstanceSet;
        private Guid actualSchoolMetricInstanceSet;
        private Guid actualLocalEducationAgencyMetricInstanceSet;

        protected override void EstablishContext()
        {
            base.EstablishContext();

            //Set expectations
            Expect.Call(StudentSchoolMetricInstanceSetRepository.GetAll()).Return(suppliedStudentSchoolMetricInstanceSetData);
            Expect.Call(SchoolMetricInstanceSetRepository.GetAll()).Return(suppliedSchoolMetricInstanceSetData);
            Expect.Call(localEducationAgencyRepository.GetAll()).Return(suppliedLocalEducationAgencyData);
        }

        protected override void ExecuteTest()
        {
            base.ExecuteTest();

            actualStudentSchoolMetricInstanceSet = studentService.GetMetricInstanceSetKey(StudentSchoolMetricInstanceSetRequest.Create(suppliedSchoolId, suppliedStudentUSI, 0));
            actualSchoolMetricInstanceSet = schoolService.GetMetricInstanceSetKey(SchoolMetricInstanceSetRequest.Create(suppliedSchoolId, 0));
            actualLocalEducationAgencyMetricInstanceSet = localEducationAgencyService.GetMetricInstanceSetKey(LocalEducationAgencyMetricInstanceSetRequest.Create(suppliedLocalEducationAgencyId, 0));
        }

        [Test]
        public void Should_return_the_correct_student_Guid()
        {
            var suppliedStudentGuid = suppliedStudentSchoolMetricInstanceSetData.Single(x => x.SchoolId == suppliedSchoolId && x.StudentUSI == suppliedStudentUSI).MetricInstanceSetKey;
            Assert.That(actualStudentSchoolMetricInstanceSet, Is.EqualTo(suppliedStudentGuid));
        }

        [Test]
        public void Should_return_the_correct_school_Guid()
        {
            var suppliedSchoolGuid = suppliedSchoolMetricInstanceSetData.Single(x => x.SchoolId == suppliedSchoolId).MetricInstanceSetKey;
            Assert.That(actualSchoolMetricInstanceSet, Is.EqualTo(suppliedSchoolGuid));
        }

        [Test]
        public void Should_return_the_correct_Local_Education_Agency_Guid()
        {
            var suppliedLEAGuid = suppliedLocalEducationAgencyData.Single(x => x.LocalEducationAgencyId == suppliedLocalEducationAgencyId).MetricInstanceSetKey;
            Assert.That(actualLocalEducationAgencyMetricInstanceSet, Is.EqualTo(suppliedLEAGuid));
        }
    }
}
