// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Testing;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Tests.Student
{
    public class When_loading_grade_metric_state : TestFixtureBase
    {
        private readonly GradeStateProvider gradeStateProvider = new GradeStateProvider();

        protected override void ExecuteTest()
        {
        }

        [Test]
        public void Above_seventy_is_good()
        {
            Assert.That(gradeStateProvider.Get(71), Is.EqualTo(MetricStateType.Good));
        }

        [Test]
        public void Seventy_is_good()
        {
            Assert.That(gradeStateProvider.Get(70), Is.EqualTo(MetricStateType.Good));
        }

        [Test]
        public void Below_seventy_is_bad()
        {
            Assert.That(gradeStateProvider.Get(69), Is.EqualTo(MetricStateType.Low));
        }

        [Test]
        public void C_plus_is_good()
        {
            Assert.That(gradeStateProvider.Get("C+"), Is.EqualTo(MetricStateType.Good));
        }

        [Test]
        public void C_minus_is_good()
        {
            Assert.That(gradeStateProvider.Get("C-"), Is.EqualTo(MetricStateType.Good));
        }

        [Test]
        public void C_is_good()
        {
            Assert.That(gradeStateProvider.Get("C"), Is.EqualTo(MetricStateType.Good));
        }

        [Test]
        public void B_plus_is_good()
        {
            Assert.That(gradeStateProvider.Get("B"), Is.EqualTo(MetricStateType.Good));
        }

        [Test]
        public void D_plus_is_bad()
        {
            Assert.That(gradeStateProvider.Get("D+"), Is.EqualTo(MetricStateType.Low));
        }

        [Test]
        public void D_is_bad()
        {
            Assert.That(gradeStateProvider.Get("D"), Is.EqualTo(MetricStateType.Low));
        }

        [Test]
        public void G_is_good()
        {
            Assert.That(gradeStateProvider.Get("G"), Is.EqualTo(MetricStateType.Good));
        }

        [Test]
        public void H_is_good()
        {
            Assert.That(gradeStateProvider.Get("H"), Is.EqualTo(MetricStateType.Good));
        }

        [Test]
        public void S_is_good()
        {
            Assert.That(gradeStateProvider.Get("S"), Is.EqualTo(MetricStateType.Good));
        }
    }
}
