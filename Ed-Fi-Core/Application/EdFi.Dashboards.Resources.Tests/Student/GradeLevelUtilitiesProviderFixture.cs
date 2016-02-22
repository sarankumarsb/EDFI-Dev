using System;
using System.Collections.Generic;
using System.Text;
using EdFi.Dashboards.Resources.StudentSchool;
using EdFi.Dashboards.Testing;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Tests.Student
{
    public class When_sorting_grade_level_in_a_student_list : TestFixtureBase
    {
        protected IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;
        protected Dictionary<string, int> expectedValues;
        protected Dictionary<string, int> actualValues;

        protected override void EstablishContext()
        {
            //initialize 
            expectedValues = new Dictionary<string, int>();
            actualValues = new Dictionary<string, int>();
            gradeLevelUtilitiesProvider = new GradeLevelUtilitiesProvider();

            //set expected values
            expectedValues.Add("1st Grade", 1);
            expectedValues.Add("2nd Grade", 2);
            expectedValues.Add("3rd Grade", 3);
            expectedValues.Add("4th Grade", 4);
            expectedValues.Add("5th Grade", 5);
            expectedValues.Add("6th Grade", 6);
            expectedValues.Add("7th Grade", 7);
            expectedValues.Add("8th Grade", 8);
            expectedValues.Add("9th Grade", 9);
            expectedValues.Add("10th Grade", 10);
            expectedValues.Add("11th Grade", 11);
            expectedValues.Add("12th Grade", 12);
            expectedValues.Add("Postsecondary", 13);
            expectedValues.Add("Early Education", -3);
            expectedValues.Add("Infant/toddler", -2);
            expectedValues.Add("Preschool/Prekindergarten", -1);
            expectedValues.Add("Transitional Kindergarten", 0);
            expectedValues.Add("Kindergarten", 0);
            expectedValues.Add("Ungraded", 14);
            expectedValues.Add("Should not match", 15);
           
        }

        protected override void ExecuteTest()
        {
            foreach (var expectedValue in expectedValues)
            {
                actualValues.Add(expectedValue.Key, gradeLevelUtilitiesProvider.FormatGradeLevelForSorting(expectedValue.Key));
            }
        }

        [Test]
        public void Should_give_correct_sort_order_based_on_grade_level()
        {
            StringBuilder results = new StringBuilder();
            foreach (var actualValue in actualValues)
            {
                if (actualValue.Value != expectedValues[actualValue.Key])
                {
                    results.AppendLine(String.Format("Expected value:{0} was different from actual value:{1} for input grade level of '{2}'.", expectedValues[actualValue.Key], actualValue.Value, actualValue.Key));
                }
            }
            if (results.Length != 0)
            {
                Assert.Fail(results.ToString());
            }
        }
    }

    public class When_displaying_grade_level_in_a_student_list : TestFixtureBase
    {
        protected IGradeLevelUtilitiesProvider gradeLevelUtilitiesProvider;
        protected Dictionary<string, string> expectedValues;
        protected Dictionary<string, string> actualValues;

        protected override void EstablishContext()
        {
            //initialize 
            expectedValues = new Dictionary<string, string>();
            actualValues = new Dictionary<string, string>();
            gradeLevelUtilitiesProvider = new GradeLevelUtilitiesProvider();

            //set expected values
            expectedValues.Add("1st Grade", "1st");
            expectedValues.Add("2nd Grade", "2nd");
            expectedValues.Add("3rd Grade", "3rd");
            expectedValues.Add("4th Grade", "4th");
            expectedValues.Add("5th Grade", "5th");
            expectedValues.Add("6th Grade", "6th");
            expectedValues.Add("7th Grade", "7th");
            expectedValues.Add("8th Grade", "8th");
            expectedValues.Add("9th Grade", "9th");
            expectedValues.Add("10th Grade", "10th");
            expectedValues.Add("11th Grade", "11th");
            expectedValues.Add("12th Grade", "12th");
            expectedValues.Add("Postsecondary", "Post");
            expectedValues.Add("Early Education", "E-E");
            expectedValues.Add("Infant/toddler", "Inf");
            expectedValues.Add("Preschool/Prekindergarten", "Pre");
            expectedValues.Add("Transitional Kindergarten", "T-K");
            expectedValues.Add("Kindergarten", "K");
            expectedValues.Add("Ungraded", "U");
            expectedValues.Add("Should not match", "NA");

        }

        protected override void ExecuteTest()
        {
            foreach (var expectedValue in expectedValues)
            {
                actualValues.Add(expectedValue.Key, gradeLevelUtilitiesProvider.FormatGradeLevelForDisplay(expectedValue.Key));
            }
        }

        [Test]
        public void Should_display_correct_text_based_on_grade_level()
        {
            StringBuilder results = new StringBuilder();
            foreach (var actualValue in actualValues)
            {
                if (actualValue.Value != expectedValues[actualValue.Key])
                {
                    results.AppendLine(String.Format("Expected value:{0} was different from actual value:{1} for input grade level of '{2}'.", expectedValues[actualValue.Key], actualValue.Value, actualValue.Key));
                }
            }
            if (results.Length != 0)
            {
                Assert.Fail(results.ToString());
            }
        }
    }
}
