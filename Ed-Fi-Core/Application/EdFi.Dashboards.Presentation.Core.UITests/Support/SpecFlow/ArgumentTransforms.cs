using System;
using System.Text.RegularExpressions;
using TechTalk.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow
{
    /// <summary>
    /// Transforms text with embedded spaces to enumeration values.
    /// </summary>
    /// <typeparam name="TEnum">The <see cref="Enum"/> type to transform.</typeparam>
    public class EnumTransform<TEnum>
    {
        [StepArgumentTransformation]
        public TEnum TransformEnum(string text)
        {
            string enumText = Regex.Replace(text, @"\s+", "");

            var result = (TEnum)Enum.Parse(typeof(TEnum), enumText, true);

            return result;
        }
    }

    [Binding]
    public class AcademicDashboardTypeTransform 
        : EnumTransform<AcademicDashboardType> {}

    [Binding]
    public class SchoolTypeTransform 
        : EnumTransform<SchoolType> { }
}
