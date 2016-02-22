using System;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Models.Application;
using EdFi.Dashboards.Resources.Security.Common;
using log4net;

namespace EdFi.Dashboards.Resources.Application
{
    public class FeedbackRequest
    {
        public FeedbackRequest()
        {
            LocalEducationAgency = String.Empty;
        }

        [AuthenticationIgnore("Anyone can submit a feedback request")]
        public string LocalEducationAgency { get; set; }
        [AuthenticationIgnore("Anyone can submit a feedback request")]
        public string Name { get; set; }
        [AuthenticationIgnore("Anyone can submit a feedback request")]
        public string Email { get; set; }
        [AuthenticationIgnore("Anyone can submit a feedback request")]
        public string Subject { get; set; }
        [AuthenticationIgnore("Anyone can submit a feedback request")]
        public string Issue { get; set; }
        [AuthenticationIgnore("Anyone can submit a feedback request")]
        public string PhoneNumber { get; set; }
        [AuthenticationIgnore("Anyone can submit a feedback request")]
        public string Feedback { get; set; }
        [AuthenticationIgnore("Anyone can submit a feedback request")]
        public string StaffUSI { get; set; }
    }

    public interface IFeedbackService : IPostHandler<FeedbackRequest, FeedbackModel> { }

    public class FeedbackService : IFeedbackService
    {
        private static readonly ILog feedbackHandlerLog = LogManager.GetLogger("FeedbackHandler");
        private static readonly ILog feedbackLog = LogManager.GetLogger("FeedbackReporter");

        private const string FeatureRequestIssueType = "Feature Request";
        private const string UnableToLoginIssueType = "Unable to Login";

        public const string Project = "Project";
        public const string EmailSubject = "EmailSubject";
        public const string Assignee = "Assignee";
        public const string LocalEducationAgencyOrgId = "LocalEducationAgencyOrgId";
        public const string LocalEducationAgencyOrg = "LocalEducationAgency/org";
        public const string SecurityLevel = "security Level";
        public const string IssueType = "issueType";
        public const string SchoolName = "School/Location Name";
        public const string ContactName = "Contact Name";
        public const string ContactEmail = "Contact Email";
        public const string ContactPhone = "Contact Phone";
        public const string Url = "URL";
        public const string SystemEnvironment = "System Environment";
        public const string EmailTimestamp = "Email Timestamp";
        public const string UserAgent = "UserAgent";
        public const string UserHostName = "UserHostName";
        public const string IPAddress = "IPAddress";
        public const string Languages = "Languages";
        public const string Component = "Component";

        public const string HttpUserAgent = "HTTP_USER_AGENT";
        public const string RemoteHost = "REMOTE_HOST";
        public const string RemoteAddress = "REMOTE_ADDR";
        public const string HttpAcceptLanguage = "HTTP_ACCEPT_LANGUAGE";

        private readonly IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency> repository;
        private readonly IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgencySupport> supportRepository;

        public FeedbackService(IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency> repository, IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgencySupport> supportRepository)
        {
            this.repository = repository;
            this.supportRepository = supportRepository;
        }

