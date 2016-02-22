using System;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Models.Application;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using log4net;

namespace EdFi.Dashboards.Resources.Application
{
    public class ErrorLoggingRequest
    {
        [AuthenticationIgnore("Anyone can log an exception")]
        public Exception Exception { get; set; }
        [AuthenticationIgnore("Anyone can log an exception")]
        public string UserName { get; set; }
        [AuthenticationIgnore("Anyone can log an exception")]
        public HttpRequestBase Request { get; set; }
    }

    public interface IErrorLoggingService : IPostHandler<ErrorLoggingRequest, string> { }

    public class ErrorLoggingService : IErrorLoggingService
    {
        private const string emailSubjectPrefix = "Ed-Fi Error Report: ";

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

        public const string HttpUserAgent = "HTTP_USER_AGENT";
        public const string RemoteHost = "REMOTE_HOST";
        public const string RemoteAddress = "REMOTE_ADDR";
        public const string HttpAcceptLanguage = "HTTP_ACCEPT_LANGUAGE";

        private readonly ILog errorLog = LogManager.GetLogger("ErrorReporter");
        private readonly ISessionStateProvider session;
        private readonly ILocalEducationAgencyContextProvider contextProvider;
        private readonly IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency> repository;
        private readonly IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgencySupport> supportRepository;
        private readonly IErrorAreaLinks errorAreaLinks;

        public ErrorLoggingService(ISessionStateProvider session, ILocalEducationAgencyContextProvider contextProvider, IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency> repository, IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgencySupport> supportRepository, IErrorAreaLinks errorAreaLinks)
        {
            this.session = session;
            this.contextProvider = contextProvider;
            this.repository = repository;
            this.supportRepository = supportRepository;
            this.errorAreaLinks = errorAreaLinks;
        }

