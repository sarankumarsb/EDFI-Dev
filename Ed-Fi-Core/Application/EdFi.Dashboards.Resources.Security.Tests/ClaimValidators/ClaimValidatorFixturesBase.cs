// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Testing;
using Microsoft.IdentityModel.Claims;

namespace EdFi.Dashboards.Resources.Security.Tests.ClaimValidators
{
    /// <summary>
    /// Summary description for ClaimsFixturesBase
    /// </summary>
    public abstract class ClaimValidatorFixturesBase : TestFixtureBase
    {
        public const int LOCALEDUCATIONAGENCY_ID_00 = 100;
        public const int LOCALEDUCATIONAGENCY_ID_01 = 200;
        public const int LOCALEDUCATIONAGENCY_ID_02 = 300;

        protected const string LOCALEDUCATIONAGENCY_NAME_01 = "Local Education Agency 01";
        protected const string LOCALEDUCATIONAGENCY_NAME_02 = "Local Education Agency 02";

        protected const string SCH_NAME_01_01 = "LEA 01 School 01";
        protected const string SCH_NAME_01_02 = "LEA 01 School 02";
        protected const string SCH_NAME_02_01 = "LEA 02 School 01";
        protected const string SCH_NAME_02_02 = "LEA 02 School 02";

        public const int SCH_ID_00_00 = (LOCALEDUCATIONAGENCY_ID_00 * 1000) + 0;
        public const int SCH_ID_00_01 = (LOCALEDUCATIONAGENCY_ID_00 * 1000) + 1;
        public const int SCH_ID_00_02 = (LOCALEDUCATIONAGENCY_ID_00 * 1000) + 2;
        public const int SCH_ID_01_01 = (LOCALEDUCATIONAGENCY_ID_01 * 1000) + 1;
        public const int SCH_ID_01_02 = (LOCALEDUCATIONAGENCY_ID_01 * 1000) + 2;
        public const int SCH_ID_02_01 = (LOCALEDUCATIONAGENCY_ID_02 * 1000) + 1;
        public const int SCH_ID_02_02 = (LOCALEDUCATIONAGENCY_ID_02 * 1000) + 2;


        protected const int SECTION_OR_COHORT_ID_INVALID = 0;
        
        protected static EducationOrganizationIdentifier EDU_ORG_ID_1_0 = CreateEducationOrganizationIdentifiers(LOCALEDUCATIONAGENCY_ID_01, null);
        protected static EducationOrganizationIdentifier EDU_ORG_ID_1_1 = CreateEducationOrganizationIdentifiers(LOCALEDUCATIONAGENCY_ID_01, SCH_ID_01_01);
        protected static EducationOrganizationIdentifier EDU_ORG_ID_0_1 = CreateEducationOrganizationIdentifiers(null, SCH_ID_01_01);

