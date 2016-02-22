// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Resources.Security.Implementations;

namespace EdFi.Dashboards.Resources.Security.ClaimValidators
{
    public abstract class ViewAllStudentsClaimValidator : ClaimValidatorBase
    {
        protected ViewAllStudentsClaimValidator(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            ClaimType = EdFiClaimTypes.ViewAllStudents;
        }
    }

    public class ViewAllStudentsClaimValidatorLocalEducationAgency : ViewAllStudentsClaimValidator
    {
        public ViewAllStudentsClaimValidatorLocalEducationAgency(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencySignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimLocalEducationAgency(request, ClaimType);

            return null;
        }
    }

    public class ViewAllStudentsClaimValidatorSchool : ViewAllStudentsClaimValidator
    {
        public ViewAllStudentsClaimValidatorSchool(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimSchool(request, ClaimType);

            return null;
        }
    }

    public class ViewAllStudentsClaimValidatorSchoolCohort : ViewAllStudentsClaimValidator
    {
        public ViewAllStudentsClaimValidatorSchoolCohort(ISecurityAssertionProvider securityAssertionProvider,
                                                         IClaimValidator next) :
                                                             base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolCohortSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimSchoolCohort(request, ClaimType);

            return null;
        }
    }


    public class ViewAllStudentsClaimValidatorSchoolStaff : ViewAllStudentsClaimValidator
    {
        public ViewAllStudentsClaimValidatorSchoolStaff(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolStaffSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimSchoolStaff(request, ClaimType);

            return null;
        }
    }

    public class ViewAllStudentsClaimValidatorSchoolStaffCohort : ViewAllStudentsClaimValidator
    {
        public ViewAllStudentsClaimValidatorSchoolStaffCohort(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolStaffCohortSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimSchoolCohortStaff(request, ClaimType);

            return null;
        }
    }

    public class ViewAllStudentsClaimValidatorSchoolStaffSection : ViewAllStudentsClaimValidator
    {
        public ViewAllStudentsClaimValidatorSchoolStaffSection(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolStaffSectionSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimSchoolStaffSection(request, ClaimType);

            return null;
        }
    }

    public class ViewAllStudentsClaimValidatorSchoolStaffStudentList : ViewAllStudentsClaimValidator
    {
        public ViewAllStudentsClaimValidatorSchoolStaffStudentList(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolStaffStudentListSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            var studentListType = request.GetParameterValueByName(ClaimValidatorRequest.StudentListTypeParameterName);

            // DJWhite 4 Jan 2012: Default null or empty to NONE which is equivalent to ALL.
            if (String.IsNullOrEmpty(studentListType)) studentListType = StudentListType.None.ToString();

            switch (studentListType.ToLower())
            {
                case ClaimValidatorRequest.StudentListEnumSection:
                    ValidateClaimSchoolStaffSection(request, ClaimType);
                    break;
                case ClaimValidatorRequest.StudentListEnumCohort:
                    ValidateClaimSchoolCohortStaff(request, ClaimType);
                    break;
                case ClaimValidatorRequest.StudentListEnumCustomStudentList:
                    ValidateClaimSchoolCustomStudentListStaff(request, ClaimType);
                    break;
                case ClaimValidatorRequest.StudentListEnumMetricsBasedWatchList:
                case ClaimValidatorRequest.StudentListEnumAll:
                case ClaimValidatorRequest.StudentListEnumNone:
                    ValidateClaimSchoolStaff(request, ClaimType);
                    break;
                default:
                    throw new UserAccessDeniedException(ClaimValidatorRequest.InvalidParameterErrorMessage);
            }

            return null;
        }
    }

    public class ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList : ViewAllStudentsClaimValidator
    {
        public ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencySchoolStaffSectionStudentListSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            var studentListType = request.GetParameterValueByName(ClaimValidatorRequest.StudentListTypeParameterName);
            var schoolId = request.GetNullableIdByName(ClaimValidatorRequest.SchoolParameterName);
            var staffUSI = request.GetNullableLongIdByName(ClaimValidatorRequest.StaffParameterName);

            // DJWhite 4 Jan 2012: Default null or empty to NONE which is equivalent to ALL.
            if (String.IsNullOrEmpty(studentListType)) studentListType = StudentListType.None.ToString();

            switch (studentListType.ToLower())
            {
                case ClaimValidatorRequest.StudentListEnumSection:
                    if (!schoolId.HasValue)
                        throw new UserAccessDeniedException(ClaimValidatorRequest.InvalidParameterErrorMessage);

                    if (staffUSI.HasValue)
                        ValidateClaimSchoolStaffSection(request, ClaimType);
                    else
                        ValidateClaimSchoolSection(request, ClaimType);
                    break;
                case ClaimValidatorRequest.StudentListEnumCohort:
                    if (staffUSI.HasValue)
                    {
                        if (schoolId.HasValue)
                            ValidateClaimSchoolCohortStaff(request, ClaimType);
                        else
                            ValidateClaimLocalEducationAgencyCohortStaff(request, ClaimType);
                    }
                    else
                    {
                        if (schoolId.HasValue)
                            ValidateClaimSchoolCohort(request, ClaimType);
                        else
                            ValidateClaimLocalEducationAgencyCohort(request, ClaimType);
                    }
                    break;
                case ClaimValidatorRequest.StudentListEnumCustomStudentList:
                    ValidateClaimSchoolCustomStudentListStaff(request, ClaimType);
                    break;
                case ClaimValidatorRequest.StudentListEnumMetricsBasedWatchList:
                case ClaimValidatorRequest.StudentListEnumAll:
                case ClaimValidatorRequest.StudentListEnumNone:
                    if (staffUSI.HasValue)
                    {
                        if (schoolId.HasValue)
                            ValidateClaimSchoolStaff(request, ClaimType);
                        else
                            ValidateClaimLocalEducationAgencyStaff(request, ClaimType);
                    }
                    else
                    {
                        if (schoolId.HasValue)
                            ValidateClaimSchool(request, ClaimType);
                        else
                            ValidateClaimLocalEducationAgency(request, ClaimType);
                    }
                    break;
                default:
                    throw new UserAccessDeniedException(ClaimValidatorRequest.InvalidParameterErrorMessage);
            }

            return null;
        }
    }

    public class ViewAllStudentsClaimValidatorSchoolSection : ViewAllStudentsClaimValidator
    {
        public ViewAllStudentsClaimValidatorSchoolSection(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolSectionSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimSchoolSection(request, ClaimType);

            return null;
        }
    }

    public class ViewAllStudentsClaimValidatorSchoolStudent : ViewAllStudentsClaimValidator
    {
        public ViewAllStudentsClaimValidatorSchoolStudent(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolStudentSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimSchoolStudentByOrganizationAssociation(request, ClaimType);

            return null;
        }
    }

    public class ViewAllStudentsClaimValidatorSchoolStudentMetric : ViewAllStudentsClaimValidator
    {
        public ViewAllStudentsClaimValidatorSchoolStudentMetric(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolStudentMetricSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimSchoolStudentByOrganizationAssociation(request, ClaimType);

            return null;
        }
    }

    public class ViewAllStudentsClaimValidatorStaff : ViewAllStudentsClaimValidator
    {
        public ViewAllStudentsClaimValidatorStaff(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.StaffSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimStaff(request, ClaimType);

            return null;
        }
    }

    public class ViewAllStudentsClaimValidatorStudent : ViewAllStudentsClaimValidator
    {
        public ViewAllStudentsClaimValidatorStudent(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.StudentSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimStudentByOrganizationAssociation(request, ClaimType);

            return null;
        }
    }

    public class ViewAllStudentsClaimValidatorLocalEducationAgencyStaffStudentList : ViewAllStudentsClaimValidator
    {
        public ViewAllStudentsClaimValidatorLocalEducationAgencyStaffStudentList(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencyStaffStudentListSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            var studentListType = request.GetParameterValueByName(ClaimValidatorRequest.StudentListTypeParameterName);

            // DJWhite 4 Jan 2012: Default null or empty to NONE which is equivalent to ALL.
            if (String.IsNullOrEmpty(studentListType)) studentListType = StudentListType.None.ToString();

            switch (studentListType.ToLower())
            {
                case ClaimValidatorRequest.StudentListEnumCohort:
                    ValidateClaimLocalEducationAgencyCohortStaff(request, ClaimType);
                    break;
                case ClaimValidatorRequest.StudentListEnumCustomStudentList:
                    ValidateClaimLocalEducationAgencyCustomStudentListStaff(request, ClaimType);
                    break;
                case ClaimValidatorRequest.StudentListEnumAll:
                case ClaimValidatorRequest.StudentListEnumNone:
                    ValidateClaimLocalEducationAgencyStaff(request, ClaimType);
                    break;
                default:
                    throw new UserAccessDeniedException(ClaimValidatorRequest.InvalidParameterErrorMessage);
            }

            return null;
        }
    }

    public class ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaff : ViewAllStudentsClaimValidator
    {
        public ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaff(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencySchoolStaffSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            var staffUSI = request.GetNullableLongIdByName(ClaimValidatorRequest.StaffParameterName);
            if (staffUSI.HasValue)
            {
                var schoolId = request.GetNullableIdByName(ClaimValidatorRequest.SchoolParameterName);
                if (schoolId.HasValue)
                    ValidateClaimSchoolStaff(request, ClaimType);
                else
                    ValidateClaimLocalEducationAgencyStaff(request, ClaimType);
            }
            else
            {
                var schoolId = request.GetNullableIdByName(ClaimValidatorRequest.SchoolParameterName);
                if (schoolId.HasValue)
                    ValidateClaimSchool(request, ClaimType);
                else
                    ValidateClaimLocalEducationAgency(request, ClaimType);
            }
            return null;
        }
    }

    public class ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffCustomStudentList : ViewAllStudentsClaimValidator
    {
        public ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStaffCustomStudentList(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencySchoolStaffCustomStudentListSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            var schoolId = request.GetNullableIdByName(ClaimValidatorRequest.SchoolParameterName);
            if (schoolId.HasValue)
                ValidateClaimSchoolStaff(request, ClaimType);
            else
                ValidateClaimLocalEducationAgencyStaff(request, ClaimType);

            var customStudentListId = request.GetNullableIdByName(ClaimValidatorRequest.CustomStudentListParameterName);
            if (customStudentListId.HasValue && schoolId.HasValue)
            {
                try
                {
                    ValidateClaimSchoolCustomStudentListStaff(request, ClaimType);
                }
                catch (UserAccessDeniedException)
                {
                    // this lets a LEA level user modify a LEA custom student list at a school level
                    ValidateClaimLocalEducationAgencyStaff(request, ClaimType);
                    ValidateClaimLocalEducationAgencyCustomStudentListStaff(request, ClaimType);
                }
            }
            else if (customStudentListId.HasValue)
                ValidateClaimLocalEducationAgencyCustomStudentListStaff(request, ClaimType);

            return null;
        }
    }

    #region implicit localeducationagencyid validation.
    /*
     * N.B.  The localeducationagencyid is implicit in the schoolid.  The HandleRequest is NOT
     * going to validate localeducationagencyid.
     */
    public class ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStudent : ViewAllStudentsClaimValidator
    {
        public ViewAllStudentsClaimValidatorLocalEducationAgencySchoolStudent(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencySchoolStudentSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimSchoolStudentByOrganizationAssociation(request, ClaimType);

            return null;
        }
    }
    #endregion
}
