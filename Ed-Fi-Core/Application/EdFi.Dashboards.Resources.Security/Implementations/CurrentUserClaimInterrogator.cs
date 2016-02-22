// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    //NOTE: In the installer, for this interface, property injection is turned off to prevent a cyclical dependency issue.
    public class CurrentUserClaimInterrogator : ICurrentUserClaimInterrogator
    {
        private const string cacheKeyPrefix = "CurrentUserClaimInterrogator.GetEducationOrganizationHierarchy";
        private readonly ISessionStateProvider sessionStateProvider;

        public CurrentUserClaimInterrogator(ISessionStateProvider sessionStateProvider)
        {
            this.sessionStateProvider = sessionStateProvider;
        }

        private IIdNameService schoolIdNameService;
        //These setters are public so that the Unit test can set them.
        //The setters will only work if the get was not already called.
        public IIdNameService SchoolIdNameService
        {
            set { schoolIdNameService = schoolIdNameService ?? value; }
            private get { return schoolIdNameService ?? (schoolIdNameService = IoC.Resolve<IIdNameService>()); }
        }
        private IIdCodeService leaIdCodeService;
        public IIdCodeService LeaIdCodeService
        {
            set { leaIdCodeService = leaIdCodeService ?? value; }
            private get { return leaIdCodeService ?? (leaIdCodeService = IoC.Resolve<IIdCodeService>()); }
        }
        private IDomainSpecificMetricNodeResolver metricNodeResolver;
        public IDomainSpecificMetricNodeResolver MetricNodeResolver
        {
            set { metricNodeResolver = metricNodeResolver ?? value; }
            private get { return metricNodeResolver ?? (metricNodeResolver = IoC.Resolve<IDomainSpecificMetricNodeResolver>()); }
        }
		private static UserInformation userInfo { get { return UserInformation.Current; } }

        private int? stateAgencyId;
        private int? StateAgencyId
        {
            get
            {
                //Note: This is only set once because the state value is (at this time) the same for all users.
                if (!stateAgencyId.HasValue)
                {
                    var stateAgency = userInfo.AssociatedStateAgencies.SingleOrDefault();
                    if (stateAgency != null)
                        stateAgencyId = stateAgency.EducationOrganizationId;
                }
                return stateAgencyId;
            }
        }

        /// <summary>
        /// Indicates whether the claim supplied is present for the current user for
        /// the current education organization or at an educational organization above this one in the education organization hierarchy.
        /// e.g. School => LocalEducationAgency => StateAgency etc.
        /// </summary>
        /// <param name="claimType">The claim type to be checked.</param>
        /// <param name="educationOrganizationId">The educational organization that is the base for the hierarchical check.</param>
        /// <returns><b>true</b> if the user has the claim within the hierarchy of the specified education organization; otherwise <b>false</b>.</returns>
        public virtual bool HasClaimWithinEducationOrganizationHierarchy(string claimType, int educationOrganizationId)
        {
            var hierarchy = GetEducationOrganizationHierarchy(educationOrganizationId);
            var result = HasClaimWithinEducationOrganizations(claimType, hierarchy);

            return result;
        }

        /// <summary>
        /// This method provides enumerable integers for all the educational organizations in the hierarchy starting at the
        /// provided base and progressing to the top level.
        /// </summary>
        /// <param name="educationOrganizationId">The educational organization that the base for the hierarchical check</param>
        /// <returns>Enumerable integers representing the educational organizations in the hierarchy</returns>
        public virtual IEnumerable<int> GetEducationOrganizationHierarchy(int educationOrganizationId)
        {
            // Using the cachekey prefix to differentiate in the session state provider and adding the 
            // EdOrgId and StaffUsi to be unique per user and EdOrg.
            var cacheKey = cacheKeyPrefix + "|" + educationOrganizationId + "|" + userInfo.StaffUSI;
            var result = sessionStateProvider.GetValue(cacheKey) as IEnumerable<int>;

            if (result == null)
            {
                result = BuildEducationOrganizationHierarchy(educationOrganizationId);
                sessionStateProvider.SetValue(cacheKey, result);
            }

            return result;
        }

        private HashSet<int> BuildEducationOrganizationHierarchy(int educationOrganizationId)
        {
            //Always add what was passed.
            HashSet<int> hierarchy = new HashSet<int> { educationOrganizationId };

            //Always add statewide id as it is the pinnacle of the hierarchy. If Add returns false it was the ed org passed.
            if (StateAgencyId != null && !hierarchy.Add((int)StateAgencyId))
                //the StateOrganizationId was passed so there is nothing above it.
                //so, Return.  
                return hierarchy;

            //If the educationOrganizationId is a local education agency 
            if (IsLocalEducationAgency(educationOrganizationId))
            {
                //LEA and State (if available) are now present, return
                return hierarchy;
            }

            //Try to get the LEA ID if this ed org is a school
            int? localEducationAgencyId = GetLocalEducationAgencyForSchool(educationOrganizationId);
            //If we got a value, then we have a school
            if (localEducationAgencyId.HasValue)
            {
                //Add LEA for the school
                hierarchy.Add(localEducationAgencyId.Value);
            }

            return hierarchy;
        }

        /// <summary>
        /// This method checks if the metric is an operational dashboard and validates associated claims for the current user.
        /// If it is not an operational dashboard metric, it just confirms that at least one of the metric claims exists for the current user.
        /// This is not metric action or metric value(data) validation this checks the Metric only.
        /// </summary>
        /// <param name="metricId">The metric id to operate on.</param>
        /// <param name="educationalOrganizationId">The education organization to use as a base for the metric claim checks.</param>
        /// <returns><b>true</b> if the user has a claim to view the metric; otherwise <b>false</b>.</returns>
        public virtual bool HasClaimForMetricWithinEducationOrganizationHierarchy(int metricId, int educationalOrganizationId)
        {
            MetricMetadataNode metricMetadataNode;
            //If this returns we have a school.
            int? localEducationAgencyCode = GetLocalEducationAgencyForSchool(educationalOrganizationId);
            if (localEducationAgencyCode.HasValue)
            {
                metricMetadataNode = MetricNodeResolver.GetOperationalDashboardMetricNode(MetricInstanceSetType.School,
                                                                                          educationalOrganizationId);
                if (IsMetricIdInOperationalMetricMetadataNodes(metricId, metricMetadataNode.DescendantsOrSelf))
                {
                    return HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewOperationalDashboard,
                                                                        educationalOrganizationId);
                }
            }

            //GetLeaOperationalMetrics
            metricMetadataNode = MetricNodeResolver.GetOperationalDashboardMetricNode(MetricInstanceSetType.LocalEducationAgency);

            if (IsMetricIdInOperationalMetricMetadataNodes(metricId, metricMetadataNode.DescendantsOrSelf))
            {
                return HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewOperationalDashboard, educationalOrganizationId);
            }

            //Default is that they are all available other than operational dashboards. Make sure they have one of the metric claims.
            //NOTE: ViewMyMetrics is applied through filtering independently.
            return HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewMyMetrics, educationalOrganizationId) ||
            HasClaimWithinEducationOrganizationHierarchy(EdFiClaimTypes.ViewAllMetrics, educationalOrganizationId);
        }

        //TODO: remove this method and replace usages with Geoff's update once integrated.
        private static bool IsMetricIdInOperationalMetricMetadataNodes(int metricId, IEnumerable<MetricMetadataNode> metricMetadataNodes)
        {
            return metricMetadataNodes.Any(decendentMetricMetadataNode => decendentMetricMetadataNode.MetricId == metricId);
        }

        private Dictionary<int, bool> isLocalEducationAgencyById = new Dictionary<int, bool>(); 

        private bool IsLocalEducationAgency(int educationOrganizationId)
        {
            bool result;

            if (!isLocalEducationAgencyById.TryGetValue(educationOrganizationId, out result))
            {
                var model = LeaIdCodeService.Get(IdCodeRequest.Create(id: educationOrganizationId));
                result = model != null;
                isLocalEducationAgencyById[educationOrganizationId] = result;
            }

            return result;
        }

        private Dictionary<int, int?> localEducationAgencyIdBySchoolId = new Dictionary<int, int?>(); 

        private int? GetLocalEducationAgencyForSchool(int educationOrganizationId)
        {
            int? result;

            if (!localEducationAgencyIdBySchoolId.TryGetValue(educationOrganizationId, out result))
            {
                var request = IdNameRequest.Create(educationOrganizationId);
                var model = SchoolIdNameService.Get(request);

                //We have a school model
                if (model != null)
                    result = model.LocalEducationAgencyId;
                else
                    result = null;

                localEducationAgencyIdBySchoolId[educationOrganizationId] = result;
            }

            return result;
        }

        /// <summary>
        /// Indicates whether the user has a specific claim explicitly on any of the specifed education organizations.
        /// </summary>
        /// <param name="claimType">The claim type to be checked.</param>
        /// <param name="educationOrganizationIds">Specific education organization ids for which the claim should exist.</param>
        /// <returns><b>true</b> if the user has the claim on any of the specified education organizations; otherwise <b>false</b>.</returns>
        public virtual bool HasClaimWithinEducationOrganizations(string claimType, IEnumerable<int> educationOrganizationIds)
        {
            return educationOrganizationIds.Any(educationOrganizationId => HasClaimOnEducationOrganization(claimType, educationOrganizationId));
        }

        /// <summary>
        /// Indicates whether the user has a specific claim explicitly on the specified education organization.
        /// </summary>
        /// <param name="educationOrganizationId">The identifier of the education organization that should be associated with the claim being sought.</param>
        /// <param name="claimType">The claim whose presence is being sought.</param>
        /// <returns><b>true</b> if the user has the claim on the specified education organization; otherwise <b>false</b>.</returns>
        public virtual bool HasClaimOnEducationOrganization(string claimType, int educationOrganizationId)
        {
            var result = (userInfo.AssociatedOrganizations.Any(
                n => n.ClaimTypes.Contains(claimType) && n.EducationOrganizationId == educationOrganizationId));
            return result;
        }

        /// <summary>
        /// Indicates whether the current user has the claim for the LEA specified or the LEA associated for the School specified, within thier organizational Hierarchy.
        /// This means for the claim must be assoicated to the LEA or higher. 
        /// </summary>
        /// <param name="claimType">The claim type to be checked.</param>
        /// <param name="educationalOrganizationId">The educational organization that is the LEA or the school for determining the LEA</param>
        /// <returns><b>true</b> if the user has the claim on the LEA or higher; otherwise <b>false</b>.</returns>
        public virtual bool HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(string claimType, int educationalOrganizationId)
        {
            int? localEducationAgencyCode = GetLocalEducationAgencyForSchool(educationalOrganizationId);
            if (!localEducationAgencyCode.HasValue)
            {
                //Make Sure we are looking for an LEA
                if (IsLocalEducationAgency(educationalOrganizationId))
                    localEducationAgencyCode = educationalOrganizationId;
                //Otherwise there is no LEA.
                else
                    return false;
            }
            return HasClaimWithinEducationOrganizationHierarchy(claimType, localEducationAgencyCode.Value);
        }

        /// <summary>
        /// Indicates whether the user has a specific claim explicitly on the state agency.
        /// </summary>
        /// <param name="claimType">The claim type to be checked.</param>
        /// <returns><b>true</b> if the user has the claim on the state agency; otherwise <b>false</b>.</returns>
        public virtual bool HasClaimForStateAgency(string claimType)
        {
            return StateAgencyId != null && HasClaimOnEducationOrganization(claimType, (int)StateAgencyId);
        }

        /// <summary>
        /// Indicates whether the user has the claim at any of thier associated educational organizations.
        /// </summary>
        /// <param name="claimType">The claim type to be checked.</param>
        /// <returns><b>true</b> if the user has the claim on any of thier asssociated educatinoal organizations; otherwise <b>false</b>.</returns>
        public virtual bool HasClaimForSearch(string claimType)
        {
            return HasClaimWithinEducationOrganizations(claimType, userInfo.AssociatedOrganizations.Select(a => a.EducationOrganizationId));
        }
    }
}
