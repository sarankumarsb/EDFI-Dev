using System;
using TechTalk.SpecFlow;
using EdFi.Dashboards.Presentation.Core.UITests.Support.SpecFlow;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support
{
    /// <summary>
    /// Provides details about a specific user profile for use in testing scenarios.
    /// </summary>
    public class UserProfile
    {
        public static readonly UserProfile Null =
            new UserProfile
                {
                    FullName = string.Empty,
                    Password = string.Empty,
                    ProfileName = string.Empty,
                    HighSchoolId = null,
                    MiddleSchoolId = null,
                    ElementarySchoolId = null,
                    StudentUSI = null,
                    Username = string.Empty,
                };

        /// <summary>
        /// Gets or sets the name of the user profile.
        /// </summary>
        public string ProfileName { get; set; }

        /// <summary>
        /// Gets or sets the user's full name.
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Gets or sets the username of the user.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user's main associated high school.
        /// </summary>
        public int StaffUSI { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user's main associated high school.
        /// </summary>
        public int? HighSchoolId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user's main associated middle school.
        /// </summary>
        public int? MiddleSchoolId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user's main associated elementary school.
        /// </summary>
        public int? ElementarySchoolId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user's main associated student.
        /// </summary>
        public long? StudentUSI { get; set; }

        ///// <summary>
        ///// Gets or sets the user's LEA's code.
        ///// </summary>
        //public string LocalEducationAgency { get; set; }

        ///// <summary>
        ///// Gets or sets the user's LEA's id.
        ///// </summary>
        //public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Gets the School Id for the requested school type from the user profile (if defined), or if the type is 
        /// not specified, returns the first configured School Id (looking first at high school, then middle school
        /// and finally elementary school).
        /// </summary>
        /// <param name="schoolType">The specific type of school Id being requested.</param>
        /// <returns>The corresponding school Id if found; otherwise <b>null</b>.</returns>
        public int GetSchoolId(SchoolType schoolType = SchoolType.Unspecified)
        {
            switch (schoolType)
            {
                case SchoolType.ElementarySchool:
                    return ElementarySchoolId.Value;
                case SchoolType.MiddleSchool:
                    return MiddleSchoolId.Value;
                case SchoolType.HighSchool:
                    return HighSchoolId.Value;
                case SchoolType.Unspecified:
                    var currentSchoolType = ScenarioContext.Current.GetSchoolType();

                    if (currentSchoolType != SchoolType.Unspecified)
                        return GetSchoolId(currentSchoolType);

                    var schoolId = HighSchoolId ?? MiddleSchoolId ?? ElementarySchoolId;

                    if (schoolId == null)
                        throw new Exception("There is no school in context, nor is there one defined for the current user profile.");

                    return schoolId.Value;
                default:
                    throw new NotSupportedException(schoolType.ToString());
            }
        }
    }
}