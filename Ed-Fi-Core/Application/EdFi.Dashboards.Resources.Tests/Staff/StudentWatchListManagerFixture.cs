using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Resources.StudentMetrics;
using EdFi.Dashboards.Testing;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Tests.Staff
{
    public abstract class StudentWatchListManagerFixtureBase : TestFixtureBase
    {
        protected List<NameValuesType> SuppliedWatchListData;
        protected int[] SuppliedMetricVariantIds;
        protected int SuppliedSchoolId;
        protected int SuppliedLocalEducationAgencyId;
        protected long? SuppliedStaffUsi;
        protected List<long> SuppliedStudentIds;
        protected long? SuppliedSectionOrCohortId;
        protected StudentListType SuppliedStudentListType;
        protected IEnumerable<SelectionOptionGroup> SuppliedDemographicOptionGroups;
        protected string SuppliedSchoolCategory;
        protected string SuppliedGradeLevel;

        protected StudentWatchListManager StudentWatchListManager;
        protected StudentMetricsProviderQueryOptions Result;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            StudentWatchListManager = new StudentWatchListManager();

            SuppliedMetricVariantIds = new int[0];
            SuppliedStudentIds = new List<long>();
        }

        protected override void ExecuteTest()
        {
            Result = StudentWatchListManager.CreateStudentMetricsProviderQueryOptions(
                SuppliedWatchListData,
                SuppliedMetricVariantIds,
                SuppliedSchoolId,
                SuppliedLocalEducationAgencyId,
                SuppliedStaffUsi,
                SuppliedStudentIds,
                SuppliedSectionOrCohortId,
                SuppliedStudentListType,
                SuppliedDemographicOptionGroups,
                SuppliedSchoolCategory,
                SuppliedGradeLevel
                );
        }

        [Test]
        public virtual void Should_not_be_null()
        {
            Assert.That(Result, Is.Not.Null);
        }

        [Test]
        public virtual void Should_set_correct_TeacherSectionIds_argument()
        {
            Assert.That(Result.TeacherSectionIds, Has.Length.EqualTo(0));
        }

        [Test]
        public virtual void Should_set_correct_StaffCohortIds_argument()
        {
            Assert.That(Result.StaffCohortIds, Is.Null);
        }

        [Test]
        public virtual void Should_set_correct_StaffUSI_argument()
        {
            Assert.That(Result.StaffUSI, Is.EqualTo(0));
        }

        [Test]
        public virtual void Should_set_correct_SchoolId_argument()
        {
            Assert.That(Result.SchoolId, Is.EqualTo(0));
        }

        [Test]
        public virtual void Should_set_correct_LocalEducationAgencyId_argument()
        {
            Assert.That(Result.LocalEducationAgencyId, Is.EqualTo(0));
        }

        [Test]
        public virtual void Should_set_correct_StudentIds_argument()
        {
            Assert.That(Result.StudentIds, Is.Null);
        }

        [Test]
        public virtual void Should_set_correct_MetricVariantIds_argument()
        {
            Assert.That(Result.MetricVariantIds.Count(), Is.EqualTo(0));
        }

        [Test]
        public virtual void Should_set_correct_GetAllMetrics_argument()
        {
            Assert.That(Result.GetAllMetrics, Is.False);
        }

        [Test]
        public virtual void Should_set_correct_SchoolCategory_argument()
        {
            Assert.That(Result.SchoolCategory, Is.Null);
        }

        [Test]
        public virtual void Should_set_correct_SchoolMetricStudentListMetricId_argument()
        {
            Assert.That(Result.SchoolMetricStudentListMetricId, Is.Null);
        }

        [Test]
        public virtual void Should_set_correct_GradeLevel_argument()
        {
            Assert.That(Result.GradeLevel, Is.Null.Or.EquivalentTo(new string[0]));
        }

        [Test]
        public virtual void Should_set_correct_DemographicOptionGroups_argument()
        {
            Assert.That(Result.DemographicOptionGroups, Is.Null.Or.EquivalentTo(new SelectionOptionGroup[0]));
        }

        [Test]
        public virtual void Should_set_correct_MetricOptionGroups_argument()
        {
            Assert.That(Result.MetricOptionGroups, Is.Null.Or.All.Property("MetricFilterOptions").Length.EqualTo(0));
        }

        [Test]
        public virtual void Should_set_correct_AssessmentOptionGroups_argument()
        {
            Assert.That(Result.AssessmentOptionGroups, Is.Null);
        }
    }

    public class When_no_options_are_passed_in : StudentWatchListManagerFixtureBase
    {
        //This tests doesn't need to override any of the base methods of the test.
        // the base methods verify nothing happened, and the base Context creates a blank request.
    }

    public class When_passed_in_MetricVariantIds : StudentWatchListManagerFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            SuppliedMetricVariantIds = new[] { 1, 2, 3 };
        }

        [Test]
        public override void Should_set_correct_MetricVariantIds_argument()
        {
            Assert.That(Result.MetricVariantIds, Has.Length.EqualTo(3));
            Assert.That(Result.MetricVariantIds, Has.Length.EqualTo(3));
        }
    }

    public class When_passed_in_SchoolId : StudentWatchListManagerFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            SuppliedSchoolId = 1234;
        }

        [Test]
        public override void Should_set_correct_SchoolId_argument()
        {
            Assert.That(Result.SchoolId, Is.EqualTo(SuppliedSchoolId));
        }
    }

    public class When_passed_in_LocalEducationAgencyId : StudentWatchListManagerFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            SuppliedLocalEducationAgencyId = 1234;
        }

        [Test]
        public override void Should_set_correct_LocalEducationAgencyId_argument()
        {
            Assert.That(Result.LocalEducationAgencyId, Is.EqualTo(SuppliedLocalEducationAgencyId));
        }
    }

    public class When_passed_in_StaffUSI : StudentWatchListManagerFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            SuppliedStaffUsi = 1234;
        }

        [Test]
        public override void Should_set_correct_StaffUSI_argument()
        {
            Assert.That(Result.StaffUSI, Is.EqualTo(SuppliedStaffUsi));
        }
    }

    public class When_passed_in_StudentIds : StudentWatchListManagerFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            SuppliedStudentIds = new List<long>{1,2,3,4};
        }

        [Test]
        public override void Should_set_correct_StudentIds_argument()
        {
            Assert.That(Result.StudentIds, Has.Count.EqualTo(4));
        }
    }

    public class When_passed_a_section_id_into_SuppliedSectionOrCohortId : StudentWatchListManagerFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            SuppliedSectionOrCohortId = 1234;
            SuppliedStudentListType = StudentListType.Section;
        }

        [Test]
        public override void Should_set_correct_TeacherSectionIds_argument()
        {
            Assert.That(Result.TeacherSectionIds, Has.Length.EqualTo(1));
            Assert.That(Result.TeacherSectionIds, Has.Exactly(1).EqualTo(SuppliedSectionOrCohortId));
        }
    }

    public class When_passed_a_cohort_id_into_SuppliedSectionOrCohortId : StudentWatchListManagerFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            SuppliedSectionOrCohortId = 1234;
            SuppliedStudentListType = StudentListType.Cohort;
        }

        [Test]
        public override void Should_set_correct_StaffCohortIds_argument()
        {
            Assert.That(Result.StaffCohortIds, Has.Length.EqualTo(1));
            Assert.That(Result.StaffCohortIds, Has.Exactly(1).EqualTo(SuppliedSectionOrCohortId));
        }
    }

    public class When_passed_in_DemographicOptionGroups : StudentWatchListManagerFixtureBase
    {
        private SelectionOptionGroup OptionGroup;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            OptionGroup = new SelectionOptionGroup
            {
                SelectionOptionName = "demographic-demographics",
                SelectedOptions = new[] { "White" }
            };
            SuppliedDemographicOptionGroups = new[] { OptionGroup };
        }

        [Test]
        public override void Should_set_correct_DemographicOptionGroups_argument()
        {
            Assert.That(Result.DemographicOptionGroups, Is.EquivalentTo(new[] { OptionGroup }));
        }
    }

    public class When_passed_in_SchoolCategory : StudentWatchListManagerFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            SuppliedSchoolCategory = "Test Category";
        }

        [Test]
        public override void Should_set_correct_SchoolCategory_argument()
        {
            Assert.That(Result.SchoolCategory, Is.EqualTo(SuppliedSchoolCategory));
        }
    }

    public class When_passed_in_GradeLevel : StudentWatchListManagerFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            SuppliedGradeLevel = "Test GradeLevel";
        }

        [Test]
        public override void Should_set_correct_GradeLevel_argument()
        {
            Assert.That(Result.GradeLevel, Is.EquivalentTo(new[] {SuppliedGradeLevel}));
        }
    }

    public class When_passed_a_section_id_into_SuppliedSectionOrCohortId_and_selected_classes_are_passed_in : StudentWatchListManagerFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();
            SuppliedSectionOrCohortId = 1234;
            SuppliedStudentListType = StudentListType.Section;

            SuppliedWatchListData = new List<NameValuesType>{new NameValuesType
                {
                    Name = "selected-classes",
                    Values = new List<string>
                        {
                            "2345",
                            "3456"
                        }
                }};
        }

        [Test]
        public override void Should_set_correct_TeacherSectionIds_argument()
        {
            Assert.That(Result.TeacherSectionIds, Has.Length.EqualTo(2));
            Assert.That(Result.TeacherSectionIds, Has.Exactly(1).EqualTo(2345));
            Assert.That(Result.TeacherSectionIds, Has.Exactly(1).EqualTo(3456));
        }
    }

    public class When_passed_in_DemographicOptionGroups_and_all_genders_demographic_is_passed_in : StudentWatchListManagerFixtureBase
    {
        private SelectionOptionGroup OptionGroup;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            OptionGroup = new SelectionOptionGroup
            {
                SelectionOptionName = "demographic-demographics",
                SelectedOptions = new[] { "White" }
            };
            SuppliedDemographicOptionGroups = new[] { OptionGroup };

            SuppliedWatchListData = new List<NameValuesType>{new NameValuesType
                {
                    Name = "gender",
                    Values = new List<string>
                        {
                            "all-genders",
                        }
                }};
        }

        [Test]
        public override void Should_set_correct_DemographicOptionGroups_argument()
        {
            Assert.That(Result.DemographicOptionGroups, Is.EquivalentTo(new[] { OptionGroup }));
        }
    }

    public class When_passed_in_DemographicOptionGroups_and_one_gender_demographic_is_passed_in : StudentWatchListManagerFixtureBase
    {
        private SelectionOptionGroup OptionGroup;

        protected override void EstablishContext()
        {
            base.EstablishContext();
            OptionGroup = new SelectionOptionGroup
            {
                SelectionOptionName = "demographic-demographics",
                SelectedOptions = new[] { "White" }
            };
            SuppliedDemographicOptionGroups = new[] { OptionGroup };

            SuppliedWatchListData = new List<NameValuesType>{new NameValuesType
                {
                    Name = "gender",
                    Values = new List<string>
                        {
                            "male",
                        }
                }};
        }

        [Test]
        public override void Should_set_correct_DemographicOptionGroups_argument()
        {
            //This should return both options.
            Assert.That(Result.DemographicOptionGroups, Has.Length.EqualTo(2));
            Assert.That(Result.DemographicOptionGroups, Has.Exactly(1).EqualTo(OptionGroup));
            Assert.That(Result.DemographicOptionGroups, Has.Exactly(1)
                .Property("SelectionOptionName").EqualTo("demographic-demographics")
                .And.Property("SelectedOptions").EqualTo(new[] { "White" }));
        }
    }

    public class When_passed_in_GradeLevel_and_grade_watchlist_option : StudentWatchListManagerFixtureBase
    {
        private const string SecondTestGradeLevel = "Second Test GradeLevel";

        protected override void EstablishContext()
        {
            base.EstablishContext();
            SuppliedGradeLevel = "Test GradeLevel";

            SuppliedWatchListData = new List<NameValuesType>{new NameValuesType
                {
                    Name = "grade",
                    Values = new List<string>
                        {
                            SecondTestGradeLevel,
                        }
                }};
        }

        [Test]
        public override void Should_set_correct_GradeLevel_argument()
        {
            Assert.That(Result.GradeLevel, Is.EquivalentTo(new[] { SuppliedGradeLevel, SecondTestGradeLevel }));
        }
    }

    public class When_passed_in_assessment_watchlist_option : StudentWatchListManagerFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            SuppliedWatchListData = new List<NameValuesType>{new NameValuesType
                {
                    Name = "assessment-1234",
                    Values = new List<string>
                        {
                            "below-basic",
                        }
                }};
        }

        [Test]
        public override void Should_set_correct_MetricOptionGroups_argument()
        {
            var group = Result.MetricOptionGroups.SingleOrDefault(mog => mog.MetricFilterOptions.Any());
            Assert.That(group, Is.Not.Null);
            Assert.That(group.MetricFilterOptions[0].MaxExclusiveMetricInstanceExtendedProperty, Is.EqualTo("Level2CutScore"));
            Assert.That(group.MetricFilterOptions[0].MetricId, Is.EqualTo(1234));
            Assert.That(group.MetricFilterOptions[0].MinInclusiveMetricInstanceExtendedProperty, Is.EqualTo("MinScore"));
        }
    }

    public class When_passed_in_dropdown_assessment_watchlist_option : StudentWatchListManagerFixtureBase
    {
        protected override void EstablishContext()
        {
            base.EstablishContext();

            SuppliedWatchListData = new List<NameValuesType>{new NameValuesType
                {
                    Name = "drp-assessment-1234",
                    Values = new List<string>
                        {
                            "below-goal",
                        }
                }};
        }

        [Test]
        public override void Should_set_correct_MetricOptionGroups_argument()
        {
            var group = Result.MetricOptionGroups.SingleOrDefault(mog => mog.MetricFilterOptions.Any());
            Assert.That(group, Is.Not.Null);
            Assert.That(group.MetricFilterOptions[0].MetricStateEqualTo, Is.EqualTo((int)MetricStateType.Low));
            Assert.That(group.MetricFilterOptions[0].MetricId, Is.EqualTo(1234));
        }
    }
}