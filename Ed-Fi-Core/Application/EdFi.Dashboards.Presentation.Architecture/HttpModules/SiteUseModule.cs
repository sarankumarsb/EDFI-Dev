// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.IO;
using System.Web;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Infrastructure;
using log4net;

namespace EdFi.Dashboards.Presentation.Architecture.HttpModules
{
    public class SiteUseModule : IHttpModule
    {
        private readonly static ILog siteUseLog = LogManager.GetLogger("SiteUseLogger");

        public void Init(HttpApplication context)
        {
            context.PostRequestHandlerExecute += LogPageView;
        }


        public void Dispose()
        {
        }

        private void LogPageView(object source, EventArgs e)
        {
            var application = (HttpApplication)source;
            var context = application.Context;
            if (String.Compare(Path.GetExtension(context.Request.FilePath), ".aspx", true) != 0)
                return;

            var userName = String.Empty;
            var currentUser = System.Threading.Thread.CurrentPrincipal;
            if (currentUser != null)
                userName = currentUser.Identity.Name;
            var page = IoC.Resolve<ICurrentUrlProvider>().Url;
            siteUseLog.Info(String.Format("{0} viewed page {1}.", userName, page));
        }

    }
}
