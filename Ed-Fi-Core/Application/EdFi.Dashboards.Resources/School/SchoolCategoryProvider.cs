// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.School
{
    public interface ISchoolCategoryProvider
    {
        SchoolCategory GetSchoolCategoryType(int schoolId);
        SchoolCategory GetSchoolCategoryType(string category);
        int GetSchoolCategoryPriorityForSorting(string category);
    }
    //NOTE: if you change anything here you need to update the JavaScript in the SchoolMetricTable.aspx or related MVC view.
    public class SchoolCategoryProvider : ISchoolCategoryProvider
    {
        private readonly IIdNameService idNameService;

        public SchoolCategoryProvider(IIdNameService idNameService)
        {
            this.idNameService = idNameService;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyMetrics)]
        public SchoolCategory GetSchoolCategoryType(int schoolId)
        {
            var schoolInformationData = idNameService.Get(IdNameRequest.Create(schoolId));
            if (schoolInformationData == null)
                throw new InvalidOperationException(String.Format("School Id:{0} Does not exist in the Database.", schoolId));

            var category = schoolInformationData.SchoolCategory;
            if (string.IsNullOrEmpty(category))
                throw new InvalidOperationException(String.Format("No category defined for School Id:{0}.", schoolId));

            return GetSchoolCategoryType(category);
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyMetrics)]
        public SchoolCategory GetSchoolCategoryType(string category)
        {
            switch (category.Trim())
            {
                case "Elementary School":
                case "Elementary":
                    return SchoolCategory.Elementary;
                case "Middle School":
                case "Middle":
                    return SchoolCategory.MiddleSchool;
                default:
                    return SchoolCategory.HighSchool;
            }
        }

        public int GetSchoolCategoryPriorityForSorting(string category)
        {
            switch (category.Trim())
            {
                case "High":
                case "High School":
                    return 1;
                case "Junior":
                case "Junior High":
                case "Junior High School":
                    return 2;
                case "Middle":
                case "Middle School":
                    return 3;
                case "Elementary":
                case "Elementary School":
                    return 4;
                case "Elementary/Secondary":
                case "Elementary/Secondary School":
                    return 5;
                case "Secondary":
                case "Secondary School":
                case "SecondarySchool":
                    return 6;
                case "Ungraded":
                    return 7;
                case "Adult":
                case "Adult School":
                    return 8;
                case "Infant/toddler":
                case "Infant/toddler School":
                    return 9;
                case "Preschool/early childhood":
                    return 10;
                case "Primary":
                case "Primary School":
                    return 11;
                case "Intermediate":
                case "Intermediate School":
                    return 12;
            }
            return 99;
        }
    }
}
