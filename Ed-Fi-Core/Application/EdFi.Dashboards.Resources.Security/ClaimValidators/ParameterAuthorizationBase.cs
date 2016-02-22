// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;
using System.Text;
using EdFi.Dashboards.Resources.Security.Implementations;

namespace EdFi.Dashboards.Resources.Security.ClaimValidators
{
    public class ParameterAuthorizationBase
    {
        public const string UnhandledParameterErrorMessageFormat = "Unhandled parameter key '{0}'.";
        public const string UnableToFindParameterErrorMessage = "Unable to find required parameter";
        public const string InvalidParameterErrorMessage = "Invalid Parameters:";

        protected const string CohortParameterName = "staffcohortid";
        protected const string SchoolParameterName = "schoolid";
        protected const string SectionParameterName = "teachersectionid";
        protected const string SectionOrCohortParameterName = "sectionorcohortid";
        protected const string StaffParameterName = "staffusi";
        protected const string StudentParameterName = "studentusi";
        protected const string StudentListTypeParameterName = "studentListType";
        protected const string LocalEducationAgencyParameterName = "localeducationagencyid";

        protected static readonly string LocalEducationAgencySignature = BuildSignatureKey(ClaimValidatorRequest.LocalEducationAgencyParameterName);
        protected static readonly string LocalEducationAgencySchool = BuildSignatureKey(ClaimValidatorRequest.LocalEducationAgencyParameterName, SchoolParameterName);
        protected static readonly string LocalEducationAgencySchoolStaff = BuildSignatureKey(ClaimValidatorRequest.LocalEducationAgencyParameterName, SchoolParameterName, StaffParameterName);
        protected static readonly string LocalEducationAgencySchoolStudentSignature = BuildSignatureKey(ClaimValidatorRequest.LocalEducationAgencyParameterName, SchoolParameterName, StudentParameterName);
        protected static readonly string SchoolSignature = BuildSignatureKey(SchoolParameterName);
        protected static readonly string SchoolCohortSignature = BuildSignatureKey(SchoolParameterName, CohortParameterName);
        protected static readonly string SchoolSectionSignature = BuildSignatureKey(SchoolParameterName, SectionParameterName);
        protected static readonly string SchoolStaffSignature = BuildSignatureKey(SchoolParameterName, StaffParameterName);
        protected static readonly string SchoolStaffCohortSignature = BuildSignatureKey(SchoolParameterName, StaffParameterName, CohortParameterName);
        protected static readonly string SchoolStaffSectionSignature = BuildSignatureKey(SchoolParameterName, StaffParameterName, SectionParameterName);
        protected static readonly string SchoolStaffStudentListSignature = BuildSignatureKey(SchoolParameterName, StaffParameterName, SectionOrCohortParameterName, StudentListTypeParameterName);
        protected static readonly string SchoolStudentSignature = BuildSignatureKey(SchoolParameterName, StudentParameterName);
        protected static readonly string StaffSignature = BuildSignatureKey(StaffParameterName);
        protected static readonly string StudentSignature = BuildSignatureKey(StudentParameterName);

        private readonly ISecurityAssertionProvider assertionProvider;
		protected ISecurityAssertionProvider AssertionProvider
		{
			get { return assertionProvider; }
		}

        public ParameterAuthorizationBase(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next)
        {
            assertionProvider = securityAssertionProvider;
        }


		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification="as per MSDN, protected methods should not be marked static due to their polymorphic behavior.")]
		protected bool FindParameterByName(IEnumerable<ParameterInstance> parameters, string name)
        {
            var parameter =
                (from p in parameters
                 where p.Name.ToLower() == name.ToLower()
                 select p)
                    .SingleOrDefault();

            return (parameter != null);
        }

        protected string GetParameterValueByName(IEnumerable<ParameterInstance> parameters, string name)
        {
            var value = GetParameterByName(parameters, name).Value;
            var result = (null == value) ? null : value.ToString();
            return result;
        }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification="as per MSDN, protected methods should not be marked static due to their polymorphic behavior.")]
		protected ParameterInstance GetParameterByName(IEnumerable<ParameterInstance> parameters, string name)
        {
            var parameter =
                (from p in parameters
                 where p.Name.ToLower() == name.ToLower()
                 select p)
                    .SingleOrDefault();

            // This should never happen.
            if (parameter == null)
            {
                var key = BuildSignatureKey(parameters);
                throw new SecurityAccessDeniedException(UnableToFindParameterErrorMessage + ": Key:" + key + " Required: " + name);
            }

            return parameter;
        }

        private static string BuildSignatureKeyInternal(IEnumerable<string> names)
        {
            return names == null ? "" : String.Join("|", names.OrderBy(n => n)).ToLower();
        }

        public static string BuildSignatureKey(params string[] names)
        {
            return BuildSignatureKeyInternal(names);
        }

        public static string BuildSignatureKey(IEnumerable<string> names)
        {
            return BuildSignatureKeyInternal(names);
        }

        public static string BuildSignatureKey(IEnumerable<ParameterInstance> parameters)
        {
            return parameters == null ? "" : BuildSignatureKeyInternal(from p in parameters select p.Name);
        }

        public static string BuildSignatureKey(ClaimValidatorRequest request)
        {
            return request == null ? "" : BuildSignatureKey(request.Parameters);
        }

        public bool HasNothingClaimType(IEnumerable<string> claimTypes)
        {
            var result = claimTypes.All(n => n.ToLower().EndsWith("nothing"));
            return result;
        }
    }
}