        [NoCache]
        [AuthenticationIgnore("Anyone can submit a feedback request")]
        public FeedbackModel Post(FeedbackRequest request)
        {
            if (String.IsNullOrWhiteSpace(request.Feedback) && String.IsNullOrWhiteSpace(request.Subject))
                return new FeedbackModel();

            var config = IoC.Resolve<IConfigValueProvider>();
            string message = String.Format("***Feedback Form***{1}Subject: {0}{1}Name: {2}{1}Email: {3}{1}StaffUSI: {4}{1}Phone: {5}{1}Issue: {6}{1}Description/Feedback: {7}{1}***End Feedback Form***", request.Subject, Environment.NewLine, request.Name, request.Email, request.StaffUSI, request.PhoneNumber, request.Issue, request.Feedback);

            try
            {
                //For JIRA we need to pre-append this....
                ThreadContext.Properties[EmailSubject] = request.Subject;

                EdFi.Dashboards.Application.Data.Entities.LocalEducationAgencySupport localEducationAgencySupport = null;

                try
                {
                    if (!String.IsNullOrEmpty(request.LocalEducationAgency))
                        localEducationAgencySupport = (from lea in repository.GetAll()
                                                       join s in supportRepository.GetAll()
                                                           on lea.LocalEducationAgencyId equals s.LocalEducationAgencyId
                                                       where lea.Code == request.LocalEducationAgency
                                                       select s).SingleOrDefault();
                }
                catch (InvalidOperationException)
                {
                    // this is thrown if the LEA can't be determined
                }

                var projectName = string.Compare(FeatureRequestIssueType, request.Issue, StringComparison.InvariantCultureIgnoreCase) == 0 ? config.GetValue("jira.defaultFeatureRequestProjectName") :
                                    (localEducationAgencySupport != null) ? localEducationAgencySupport.ProjectName : config.GetValue("jira.defaultProjectName");

                //double check required parameter for JIRA ticket creation
                if (string.IsNullOrEmpty(projectName))
                {
                    if (string.IsNullOrEmpty(config.GetValue("jira.defaultProjectName")))
                    {
                        throw new EventLogInvalidDataException("No value is defined for jira.defaultProjectName in the web.config file");
                    }
                    projectName = config.GetValue("jira.defaultProjectName");
                }


                if (localEducationAgencySupport != null)
                {
                    ThreadContext.Properties[Project] = projectName;
                    ThreadContext.Properties[Assignee] = localEducationAgencySupport.TicketAssignee;
                    ThreadContext.Properties[IssueType] = request.Issue;
                    ThreadContext.Properties[SecurityLevel] = localEducationAgencySupport.TicketSecurityLevel;
                    ThreadContext.Properties[LocalEducationAgencyOrgId] = localEducationAgencySupport.LocalEducationAgencyId.ToString("D6");
                    ThreadContext.Properties[LocalEducationAgencyOrg] = localEducationAgencySupport.TicketOrgDescription;

                    // Set up default no user values
                    ThreadContext.Properties[SchoolName] = localEducationAgencySupport.MissingSchoolName;
                }
                else
                {
                    ThreadContext.Properties[Project] = projectName;
                    ThreadContext.Properties[Assignee] = config.GetValue("jira.defaultAssignee");
                    ThreadContext.Properties[IssueType] = request.Issue;
                    ThreadContext.Properties[SecurityLevel] = config.GetValue("jira.defaultSecurityLevel");
                    ThreadContext.Properties[LocalEducationAgencyOrg] = string.IsNullOrEmpty(request.LocalEducationAgency) ? "NoDistrictFound" : request.LocalEducationAgency;
                }

                if (String.Compare(UnableToLoginIssueType, request.Issue, StringComparison.InvariantCultureIgnoreCase) == 0)
                    ThreadContext.Properties[Component] = config.GetValue("jira.userAccessComponent");

                ThreadContext.Properties[ContactName] = request.Name;
                ThreadContext.Properties[ContactEmail] = request.Email;
                ThreadContext.Properties[ContactPhone] = request.PhoneNumber;

                //SetSchoolNameToContext(schoolId);

                var sb2 = new StringBuilder();
                sb2.Append(new XElement(UserAgent, HttpContext.Current.Request.ServerVariables[HttpUserAgent]));
                sb2.Append(new XElement(UserHostName, HttpContext.Current.Request.ServerVariables[RemoteHost]));
                sb2.Append(new XElement(IPAddress, HttpContext.Current.Request.ServerVariables[RemoteAddress]));
                sb2.Append(new XElement(Languages, HttpContext.Current.Request.ServerVariables[HttpAcceptLanguage]));

                ThreadContext.Properties[SystemEnvironment] = sb2.ToString();
                ThreadContext.Properties[EmailTimestamp] = DateTime.Now.ToString("dd/MMM/yy h:mm tt");

                feedbackLog.Error(message);
            }
            catch
            {
                if (!String.IsNullOrEmpty(message))
                    feedbackHandlerLog.DebugFormat("Exception while trying to save feedback:\n\n{0}", message);
                throw;
            }

            return new FeedbackModel();
        }
    }
}