        [NoCache]
        [AuthenticationIgnore("Anyone can log an exception")]
        public string Post(ErrorLoggingRequest request)
        {
            var ex = request.Exception;
            var name = String.Empty;
            var email = String.Empty;
            var phone = String.Empty;

            while (ex != null && (ex is HttpUnhandledException || ex is TargetInvocationException || (ex is HttpException && ex.Message.Contains("System.Web.Mvc.HttpHandlerUtil+ServerExecuteHttpHandlerAsyncWrapper"))))
                ex = ex.InnerException;

            // Save exception to Session state for display on error page after redirect.
            var exceptionTransporter = new ExceptionModel(ex);
            session[EdFiApp.Session.LastException] = exceptionTransporter;

            var userInfo = UserInformation.Current;

            if (userInfo != null)
            {
                name = userInfo.FullName;
                email = userInfo.EmailAddress;
            }

            var exceptionStr = (ex != null) ? ex.ToString() : "Exception was null.";
            if (exceptionStr.Length > 4000)
                exceptionStr = exceptionStr.Substring(0, 4000) + Environment.NewLine + "<Stack trace trimmed>";

            ////Check if there is a password in the form and remove it because it will go through as plain text when logged.
            var form = RemovePasswordFromUrlEncodedFormData(request.Request.Form.ToString());

            if (String.IsNullOrWhiteSpace(form) && ex.Data.Contains("JSON"))
                form = ex.Data["JSON"] as string;
            if (form.Length > 4000)
                form = form.Substring(0, 4000) + "<Form data trimmed>";

            var sb = new StringBuilder();
            sb.AppendFormat("MESSAGE: {1} {0}SOURCE: {2} {0}FORM: {3} {0}URL: {4} {0}UserName: {5} {0}QUERYSTRING: {6} {0}TARGETSITE: {7} {0}EXCEPTION: {8} {0}",
                                Environment.NewLine,
                                ex.Message,
                                ex.Source,
                                form,
                                request.Request.Url,
                                request.UserName,
                                request.Request.QueryString,
                                ex.TargetSite,
                                exceptionStr);

            sb.AppendLine();
            sb.AppendLine();
            sb.AppendLine("***ServerVariables:***");
            //Lets add more important things to the error log...
            //Note: Don't change to linq or foreach because the Request.ServerVariables collection gets modified and you might end up with an error: Collection was modified during the enumeration.
            for (var i = 0; i < request.Request.ServerVariables.Keys.Count; i++)
            {
                if (request.Request.ServerVariables.Keys[i].Equals("ALL_HTTP", StringComparison.InvariantCultureIgnoreCase) || request.Request.ServerVariables.Keys[i].Equals("ALL_RAW", StringComparison.InvariantCultureIgnoreCase) || request.Request.ServerVariables.Keys[i].Equals("AUTH_PASSWORD", StringComparison.InvariantCultureIgnoreCase))
                    continue;

                var serverVariable = request.Request.ServerVariables[i];
                if (request.Request.ServerVariables.Keys[i].Equals("HTTP_COOKIE", StringComparison.InvariantCultureIgnoreCase))
                {
                    var removeFedAuthCookie = new Regex("FedAuth(\\w|\\+|/|=)*;");
                    serverVariable = removeFedAuthCookie.Replace(serverVariable, String.Empty);
                }
                sb.AppendFormat("{0}:{1}", request.Request.ServerVariables.Keys[i], serverVariable);
                sb.AppendLine();
            }

            if (request.Request.HttpMethod == "POST")
            {
                sb.AppendLine("***Content***");
                request.Request.InputStream.Position = 0;
                try
                {
                    using (var inputStream = new StreamReader(request.Request.InputStream))
                    {
                        var s = inputStream.ReadToEnd();
                        if (s.Length > 1000)
                        {
                            s = s.Substring(0, 1000) + Environment.NewLine + "<Content trimmed>";
                        }
                        ////Check if there is a password in the form and remove it because it will go through as plain text when logged.
                        s = RemovePasswordFromUrlEncodedFormData(s);
                        sb.AppendLine(s);
                    }
                }
                catch
                {
                    sb.AppendLine("***error reading content***");
                }
            }

            ThreadContext.Properties[Url] = request.Request.Url;
            //For JIRA we need to pre-append this....
            ThreadContext.Properties[EmailSubject] = emailSubjectPrefix + ex.Message;

            EdFi.Dashboards.Application.Data.Entities.LocalEducationAgencySupport localEducationAgencySupport = null;
            string localEducationAgencyCode = String.Empty;
            try
            {
                localEducationAgencyCode = contextProvider.GetCurrentLocalEducationAgencyCode();
                if (!String.IsNullOrEmpty(localEducationAgencyCode))
                    localEducationAgencySupport = (from lea in repository.GetAll()
                                                   join s in supportRepository.GetAll()
                                                   on lea.LocalEducationAgencyId equals s.LocalEducationAgencyId
                                                   where lea.Code == localEducationAgencyCode
                                                   select s).SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                // this is thrown if the LEA can't be determined
            }

            if (localEducationAgencySupport != null)
            {
                ThreadContext.Properties[Project] = localEducationAgencySupport.ProjectName;
                ThreadContext.Properties[Assignee] = localEducationAgencySupport.TicketAssignee;
                ThreadContext.Properties[SecurityLevel] = localEducationAgencySupport.TicketSecurityLevel;
                ThreadContext.Properties[IssueType] = localEducationAgencySupport.IssueType;
                ThreadContext.Properties[LocalEducationAgencyOrgId] = localEducationAgencySupport.LocalEducationAgencyId.ToString("D6");
                ThreadContext.Properties[LocalEducationAgencyOrg] = localEducationAgencySupport.TicketOrgDescription;

                // Set up default no user values
                ThreadContext.Properties[SchoolName] = localEducationAgencySupport.MissingSchoolName;
            }
            else
            {
                var config = IoC.Resolve<IConfigValueProvider>();
                ThreadContext.Properties[Project] = config.GetValue("jira.defaultProjectName");
                ThreadContext.Properties[Assignee] = config.GetValue("jira.defaultAssignee");
                ThreadContext.Properties[IssueType] = config.GetValue("jira.defaultIssueType");
                ThreadContext.Properties[SecurityLevel] = config.GetValue("jira.defaultSecurityLevel");
            }

            ThreadContext.Properties[ContactName] = name;
            ThreadContext.Properties[ContactEmail] = email;
            ThreadContext.Properties[ContactPhone] = phone;

            //SetSchoolNameToContext(schoolId);

            var sb2 = new StringBuilder();
            sb2.Append(new XElement(UserAgent, HttpContext.Current.Request.ServerVariables[HttpUserAgent]));
            sb2.Append(new XElement(UserHostName, HttpContext.Current.Request.ServerVariables[RemoteHost]));
            sb2.Append(new XElement(IPAddress, HttpContext.Current.Request.ServerVariables[RemoteAddress]));
            sb2.Append(new XElement(Languages, HttpContext.Current.Request.ServerVariables[HttpAcceptLanguage]));

            ThreadContext.Properties[SystemEnvironment] = sb2.ToString();
            ThreadContext.Properties[EmailTimestamp] = DateTime.Now.ToString("dd/MMM/yy h:mm tt");

            errorLog.Error(sb.ToString());

            return errorAreaLinks.ErrorPage(localEducationAgencyCode);
        }

        private static string RemovePasswordFromUrlEncodedFormData(string inputString)
        {
            // Match any URL encoded parameter with "password" in the name
            const string passwordPattern = @"((&[\w]+)?password[^&]+)"; 

            // Replace all occurrences of parameters with "password" in the name
            string result = Regex.Replace(inputString, passwordPattern, string.Empty, RegexOptions.IgnoreCase);

            return result;
        }
    }
 }