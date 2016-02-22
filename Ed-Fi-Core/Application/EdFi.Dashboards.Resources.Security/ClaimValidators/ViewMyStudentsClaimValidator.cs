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
    public abstract class ViewMyStudentsClaimValidator : ClaimValidatorBase
    {
        protected ViewMyStudentsClaimValidator(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            ClaimType = EdFiClaimTypes.ViewMyStudents;
        }
    }

    public class ViewMyStudentsClaimValidatorSchoolStaff : ViewMyStudentsClaimValidator
    {
        public ViewMyStudentsClaimValidatorSchoolStaff(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolStaffSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateCurrentUserIsStaff(request);
            ValidateClaimSchoolStaff(request, ClaimType);

            return null;
        }
    }

    public class ViewMyStudentsClaimValidatorSchoolStaffCohort : ViewMyStudentsClaimValidator
    {
        public ViewMyStudentsClaimValidatorSchoolStaffCohort(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolStaffCohortSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateCurrentUserIsStaff(request);
            ValidateClaimSchoolCohortStaff(request, ClaimType);

            return null;
        }
    }
    
    public class ViewMyStudentsClaimValidatorSchoolStaffSection : ViewMyStudentsClaimValidator
    {
        public ViewMyStudentsClaimValidatorSchoolStaffSection(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolStaffSectionSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateCurrentUserIsStaff(request);
            ValidateClaimSchoolStaffSection(request, ClaimType);

            return null;
        }
    }

    public class ViewMyStudentsClaimValidatorSchoolStaffStudentList : ViewMyStudentsClaimValidator
    {
        public ViewMyStudentsClaimValidatorSchoolStaffStudentList(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolStaffStudentListSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {

            ValidateCurrentUserIsStaff(request);

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

    public class ViewMyStudentsClaimValidatorSchoolStudent : ViewMyStudentsClaimValidator
    {
        public ViewMyStudentsClaimValidatorSchoolStudent(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolStudentSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimSchoolStudentByStudentListAssociation(request, ClaimType);

            return null;
        }
    }

    public class ViewMyStudentsClaimValidatorSchoolStudentMetric : ViewMyStudentsClaimValidator
    {
        public ViewMyStudentsClaimValidatorSchoolStudentMetric(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.SchoolStudentMetricSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimSchoolStudentByStudentListAssociation(request, ClaimType);

            return null;
        }
    }

    public class ViewMyStudentsClaimValidatorSchool : ViewMyStudentsClaimValidator
    {
        public ViewMyStudentsClaimValidatorSchool(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
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

    public class ViewMyStudentsClaimValidatorStudent : ViewMyStudentsClaimValidator
    {
        public ViewMyStudentsClaimValidatorStudent(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.StudentSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimStudentByCurrentUser(request, ClaimType);

            return null;
        }
    }

    /*
     * N.B.  The localeducationagencyid is implicit in the schoolid.  The HandleRequest is NOT
     * going to validate localeducationagencyid.
     */
    public class ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStudent : ViewMyStudentsClaimValidator
    {
        public ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStudent(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencySchoolStudentSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateClaimSchoolStudentByStudentListAssociation(request, ClaimType);

            return null;
        }
    }

    public class ViewMyStudentsClaimValidatorLocalEducationAgencyStaffStudentList : ViewMyStudentsClaimValidator
    {
        public ViewMyStudentsClaimValidatorLocalEducationAgencyStaffStudentList(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencyStaffStudentListSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateCurrentUserIsStaff(request);

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
                case ClaimValidatorRequest.StudentListEnumMetricsBasedWatchList:
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

    public class ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList : ViewMyStudentsClaimValidator
    {
        public ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffSectionStudentList(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencySchoolStaffSectionStudentListSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateCurrentUserIsStaff(request);

            var studentListType = request.GetParameterValueByName(ClaimValidatorRequest.StudentListTypeParameterName);
            var schoolId = request.GetNullableIdByName(ClaimValidatorRequest.SchoolParameterName);
            if (schoolId.HasValue)
                ValidateLocalEducationAgencySchoolIfPresent(request);

            // DJWhite 4 Jan 2012: Default null or empty to NONE which is equivalent to ALL.
            if (String.IsNullOrEmpty(studentListType)) studentListType = StudentListType.None.ToString();

            switch (studentListType.ToLower())
            {
                case ClaimValidatorRequest.StudentListEnumSection:
                    ValidateClaimSchoolStaffSection(request, ClaimType);
                    break;
                case ClaimValidatorRequest.StudentListEnumCohort:
                    ValidateClaimLocalEducationAgencyCohortStaff(request, ClaimType);
                    break;
                case ClaimValidatorRequest.StudentListEnumCustomStudentList:
                    if (schoolId.HasValue)
                        ValidateClaimSchoolCustomStudentListStaff(request, ClaimType);
                    else
                        ValidateClaimLocalEducationAgencyCustomStudentListStaff(request, ClaimType);
                    break;
                case ClaimValidatorRequest.StudentListEnumMetricsBasedWatchList:
                case ClaimValidatorRequest.StudentListEnumAll:
                case ClaimValidatorRequest.StudentListEnumNone:
                    var staffUSI = request.GetNullableLongIdByName(ClaimValidatorRequest.StaffParameterName);
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

    public class ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaff : ViewMyStudentsClaimValidator
    {
        public ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaff(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencySchoolStaffSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateCurrentUserIsStaff(request);

            var schoolId = request.GetNullableIdByName(ClaimValidatorRequest.SchoolParameterName);
            if (schoolId.HasValue)
                ValidateClaimSchoolStaff(request, ClaimType);
            else
                ValidateClaimLocalEducationAgencyStaff(request, ClaimType);

            return null;
        }
    }

    public class ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffCustomStudentList : ViewMyStudentsClaimValidator
    {
        public ViewMyStudentsClaimValidatorLocalEducationAgencySchoolStaffCustomStudentList(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
            base(securityAssertionProvider, next)
        {
            HandledSignatureKey = ClaimValidatorRequest.LocalEducationAgencySchoolStaffCustomStudentListSignature;
        }

        protected override object HandleRequest(ClaimValidatorRequest request)
        {
            ValidateCurrentUserIsStaff(request);

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

    public class ViewMyStudentsClaimValidatorLocalEducationAgency : ViewMyStudentsClaimValidator
    {
        public ViewMyStudentsClaimValidatorLocalEducationAgency(ISecurityAssertionProvider securityAssertionProvider, IClaimValidator next) :
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
}
