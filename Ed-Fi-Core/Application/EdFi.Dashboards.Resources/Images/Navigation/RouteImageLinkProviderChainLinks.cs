using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Resources.Images.Navigation
{
    public class RouteImageLinkProviderChainLinks
    {
    }

    public class StudentRouteImageLinkProvider : ChainOfResponsibilityBase<IImageLinkProvider, ImageRequestBase, String>, IImageLinkProvider
    {
        public IRouteValuesPreparer RouteValuesPreparer { get; set; }
        public IHttpRequestProvider HttpRequestProvider { get; set; }
        public IConfigValueProvider ConfigValueProvider { get; set; }

        public StudentRouteImageLinkProvider(IImageLinkProvider next) : base(next) {}

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as StudentSchoolImageRequest != null);
        }

        protected override string HandleRequest(ImageRequestBase request)
        {
            var studentRequest = request as StudentSchoolImageRequest;
            var studentSchoolLinks = new StudentSchool { RouteValuesPreparer = RouteValuesPreparer, HttpRequestProvider = HttpRequestProvider, ConfigValueProvider = ConfigValueProvider};

            return studentSchoolLinks.Image(studentRequest.SchoolId, studentRequest.StudentUSI, null, new {studentRequest.DisplayType, format = "image", studentRequest.Gender});
        }

        public string GetImageLink(ImageRequestBase request)
        {
            //Delegate to the base...
            return ProcessRequest(request);
        }

        private class StudentSchool : SiteAreaBase
        {
            public string Image(int schoolId, long studentUSI, string student, object additionalValues = null)
            {
                return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, studentUSI, student);
            }  
        }
    }

    public class StaffRouteImageLinkProvider : ChainOfResponsibilityBase<IImageLinkProvider, ImageRequestBase, String>, IImageLinkProvider
    {
        public IRouteValuesPreparer RouteValuesPreparer { get; set; }
        public IHttpRequestProvider HttpRequestProvider { get; set; }
        public IConfigValueProvider ConfigValueProvider { get; set; }

        public StaffRouteImageLinkProvider(IImageLinkProvider next) : base(next){}

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as StaffSchoolImageRequest != null);
        }

        protected override string HandleRequest(ImageRequestBase request)
        {
            var staffRequest = request as StaffSchoolImageRequest;
            var staffLinks = new Staff { RouteValuesPreparer = RouteValuesPreparer, HttpRequestProvider = HttpRequestProvider, ConfigValueProvider = ConfigValueProvider };

            return staffLinks.Image(staffRequest.SchoolId, staffRequest.StaffUSI, new { staffRequest.DisplayType, format = "image", staffRequest.Gender });
        }

        public string GetImageLink(ImageRequestBase request)
        {
            //Delegate to the base...
            return ProcessRequest(request);
        }

        private class Staff : SiteAreaBase
        {
            public string Image(int schoolId, long staffUSI, object additionalValues = null)
            {
                return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId, staffUSI);
            }
        }
    }

    public class SchoolRouteImageLinkProvider : ChainOfResponsibilityBase<IImageLinkProvider, ImageRequestBase, String>, IImageLinkProvider
    {
        public IRouteValuesPreparer RouteValuesPreparer { get; set; }
        public IHttpRequestProvider HttpRequestProvider { get; set; }
        public IConfigValueProvider ConfigValueProvider { get; set; }

        public SchoolRouteImageLinkProvider(IImageLinkProvider next) : base(next){}

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as SchoolImageRequest != null);
        }

        protected override string HandleRequest(ImageRequestBase request)
        {
            var schoolRequest = request as SchoolImageRequest;
            var schoolLinks = new School { RouteValuesPreparer = RouteValuesPreparer, HttpRequestProvider = HttpRequestProvider, ConfigValueProvider = ConfigValueProvider };

            return schoolLinks.Image(schoolRequest.SchoolId, new { schoolRequest.DisplayType, format = "image" });
        }

        public string GetImageLink(ImageRequestBase request)
        {
            //Delegate to the base...
            return ProcessRequest(request);
        }

        private class School : SiteAreaBase
        {
            public string Image(int schoolId, object additionalValues = null)
            {
                return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), schoolId);
            }
        }
    }

    public class LocalEducationAgencyRouteImageLinkProvider : ChainOfResponsibilityBase<IImageLinkProvider, ImageRequestBase, String>, IImageLinkProvider
    {
        public IRouteValuesPreparer RouteValuesPreparer { get; set; }
        public IHttpRequestProvider HttpRequestProvider { get; set; }
        public IConfigValueProvider ConfigValueProvider { get; set; }

        public LocalEducationAgencyRouteImageLinkProvider(IImageLinkProvider next) : base(next){}

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as LocalEducationAgencyImageRequest != null);
        }

        protected override string HandleRequest(ImageRequestBase request)
        {
            var localEducationAgencyRequest = request as LocalEducationAgencyImageRequest;
            var localEducationAgencyLinks = new LocalEducationAgency { RouteValuesPreparer = RouteValuesPreparer, HttpRequestProvider = HttpRequestProvider, ConfigValueProvider = ConfigValueProvider };

            return localEducationAgencyLinks.Image(localEducationAgencyRequest.LocalEducationAgencyId, new { localEducationAgencyRequest.DisplayType, format = "image" });
        }

        public string GetImageLink(ImageRequestBase request)
        {
            //Delegate to the base...
            return ProcessRequest(request);
        }

        private class LocalEducationAgency : SiteAreaBase
        {
            public string Image(int localEducationAgencyId, object additionalValues = null)
            {
                return BuildResourceUrl(additionalValues, MethodBase.GetCurrentMethod(), localEducationAgencyId);
            }
        }
    }
}
