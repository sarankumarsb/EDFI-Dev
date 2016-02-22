// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.MetricInstanceSetKeyResolvers;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Testing;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Tests.Metric
{
    [TestFixture]
    public class When_resolving_the_metric_instance_set_key_for_a_local_education_agency_metric_set : TestFixtureBase
    {
        private Guid actualKey;

        protected override void ExecuteTest()
        {
            var resolver = new SsisMultipleHashMetricInstanceSetKeyResolverConvertLongToInt<LocalEducationAgencyMetricInstanceSetRequest>();

            actualKey = resolver.GetMetricInstanceSetKey(
                new LocalEducationAgencyMetricInstanceSetRequest
                    {
                        // Supplied value pulled from a loaded database for Allen ISD
                        LocalEducationAgencyId = 43901
                    });
        }

        [Test]
        public void Should_generate_the_hash_code_using_the_local_education_agency_domain_entity_type_id_and_the_local_education_agency_identifier_with_null_safe_handling()
        {
            // Expected value pulled from a loaded database for Allen ISD
            Assert.That(actualKey, Is.EqualTo(new Guid("1E41CD7E-F9B9-ABBD-134A-4A47DA20941E")));
        }
    }

    [TestFixture]
    public class When_resolving_the_metric_instance_set_key_for_a_school_metric_set : TestFixtureBase
    {
        private Guid actualKey;

        protected override void ExecuteTest()
        {
            var resolver = new SsisMultipleHashMetricInstanceSetKeyResolverConvertLongToInt<SchoolMetricInstanceSetRequest>();

            actualKey = resolver.GetMetricInstanceSetKey(
                new SchoolMetricInstanceSetRequest
                    {
                        // Supplied value pulled from a loaded database for Allen ISD
                        SchoolId = 43901001
                    });
        }

        [Test]
        public void Should_generate_the_hash_code_using_the_school_domain_entity_type_id_and_the_school_identifier_with_null_safe_handling()
        {
            // Expected value pulled from a loaded database for Allen ISD
            Assert.That(actualKey, Is.EqualTo(new Guid("277DCF3C-060D-EA8F-D3BA-4020A7634986")));
        }
    }

    [TestFixture]
    public class When_resolving_the_metric_instance_set_key_for_a_student_school_metric_set_for_SsisMultipleHashMetricInstanceSetKeyResolverConvertLongToInt : TestFixtureBase
    {
        private Guid actualKey;

        protected override void ExecuteTest()
        {
            var resolver = new SsisMultipleHashMetricInstanceSetKeyResolverConvertLongToInt<StudentSchoolMetricInstanceSetRequest>();

            actualKey = resolver.GetMetricInstanceSetKey(
                new StudentSchoolMetricInstanceSetRequest
                    {
                        // Supplied values pulled from a loaded databsae for Allen ISD
                        SchoolId = 43901001,
                        StudentUSI = 20213,
                    });
        }

        [Test]
        public void Should_generate_the_hash_code_using_the_student_domain_entity_type_id_and_the_student_and_school_identifiers_with_null_safe_handling()
        {
            // Expected value pulled from a loaded database for Allen ISD
            Assert.That(actualKey, Is.EqualTo(new Guid("3EAF234A-BD1C-B024-58D9-6B2571CEEC36")));
        }
    }

    [TestFixture]
    public class When_resolving_the_metric_instance_set_key_for_a_student_school_metric_set_for_SsisMultipleHashMetricInstanceSetKeyResolverHandleLongNormally : TestFixtureBase
    {
        private Guid actualKey;

        protected override void ExecuteTest()
        {
            var resolver = new SsisMultipleHashMetricInstanceSetKeyResolverHandleLongNormally<StudentSchoolMetricInstanceSetRequest>();

            actualKey = resolver.GetMetricInstanceSetKey(
                new StudentSchoolMetricInstanceSetRequest
                {
                    // Supplied values pulled from a loaded databsae for Allen ISD
                    SchoolId = 43901001,
                    StudentUSI = 20213,
                });
        }

        [Test]
        public void Should_generate_the_hash_code_using_the_student_domain_entity_type_id_and_the_student_and_school_identifiers_with_null_safe_handling()
        {
            // Expected value pulled from a loaded database for Allen ISD
            Assert.That(actualKey, Is.EqualTo(new Guid("7532d043-6e3f-4249-3e1d-1e258b6909de")));
        }
    }
}