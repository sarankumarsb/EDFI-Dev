// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Security;

namespace EdFi.Dashboards.Resources.Security.ClaimValidators
{
    public class ClaimValidatorRequest
    {
        public const string InvalidParameterErrorMessage = "Invalid Parameters:";
        public const string UnhandledParameterErrorMessageFormat = "Unhandled parameter key '{0}'.";
        public const string UnableToFindParameterErrorMessage = "Unable to find required parameter";

        public const string CohortParameterName = "staffcohortid";
        public const string SchoolParameterName = "schoolid";
        public const string SectionParameterName = "teachersectionid";
        public const string SectionOrCohortParameterName = "sectionorcohortid";
        public const string StaffParameterName = "staffusi";
        public const string StudentParameterName = "studentusi";
        public const string StudentListTypeParameterName = "studentListType";
        public const string LocalEducationAgencyParameterName = "localeducationagencyid";
        public const string MetricParameterName = "metricid";
        public const string CustomStudentListParameterName = "customstudentlistid";
        public const string CustomMetricsBasedWatchListParameterName = "metricbasedwatchlistid";

        public const string StudentListEnumSection = "section";
        public const string StudentListEnumCohort = "cohort";
        public const string StudentListEnumCustomStudentList = "customstudentlist";
        public const string StudentListEnumMetricsBasedWatchList = "metricsbasedwatchlist";
        public const string StudentListEnumAll = "all";
        public const string StudentListEnumNone = "none";

        public static readonly string LocalEducationAgencySignature = BuildSignatureKey(LocalEducationAgencyParameterName);
        public static readonly string LocalEducationAgencyMetricSignature = BuildSignatureKey(LocalEducationAgencyParameterName, MetricParameterName);
        public static readonly string LocalEducationAgencySchoolSignature = BuildSignatureKey(LocalEducationAgencyParameterName, SchoolParameterName);
        public static readonly string LocalEducationAgencySchoolStaffSignature = BuildSignatureKey(LocalEducationAgencyParameterName, SchoolParameterName, StaffParameterName);
        public static readonly string LocalEducationAgencyStaffSignature = BuildSignatureKey(LocalEducationAgencyParameterName, StaffParameterName);
        public static readonly string LocalEducationAgencySchoolStudentSignature = BuildSignatureKey(LocalEducationAgencyParameterName, SchoolParameterName, StudentParameterName);
        public static readonly string LocalEducationAgencyStaffStudentListSignature = BuildSignatureKey(LocalEducationAgencyParameterName, StaffParameterName, SectionOrCohortParameterName, StudentListTypeParameterName);
        public static readonly string LocalEducationAgencySchoolStaffCustomStudentListSignature = BuildSignatureKey(LocalEducationAgencyParameterName, SchoolParameterName, StaffParameterName, CustomStudentListParameterName);
        public static readonly string LocalEducationAgencySchoolStaffWatchList = BuildSignatureKey(LocalEducationAgencyParameterName, SchoolParameterName, StaffParameterName, CustomMetricsBasedWatchListParameterName);
        public static readonly string LocalEducationAgencySchoolStaffSectionStudentListSignature = BuildSignatureKey(LocalEducationAgencyParameterName, SchoolParameterName, StaffParameterName, SectionOrCohortParameterName, StudentListTypeParameterName);
        public static readonly string SchoolSignature = BuildSignatureKey(SchoolParameterName);
        public static readonly string SchoolCohortSignature = BuildSignatureKey(SchoolParameterName, CohortParameterName);
        public static readonly string SchoolSectionSignature = BuildSignatureKey(SchoolParameterName, SectionParameterName);
        public static readonly string SchoolStaffSignature = BuildSignatureKey(SchoolParameterName, StaffParameterName);
        public static readonly string SchoolStaffCohortSignature = BuildSignatureKey(SchoolParameterName, StaffParameterName, CohortParameterName);
        public static readonly string SchoolStaffSectionSignature = BuildSignatureKey(SchoolParameterName, StaffParameterName, SectionParameterName);
        public static readonly string SchoolStaffStudentListSignature = BuildSignatureKey(SchoolParameterName, StaffParameterName, SectionOrCohortParameterName, StudentListTypeParameterName);
        public static readonly string SchoolStudentSignature = BuildSignatureKey(SchoolParameterName, StudentParameterName);
        public static readonly string StaffSignature = BuildSignatureKey(StaffParameterName);
        public static readonly string StudentSignature = BuildSignatureKey(StudentParameterName);
        public static readonly string SchoolStudentMetricSignature = BuildSignatureKey(SchoolParameterName, StudentParameterName, MetricParameterName);

        public ClaimValidatorRequest()
        {
            Parameters = null;
        }

        public IEnumerable<ParameterInstance> Parameters { get; set; }

        public string GetParameterValueByName(string name)
        {
            var value = GetParameterByName(name).Value;
            var result = (null == value) ? null : value.ToString();
            return result;
        }

        public int? GetNullableIdByName(string name)
        {
            var result = (int?)GetParameterByName(name).Value;
            return result;
        }

        public long? GetNullableLongIdByName(string name)
        {
            var result = (long?)GetParameterByName(name).Value;
            return result;
        }

        public long GetCohortId()
        {
            var result = (long)GetParameterByName(CohortParameterName, SectionOrCohortParameterName).Value;
            return result;
        }

        public long GetSectionId()
        {
            var result = (long)GetParameterByName(SectionParameterName, SectionOrCohortParameterName).Value;
            return result;
        }

        public long GetCustomStudentListId()
        {
            var value = GetParameterByName(CustomStudentListParameterName, SectionOrCohortParameterName).Value;
            var result = value is int ? (int)value : (long)value;

            return result;
        }

        public long GetCustomMetricsBasedWatchListId()
        {
            var value = GetParameterByName(CustomMetricsBasedWatchListParameterName, SectionOrCohortParameterName).Value;
            var result = value is int ? (int) value : (long) value;

            return result;
        }

        public int GetLocalEducationAgencyId()
        {
            var result = (int)GetParameterByName(LocalEducationAgencyParameterName).Value;
            return result;
        }

        public int GetSchoolId()
        {
            var result = (int)GetParameterByName(SchoolParameterName).Value;
            return result;
        }

        public long GetStaffUSI()
        {
            var result = (long)GetParameterByName(StaffParameterName).Value;
            return result;
        }

        public long GetStudentUSI()
        {
            var result = (long)GetParameterByName(StudentParameterName).Value;
            return result;
        }

        public ParameterInstance GetParameterByName(params string[] possibleNames)
        {
            var names = possibleNames.Select(pn => pn.ToLower());
            var parameter =
                (from p in Parameters
                 where names.Contains(p.Name.ToLower())
                 select p)
                    .SingleOrDefault();

            // This should never happen.
            if (parameter == null)
            {
                var key = BuildSignatureKey();
                var allNames = String.Join("|", possibleNames);

                throw new SecurityAccessDeniedException(UnableToFindParameterErrorMessage + ": Key:" + key + " Required: " + allNames);
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

        public string BuildSignatureKey()
        {
            return BuildSignatureKey(Parameters);
        }

        public bool FindParameterByName(string name)
        {
            var parameter =
                (from p in Parameters
                 where p.Name.ToLower() == name.ToLower()
                 select p)
                    .SingleOrDefault();

            return (parameter != null);
        }
    }
}
