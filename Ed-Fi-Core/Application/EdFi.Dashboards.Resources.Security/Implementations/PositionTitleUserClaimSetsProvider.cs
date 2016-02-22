// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using log4net;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    public class PositionTitleUserClaimSetsProvider : ChainOfResponsibilityUserClaimSetsProviderBase<ClaimsSet, EdFiUserSecurityDetails>
    {
        public const string InvalidMappingFormat = @"Invalid mapping: '{0}'.";
        public const string InvalidSystemClaimSetFormat = @"Invalid system claim set: '{0}'.";
        public const string None = "NONE";
        public const string ClaimSetMappingKeyFormat = "ClaimSetMapping{0}";
        //public const string RoleMappingLocalEducationAgencyName = "LocalEducationAgencyCode";

        //private const string claimSetMappingPathFormat = @"~/Files/ClaimSetMapping/{0}.csv";

        private readonly IFile file;
        private readonly ICacheProvider cacheProvider;
        private readonly ILocalEducationAgencyContextProvider localEducationAgencyContextProvider;
        private readonly IHttpServerProvider httpServerProvider;
        private readonly IPersistingRepository<ClaimSetMapping> claimSetMappingRepo;
        private readonly ICodeIdProvider codeIdProvider;
        private static readonly ILog log = LogManager.GetLogger(typeof(PositionTitleUserClaimSetsProvider));

        public PositionTitleUserClaimSetsProvider(IFile file, ICacheProvider cacheProvider, ILocalEducationAgencyContextProvider localEducationAgencyProvider,
                                                IHttpServerProvider httpServerProvider, IUserClaimSetsProvider<ClaimsSet, EdFiUserSecurityDetails> next, IPersistingRepository<ClaimSetMapping> claimSetMappingRepo, 
                                                ICodeIdProvider codeIdProvider)
        {
            this.file = file;
            this.cacheProvider = cacheProvider;
            this.localEducationAgencyContextProvider = localEducationAgencyProvider;
            this.Next = next;
            this.httpServerProvider = httpServerProvider;
            this.claimSetMappingRepo = claimSetMappingRepo;
            this.codeIdProvider = codeIdProvider;
        }

        /// <summary>
        /// If the cache contains the mapping dictionary then use it.
        /// If not, then generate the mapping dictionary from the text file,
        /// Add the mapping dictionary to the cache,
        /// And use the mapping dictionary for the role mappings.
        /// </summary>
        public Dictionary<string, ICollection<ClaimsSet>> ClaimSetsByPositionTitle
        {
            get
            {
                object cachedObject;

                if (cacheProvider.TryGetCachedObject(CurrentClaimSetMappingCacheKey, out cachedObject))
                {
                    return (Dictionary<string, ICollection<ClaimsSet>>)cachedObject;
                }

                //var pathname = GetPathname();
                //if (!file.Exists(pathname))
                //{
                //    log.WarnFormat("File {0} was not found. PositionTitleUserClaimSetsProvider can not load roles.", pathname);
                //    return null;
                //}

                //var data = file.ReadAllLines(pathname);
                //var result = ParseMappings(data);
                var result = ParseMappings();
                if (result != null)
                {
                    log.Info("PositionTitleUserClaimSetsProvider has loaded roles: " + CurrentClaimSetMappingCacheKey);

                    //TODO: tjm 5/22/12 talked to Geoff...temporarily setting this to cache for only 5 minutes until we get a better distributed caching in place
                    cacheProvider.Insert(CurrentClaimSetMappingCacheKey, result, DateTime.Now.AddMinutes(5), TimeSpan.Zero);
                }
                else
                {
                    log.WarnFormat(string.Format("PositionTitleUserClaimSetsProvider did not load any role mappings for {0}.", CurrentClaimSetMappingCacheKey));
                }

                return result;
            }	
        }

        //public static Dictionary<string, ICollection<ClaimsSet>> ParseMappings(IEnumerable<string> data)
        //{
        //    var mappings = new Dictionary<string, ICollection<ClaimsSet>>();

        //    foreach (var line in data)
        //    {
        //        if (line == null) continue;

        //        var temp = line.Trim();

        //        //Comment.
        //        if (temp.StartsWith("#")) continue;

        //        //blank line.
        //        if (temp.Length == 0) continue;

        //        //Invalid field count.
        //        string[] fields;
        //        if (line.StartsWith("\""))
        //        {
        //            fields = line.Split(new[] { "\"," }, StringSplitOptions.None);
        //            fields[0] = fields[0].Replace("\"", "");
        //        }
        //        else
        //            fields = line.Split(',');

        //        if (2 != fields.Length)
        //            throw new ArgumentException(string.Format(InvalidMappingFormat, line));

        //        var positionTitleName = fields[0].Trim().ToUpper();
        //        var claimSetName = fields[1].Trim().Replace(" ", "");

        //        // No Security ClaimsSet.
        //        if (claimSetName.ToUpper().Equals(None)) continue;

        //        try
        //        {
        //            var role = (ClaimsSet)Enum.Parse(typeof(ClaimsSet), claimSetName);

        //            if (mappings.ContainsKey(positionTitleName))
        //            {
        //                mappings[positionTitleName].Add(role);
        //            }
        //            else
        //            {
        //                var roleList = new List<ClaimsSet> { role };
        //                mappings.Add(positionTitleName, roleList);
        //            }

        //        }
        //        catch (ArgumentException ae)
        //        {
        //            throw new UserAccessDeniedException(string.Format(InvalidSystemClaimSetFormat, claimSetName), ae);
        //        }
        //    }

        //    return mappings;
        //}

        private Dictionary<string, ICollection<ClaimsSet>> ParseMappings()
        {
            //Get the LEA ID
            var leaId = codeIdProvider.Get(localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode());

            //get the ClaimSet mappings for this leaId
            var claimSetMappings = claimSetMappingRepo.GetAll().Where(map => map.LocalEducationAgencyId == leaId);

            var mappings = new Dictionary<string, ICollection<ClaimsSet>>();

            foreach (var claimSetMapping in claimSetMappings.ToList())
            {
                var positionTitleName = claimSetMapping.PositionTitle.Trim().ToUpper();
                var claimSetName = claimSetMapping.ClaimSet.Trim().Replace(" ", "");

                // No Security ClaimsSet.
                if (claimSetName.ToUpper().Equals(None)) continue;

                ClaimsSet role;
                if (!ClaimsSet.TryParse(claimSetName, true, out role))
                    throw new UserAccessDeniedException(string.Format(InvalidSystemClaimSetFormat, claimSetName));
                    
                if (mappings.ContainsKey(positionTitleName))
                {
                    mappings[positionTitleName].Add(role);
                }
                else
                {
                    var roleList = new List<ClaimsSet> { role };
                    mappings.Add(positionTitleName, roleList);
                }
            }

            return mappings;
        }

        /// <summary>
        /// Generates the role mapping data file location from the values returned from the ConfigValueProvider.
        /// In order to facilitate Unit Tests if the Local Education Agency Name is an absolute path name, then use it.
        /// Otherwise generate the pathname from the config values.
        /// </summary>
        /// <returns></returns>
        //public string GetPathname()
        //{
        //    var localEducationAgencyPath = localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode();

        //    // DJWhite, 31 August 2011:  This is here to facilitate testing of the
        //    // ParseMappings
        //    if (Path.IsPathRooted(localEducationAgencyPath))
        //        return localEducationAgencyPath;

        //    var localEducationAgencyName = localEducationAgencyPath.Replace(" ", "");
        //    var fullPath = httpServerProvider.MapPath(String.Format(claimSetMappingPathFormat, localEducationAgencyName));

        //    return fullPath;
        //}

        protected override bool CanGetUserClaimSets(EdFiUserSecurityDetails userSecurityDetails)
        {
            if (userSecurityDetails == null || String.IsNullOrEmpty(userSecurityDetails.PositionTitle))
                return false;

            var positionTitleKey = userSecurityDetails.PositionTitle.ToUpper();
            return (ClaimSetsByPositionTitle != null && ClaimSetsByPositionTitle.ContainsKey(positionTitleKey));
        }

        protected override IEnumerable<ClaimsSet> DoGetUserClaimSets(EdFiUserSecurityDetails userSecurityDetails)
        {
            var roles = new List<ClaimsSet>();

            var positionTitleKey = userSecurityDetails.PositionTitle.ToUpper();
            roles.AddRange(ClaimSetsByPositionTitle[positionTitleKey]);
            
            return roles.Distinct();
        }
    
        private string CurrentClaimSetMappingCacheKey
        {
            get { return String.Format(ClaimSetMappingKeyFormat, localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode()); }
        }
    }
}
