using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Navigation.Support;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Staff
{
    public class DefaultSectionRequest
    {
        public int SchoolId { get; set; }
        public long StaffUSI { get; set; }
        [AuthenticationIgnore("Used to create the Url")]
        public string Staff { get; set; }
        public string StudentListType { get; set; }
        public long SectionOrCohortId { get; set; }
        [AuthenticationIgnore("Used to create the Url")]
        public StaffModel.ViewType ViewType { get; set; }
    }

    public class DefaultSectionService : StaffServiceBase, IService<DefaultSectionRequest, DefaultSectionModel>
    {
        private readonly IStaffAreaLinks staffAreaLinks;

        public DefaultSectionService(IStaffAreaLinks staffAreaLinks)
        {
            this.staffAreaLinks = staffAreaLinks;    
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllStudents, EdFiClaimTypes.ViewMyStudents)]
        public DefaultSectionModel Get(DefaultSectionRequest request)
        {
            var sectionOrCohortId = request.SectionOrCohortId;
            var studentListType = GetSection(request.StaffUSI, request.SchoolId, request.StudentListType, ref sectionOrCohortId);

            var url = string.Empty;

            //if student list type comes back as None, it means there is no section, cohort, or custom list
            if (studentListType != StudentListType.None)
            {
                switch (request.ViewType)
                {
                    case StaffModel.ViewType.GeneralOverview:
                        url = staffAreaLinks.GeneralOverview(request.SchoolId, request.StaffUSI, request.Staff, sectionOrCohortId, studentListType.ToString());
                        break;

                    case StaffModel.ViewType.PriorYear:
                        url = staffAreaLinks.PriorYear(request.SchoolId, request.StaffUSI, request.Staff, sectionOrCohortId, studentListType.ToString());
                        break;
                }
            }

            return new DefaultSectionModel
                       {
                           ListType = studentListType.ToString(),
                           Link = url
                       };
        }
    }

    public class DefaultSectionUserContextApplicator : IUserContextApplicator
    {
        private const string listContext = "listContext";
        private readonly IHttpRequestProvider httpRequestProvider;

        public DefaultSectionUserContextApplicator(IHttpRequestProvider httpRequestProvider)
        {
            this.httpRequestProvider = httpRequestProvider;
        }

        public Type ModelType
        {
            get { return typeof(DefaultSectionModel); }
        }

        public void ApplyUserContextToModel(object modelAsObject, object requestAsObject)
        {
            var model = modelAsObject as DefaultSectionModel;

            // Should never happen
            if (model == null)
                return;

            var requestListContext = httpRequestProvider.GetValue(listContext);

            // Skip processing if there's no list context to apply
            if (string.IsNullOrEmpty(requestListContext))
                return;

            requestListContext = listContext + "=" + requestListContext;

            // Add the list context to each link in the model
            model.Link = model.Link.AppendParameters(requestListContext);
        }
    }
}