        protected static Claim CLAIM_ACCESS_ORGANIZATION_1_0 = ClaimHelper.CreateClaim(EdFiClaimTypes.AccessOrganization, EDU_ORG_ID_1_0);
        protected static Claim CLAIM_ACCESS_ORGANIZATION_1_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.AccessOrganization, EDU_ORG_ID_1_1);
        protected static Claim CLAIM_ACCESS_ORGANIZATION_0_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.AccessOrganization, EDU_ORG_ID_0_1);

        protected static Claim CLAIM_ADMIN_1_0 = ClaimHelper.CreateClaim(EdFiClaimTypes.AdministerDashboard, EDU_ORG_ID_1_0);
        protected static Claim CLAIM_ADMIN_1_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.AdministerDashboard, EDU_ORG_ID_1_1);
        protected static Claim CLAIM_ADMIN_0_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.AdministerDashboard, EDU_ORG_ID_0_1);

        protected static Claim CLAIM_MANAGE_GOALS_1_0 = ClaimHelper.CreateClaim(EdFiClaimTypes.ManageGoals, EDU_ORG_ID_1_0);
        protected static Claim CLAIM_MANAGE_GOALS_1_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.ManageGoals, EDU_ORG_ID_1_1);
        protected static Claim CLAIM_MANAGE_GOALS_0_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.ManageGoals, EDU_ORG_ID_0_1);

        protected static Claim CLAIM_ALL_STUDENTS_1_0 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllStudents, EDU_ORG_ID_1_0);
        protected static Claim CLAIM_ALL_STUDENTS_1_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllStudents, EDU_ORG_ID_1_1);
        protected static Claim CLAIM_ALL_STUDENTS_0_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllStudents, EDU_ORG_ID_0_1);

        protected static Claim CLAIM_ALL_METRICS_1_0 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllMetrics, EDU_ORG_ID_1_0);
        protected static Claim CLAIM_ALL_METRICS_1_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllMetrics, EDU_ORG_ID_1_1);
        protected static Claim CLAIM_ALL_METRICS_0_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllMetrics, EDU_ORG_ID_0_1);

        protected static Claim CLAIM_MY_METRICS_1_0 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewMyMetrics, EDU_ORG_ID_1_0);
        protected static Claim CLAIM_MY_METRICS_1_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewMyMetrics, EDU_ORG_ID_1_1);
        protected static Claim CLAIM_MY_METRICS_0_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewMyMetrics, EDU_ORG_ID_0_1);

        protected static Claim CLAIM_MY_STUDENTS_1_0 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewMyStudents, EDU_ORG_ID_1_0);
        protected static Claim CLAIM_MY_STUDENTS_1_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewMyStudents, EDU_ORG_ID_1_1);
        protected static Claim CLAIM_MY_STUDENTS_0_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewMyStudents, EDU_ORG_ID_0_1);

        protected static Claim CLAIM_OPERATIONS_1_0 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewOperationalDashboard, EDU_ORG_ID_1_0);
        protected static Claim CLAIM_OPERATIONS_1_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewOperationalDashboard, EDU_ORG_ID_1_1);
        protected static Claim CLAIM_OPERATIONS_0_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewOperationalDashboard, EDU_ORG_ID_0_1);

        protected static Claim CLAIM_TEACHERS_1_0 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllTeachers, EDU_ORG_ID_1_0);
        protected static Claim CLAIM_TEACHERS_1_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllTeachers, EDU_ORG_ID_1_1);
        protected static Claim CLAIM_TEACHERS_0_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.ViewAllTeachers, EDU_ORG_ID_0_1);

        protected static Claim CLAIM_WATCH_LIST_1_0 = ClaimHelper.CreateClaim(EdFiClaimTypes.ManageWatchList, EDU_ORG_ID_1_0);
        protected static Claim CLAIM_WATCH_LIST_1_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.ManageWatchList, EDU_ORG_ID_1_1);
        protected static Claim CLAIM_WATCH_LIST_0_1 = ClaimHelper.CreateClaim(EdFiClaimTypes.ManageWatchList, EDU_ORG_ID_0_1);

        protected static EducationOrganizationIdentifier CreateEducationOrganizationIdentifiers(int? leaId, int? schoolId)
        {
            var result = new EducationOrganizationIdentifier {LocalEducationAgencyId = leaId, SchoolId = schoolId};

            return result;
        }

        protected const long TEACHER_USI_00 = 7000;
        protected const long TEACHER_USI_01 = 7001;
        protected const long TEACHER_USI_02 = 7002;

        protected const long COHORT_00_01 = 10001;
        protected const long COHORT_00_02 = 10002;
        protected const long COHORT_01_01 = 10101;
        protected const long COHORT_01_02 = 10202;

        protected const int CUSTOMSTUDENTLIST_00_01 = 10001;
        protected const int CUSTOMSTUDENTLIST_00_02 = 10002;
        protected const int CUSTOMSTUDENTLIST_01_01 = 10101;
        protected const int CUSTOMSTUDENTLIST_01_02 = 10202;

        protected const int METRICSBASEDWATCHLIST_01_01 = 10101;
        protected const int METRICSBASEDWATCHLIST_01_02 = 10202;

        protected const string StudentListTypeCustomStudentList = "CustomStudentList";
        protected const string StudentListTypeCohort = "Cohort";

        protected const long SECTION_00_01 = 20001;
        protected const long SECTION_00_02 = 20002;
        protected const long SECTION_01_01 = 20101;
        protected const long SECTION_01_02 = 20102;

        public const long STUDENT_00_01 = 30001;
        public const long STUDENT_00_02 = 30002;
        public const long STUDENT_01_01 = 30101;
        public const long STUDENT_01_02 = 30102;

        public const string CURRENT_USER_HAS_SCHOOL_LEVEL_CLAIMS_NAME = "Current User Has School Claims";
        public const string CURRENT_USER_HAS_LOCALEDUCATIONAGENCY_LEVEL_CLAIMS_NAME = "Current User Has LEA Claims";
        public const string CURRENT_USER_HAS_NO_CLAIMS_NAME = "Current User Has No Claims";

        // DJWhite, 30 Nov 2011.
        // The current user USI is only relevant in the context of ViewMyStudents,
        // where the current user USI must match the provided staff usi.
        // Therefore use TEACHER_USI_01 as the value of the USI
        //
        public const long CURRENT_USER_HAS_SCHOOL_LEVEL_CLAIMS_USI = TEACHER_USI_01;
        public const long CURRENT_USER_HAS_LOCALEDUCATIONAGENCY_LEVEL_CLAIMS_USI = TEACHER_USI_01;
        public const long CURRENT_USER_HAS_NO_CLAIMS_USI = TEACHER_USI_01;
        public const long CURRENT_USER_INVALID = TEACHER_USI_00;
    }
}
