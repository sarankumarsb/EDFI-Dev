// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Reflection;
using EdFi.Dashboards.Resources.Images;
using EdFi.Dashboards.Resources.Images.Navigation;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Staff;

namespace EdFi.Dashboards.Resources.Navigation.Mvc.Areas
{
    public class Staff : SiteAreaBase, IStaffAreaLinks
    {
        private readonly IImageLinkProvider imageLinkProvider;
        private readonly IClassroomViewProvider _classroomViewProvider;
        private readonly ICodeIdProvider _codeIdProvider;
        private readonly ILocalEducationAgencyContextProvider _localEducationAgencyContextProvider;

        public Staff(IClassroomViewProvider classroomViewProvider, ICodeIdProvider codeIdProvider, ILocalEducationAgencyContextProvider localEducationAgencyContextProvider, IImageLinkProvider imageLinkProvider)
        {
            _classroomViewProvider = classroomViewProvider;
            _codeIdProvider = codeIdProvider;
            _localEducationAgencyContextProvider = localEducationAgencyContextProvider;
            this.imageLinkProvider = imageLinkProvider;
        }

        public virtual string Default(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            var leaId = _codeIdProvider.Get(_localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode());
            var defaultClassroomView = _classroomViewProvider.GetDefaultClassroomView(leaId);

            StaffModel.ViewType result;
            if (StaffModel.ViewType.TryParse(defaultClassroomView, false, out result))
            {
                switch (result)
                {
                    case StaffModel.ViewType.PriorYear:
                        return PriorYear(schoolId, staffUSI, staff, sectionOrCohortId, studentListType, additionalValues);

                    case StaffModel.ViewType.AssessmentDetails:
                        return AssessmentDetails(schoolId, staffUSI, staff, sectionOrCohortId, studentListType, null, null, additionalValues);
                }
            }

            return GeneralOverview(schoolId, staffUSI, staff, sectionOrCohortId, studentListType, additionalValues);
        }

        public virtual string GeneralOverview(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, staffUSI, staff, sectionOrCohortId, studentListType);
        }

        public virtual string PriorYear(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, staffUSI, staff, sectionOrCohortId, studentListType);
        }

        public virtual string SubjectSpecificOverview(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, staffUSI, staff, sectionOrCohortId, studentListType);
        }

        public virtual string AssessmentDetails(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, string subjectArea = null, string assessmentSubType = null, object additionalValues = null)
        {
            if (String.IsNullOrWhiteSpace(subjectArea))
                return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, staffUSI, staff, sectionOrCohortId, studentListType, subjectArea, assessmentSubType);

            return BuildResourceUrl(new { SubjectArea = subjectArea }, MethodBase.GetCurrentMethod(), schoolId, staffUSI, staff, sectionOrCohortId, studentListType, subjectArea, assessmentSubType);
        }

        public virtual string ProfileThumbnail(int schoolId, long staffUSI, string gender)
        {
            return imageLinkProvider.GetImageLink(new StaffSchoolImageRequest { SchoolId = schoolId, StaffUSI = staffUSI, Gender = gender, DisplayType = "Thumb" });
        }

        public virtual string Image(int schoolId, long staffUSI, string gender)
        {
            return imageLinkProvider.GetImageLink(new StaffSchoolImageRequest { SchoolId = schoolId, StaffUSI = staffUSI, Gender = gender });
        }

        public virtual string ListImage(int schoolId, long staffUSI, string gender)
        {
            return imageLinkProvider.GetImageLink(new StaffSchoolImageRequest { SchoolId = schoolId, StaffUSI = staffUSI, Gender = gender, DisplayType = "List" });
        }

        public virtual string ExportAllMetrics(int schoolId, long staffUSI, string staff = null, long? sectionOrCohortId = null, string studentListType = null, string subjectArea = null, string assessmentSubType = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, staffUSI, staff, sectionOrCohortId, studentListType, subjectArea, assessmentSubType);
        }

        public virtual string Resource(int schoolId, long staffUSI, string resourceName, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, resourceName, MethodBase.GetCurrentMethod(), schoolId, staffUSI);
        }
        
        public string CustomStudentList(int schoolId, long staffUSI, string staff = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, staffUSI, staff);
        }

        public string CustomMetricsBasedWatchList(int schoolId, long staffUSI, string resourceName, int? sectionOrCohortId = null, string staff = null, string studentListType = null, object additionalValues = null)
        {
            return BuildResourceUrl(additionalValues, resourceName, MethodBase.GetCurrentMethod(), schoolId, staffUSI, resourceName,
                sectionOrCohortId, staff, studentListType);
        }

        public string LocalEducationAgencyCustomStudentList(int localEducationAgencyId, long staffUSI, string staff = null, object additionalValues = null)
        {
            return BuildUrl("Staff_LocalEducationAgency_Resources", "CustomStudentList", additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId, staffUSI, staff);
        }
    }
}
