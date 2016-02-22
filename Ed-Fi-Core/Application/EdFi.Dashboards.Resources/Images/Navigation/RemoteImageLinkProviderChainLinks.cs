using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure;

namespace EdFi.Dashboards.Resources.Images.Navigation
{
    public class RemoteImageLinkProviderChainLinks
    {
    }
    public class StudentRemoteImageLinkProvider : ChainOfResponsibilityBase<IImageLinkProvider, ImageRequestBase, String>, IImageLinkProvider
    {
        private readonly IConfigValueProvider configValueProvider;
        private readonly ILocalEducationAgencyContextProvider localEducationAgencyContextProvider;

        private static readonly object remoteImagePathLockObject = new object();
        private static readonly object bucketCountLockObject = new object();
        private static string remoteImagePath = String.Empty;
        private static int bucketCount;
        private const string peoplePathFormat = "{0}/{1}";
        private const string personImageFormat = @"{0}/{1}/Images/People/{2}/{3}.jpg";

        public StudentRemoteImageLinkProvider(IConfigValueProvider configValueProvider, ILocalEducationAgencyContextProvider localEducationAgencyContextProvider, IImageLinkProvider next) : base(next)
        {
            this.configValueProvider = configValueProvider;
            this.localEducationAgencyContextProvider = localEducationAgencyContextProvider;
        }

        private int BucketCount
        {
            get
            {
                if (bucketCount == 0)
                {
                    lock (bucketCountLockObject)
                    {
                        bucketCount = Convert.ToInt32(configValueProvider.GetValue("PersistedRepositoryBucketCount"));
                    }
                }
                return bucketCount;
            }
        }

