using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Resources.Navigation;

namespace EdFi.Dashboards.Resources.Images.Navigation
{
    public class GenericImageLinkProviderChainLinks
    {
    }

    public class StudentGenericImageLinkProvider : ChainOfResponsibilityBase<IImageLinkProvider, ImageRequestBase, String>, IImageLinkProvider
    {
        private const string defaultPersonImageFilePathFormat = "~/Core_Content/Images/Students/NoImage{0}.jpg";

        public StudentGenericImageLinkProvider(IImageLinkProvider next) : base(next) {}

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as StudentSchoolImageRequest != null);
        }

        protected override string HandleRequest(ImageRequestBase request)
        {
            var displayType = request.DisplayType ?? String.Empty;
            displayType = displayType.Trim();
            var defaultFilePath = string.Format(defaultPersonImageFilePathFormat, displayType);
            return defaultFilePath.Resolve();
        }

        public string GetImageLink(ImageRequestBase request)
        {
            //Delegate to the base...
            return ProcessRequest(request);
        }
    }

    public class StaffGenericImageLinkProvider : ChainOfResponsibilityBase<IImageLinkProvider, ImageRequestBase, String>, IImageLinkProvider
    {
        private const string defaultPersonImageFilePathFormat = "~/Core_Content/Images/Students/NoImage{0}.jpg";

        public StaffGenericImageLinkProvider(IImageLinkProvider next) : base(next) { }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as StaffSchoolImageRequest != null);
        }

        protected override string HandleRequest(ImageRequestBase request)
        {
            var displayType = request.DisplayType ?? String.Empty;
            displayType = displayType.Trim();
            var defaultFilePath = string.Format(defaultPersonImageFilePathFormat, displayType);
            return defaultFilePath.Resolve();
        }

        public string GetImageLink(ImageRequestBase request)
        {
            //Delegate to the base...
            return ProcessRequest(request);
        }
    }

    public class SchoolGenericImageLinkProvider : ChainOfResponsibilityBase<IImageLinkProvider, ImageRequestBase, String>, IImageLinkProvider
    {
        private const string defaultSchoolImageFilePathFormat = "~/Core_Content/Images/EducationOrganization/NoEducationOrganizationImage{0}.jpg";

        public SchoolGenericImageLinkProvider(IImageLinkProvider next) : base(next) { }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as SchoolImageRequest != null);
        }

        protected override string HandleRequest(ImageRequestBase request)
        {
            var displayType = request.DisplayType ?? String.Empty;
            displayType = displayType.Trim();
            var defaultFilePath = string.Format(defaultSchoolImageFilePathFormat, displayType);
            return defaultFilePath.Resolve();
        }

        public string GetImageLink(ImageRequestBase request)
        {
            //Delegate to the base...
            return ProcessRequest(request);
        }
    }

    public class LocalEducationAgencyGenericImageLinkProvider : ChainOfResponsibilityBase<IImageLinkProvider, ImageRequestBase, String>, IImageLinkProvider
    {
        private const string defaultSchoolImageFilePathFormat = "~/Core_Content/Images/EducationOrganization/NoEducationOrganizationImage{0}.jpg";

        public LocalEducationAgencyGenericImageLinkProvider(IImageLinkProvider next) : base(next) { }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as LocalEducationAgencyImageRequest != null);
        }

        protected override string HandleRequest(ImageRequestBase request)
        {
            var displayType = request.DisplayType ?? String.Empty;
            displayType = displayType.Trim();
            var defaultFilePath = string.Format(defaultSchoolImageFilePathFormat, displayType);
            return defaultFilePath.Resolve();
        }

        public string GetImageLink(ImageRequestBase request)
        {
            //Delegate to the base...
            return ProcessRequest(request);
        }
    }
}
