// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using EdFi.Dashboards.Presentation.Architecture;
using EdFi.Dashboards.Presentation.Core.Models.Shared;
using EdFi.Dashboards.Presentation.Web.Architecture;

namespace EdFi.Dashboards.Presentation.Web.Areas.LocalEducationAgency.Models.Home
{
    public class HomeModel : IPartiallySerializable<EdFi.Dashboards.Resources.Models.LocalEducationAgency.HomeModel>
    {
        public EdFi.Dashboards.Resources.Models.LocalEducationAgency.HomeModel LocalEducationAgencyInformation { get; set; }
        public FeedbackModel Feedback { get; set; }

        EdFi.Dashboards.Resources.Models.LocalEducationAgency.HomeModel IPartiallySerializable<EdFi.Dashboards.Resources.Models.LocalEducationAgency.HomeModel>.SerializableModel
        {
            get { return LocalEducationAgencyInformation; }
        }
    }
}