        private string RemoteImagePath
        {
            get
            {
                if (String.IsNullOrEmpty(remoteImagePath))
                {
                    lock (remoteImagePathLockObject)
                    {
                        remoteImagePath = configValueProvider.GetValue("RemoteImagePath");
                    }
                }
                return remoteImagePath;
            }
        }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as StudentSchoolImageRequest != null);
        }

        protected override string HandleRequest(ImageRequestBase request)
        {
            var studentRequest = request as StudentSchoolImageRequest;

            var imageName = GetImageName(studentRequest.StudentUSI);
            var displayTypeForProfile = string.IsNullOrEmpty(studentRequest.DisplayType) ? "Profile" : studentRequest.DisplayType.Trim();
            return String.Format(personImageFormat, RemoteImagePath, localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode(), displayTypeForProfile, imageName);
        }

        public string GetImageLink(ImageRequestBase request)
        {
            //Delegate to the base...
            return ProcessRequest(request);
        }

        private string GetImageName(long studentUSI)
        {
            var bucket = Math.Abs(studentUSI.GetHashCode()) % BucketCount;

            return String.Format(peoplePathFormat, bucket, studentUSI);
        }
    }

    public class StaffRemoteImageLinkProvider : ChainOfResponsibilityBase<IImageLinkProvider, ImageRequestBase, String>, IImageLinkProvider
    {
        private readonly IConfigValueProvider configValueProvider;
        private readonly ILocalEducationAgencyContextProvider localEducationAgencyContextProvider;

        private static readonly object remoteImagePathLockObject = new object();
        private static readonly object bucketCountLockObject = new object();
        private static string remoteImagePath = String.Empty;
        private static int bucketCount;
        private const string peoplePathFormat = "{0}/{1}";
        private const string personImageFormat = @"{0}/{1}/Images/People/{2}/{3}.jpg";

        public StaffRemoteImageLinkProvider(IConfigValueProvider configValueProvider, ILocalEducationAgencyContextProvider localEducationAgencyContextProvider, IImageLinkProvider next) : base(next)
        {
            this.configValueProvider = configValueProvider;
            this.localEducationAgencyContextProvider = localEducationAgencyContextProvider;
        }

        private int BucketCount
        {
            get
            {
                if (bucketCount == 0)
                {
                    lock (bucketCountLockObject)
                    {
                        bucketCount = Convert.ToInt32(configValueProvider.GetValue("PersistedRepositoryBucketCount"));
                    }
                }
                return bucketCount;
            }
        }

        private string RemoteImagePath
        {
            get
            {
                if (String.IsNullOrEmpty(remoteImagePath))
                {
                    lock (remoteImagePathLockObject)
                    {
                        remoteImagePath = configValueProvider.GetValue("RemoteImagePath");
                    }
                }
                return remoteImagePath;
            }
        }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as StaffSchoolImageRequest != null);
        }

        protected override string HandleRequest(ImageRequestBase request)
        {
            var staffRequest = request as StaffSchoolImageRequest;

            var imageName = GetImageName(staffRequest.StaffUSI);
            var displayTypeForProfile = string.IsNullOrEmpty(staffRequest.DisplayType) ? "Profile" : staffRequest.DisplayType.Trim();
            return String.Format(personImageFormat, RemoteImagePath, localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode(), displayTypeForProfile, imageName);
        }

        public string GetImageLink(ImageRequestBase request)
        {
            //Delegate to the base...
            return ProcessRequest(request);
        }

        private string GetImageName(long staffUSI)
        {
            var bucket = Math.Abs(staffUSI.GetHashCode()) % BucketCount;

            return String.Format(peoplePathFormat, bucket, staffUSI);
        }
    }

    public class SchoolRemoteImageLinkProvider : ChainOfResponsibilityBase<IImageLinkProvider, ImageRequestBase, String>, IImageLinkProvider
    {
        private readonly IConfigValueProvider configValueProvider;
        private readonly ILocalEducationAgencyContextProvider localEducationAgencyContextProvider;

        private static readonly object remoteImagePathLockObject = new object();
        private static string remoteImagePath = String.Empty;
        private const string educationOrganizationImageFormat = @"{0}/{1}/Images/EducationOrganization/{2}/{3}.jpg";

        public SchoolRemoteImageLinkProvider(IConfigValueProvider configValueProvider, ILocalEducationAgencyContextProvider localEducationAgencyContextProvider, IImageLinkProvider next) : base(next)
        {
            this.configValueProvider = configValueProvider;
            this.localEducationAgencyContextProvider = localEducationAgencyContextProvider;
        }
        
        private string RemoteImagePath
        {
            get
            {
                if (String.IsNullOrEmpty(remoteImagePath))
                {
                    lock (remoteImagePathLockObject)
                    {
                        remoteImagePath = configValueProvider.GetValue("RemoteImagePath");
                    }
                }
                return remoteImagePath;
            }
        }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as SchoolImageRequest != null);
        }

        protected override string HandleRequest(ImageRequestBase request)
        {
            var schoolRequest = request as SchoolImageRequest;

            var displayTypeForProfile = string.IsNullOrEmpty(schoolRequest.DisplayType) ? "Profile" : schoolRequest.DisplayType.Trim();
            return String.Format(educationOrganizationImageFormat, RemoteImagePath, localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode(), displayTypeForProfile, schoolRequest.SchoolId);
        }

        public string GetImageLink(ImageRequestBase request)
        {
            //Delegate to the base...
            return ProcessRequest(request);
        }
    }

    public class LocalEducationAgencyRemoteImageLinkProvider : ChainOfResponsibilityBase<IImageLinkProvider, ImageRequestBase, String>, IImageLinkProvider
    {
        private readonly IConfigValueProvider configValueProvider;
        private readonly ILocalEducationAgencyContextProvider localEducationAgencyContextProvider;

        private static readonly object remoteImagePathLockObject = new object();
        private static string remoteImagePath = String.Empty;
        private const string educationOrganizationImageFormat = @"{0}/{1}/Images/EducationOrganization/{2}/{3}.jpg";

        public LocalEducationAgencyRemoteImageLinkProvider(IConfigValueProvider configValueProvider, ILocalEducationAgencyContextProvider localEducationAgencyContextProvider, IImageLinkProvider next) : base(next)
        {
            this.configValueProvider = configValueProvider;
            this.localEducationAgencyContextProvider = localEducationAgencyContextProvider;
        }
        
        private string RemoteImagePath
        {
            get
            {
                if (String.IsNullOrEmpty(remoteImagePath))
                {
                    lock (remoteImagePathLockObject)
                    {
                        remoteImagePath = configValueProvider.GetValue("RemoteImagePath");
                    }
                }
                return remoteImagePath;
            }
        }

        protected override bool CanHandleRequest(ImageRequestBase request)
        {
            return (request as LocalEducationAgencyImageRequest != null);
        }

        protected override string HandleRequest(ImageRequestBase request)
        {
            var localEducationAgencyRequest = request as LocalEducationAgencyImageRequest;

            var displayTypeForProfile = string.IsNullOrEmpty(localEducationAgencyRequest.DisplayType) ? "Profile" : localEducationAgencyRequest.DisplayType.Trim();
            return String.Format(educationOrganizationImageFormat, RemoteImagePath, localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode(), displayTypeForProfile, localEducationAgencyRequest.LocalEducationAgencyId);
        }

        public string GetImageLink(ImageRequestBase request)
        {
            //Delegate to the base...
            return ProcessRequest(request);
        }
    }
}
