// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Security.Common
{
    public interface ICurrentUserClaimInterrogator
    {
        /// <summary>
        /// This method checks if the metric is an operational dashboard and validates associated claims for the current user.
        /// If it is not an operational dashboard metric, it just confirms that at least one of the metric claims exists for the current user.
        /// This is not metric action or metric value(data) validation this checks the Metric only.
        /// </summary>
        /// <param name="metricId">The metric id to operate on.</param>
        /// <param name="educationalOrganizationId">The education organization to use as a base for the metric claim checks.</param>
        /// <returns><b>true</b> if the user has a claim to view the metric; otherwise <b>false</b>.</returns>
        bool HasClaimForMetricWithinEducationOrganizationHierarchy(int metricId, int educationalOrganizationId);

        /// <summary>
        /// Indicates whether the current user has the claim for the LEA specified or the LEA associated for the School specified, within their organizational Hierarchy.
        /// This means for the claim must be associated to the LEA or higher. 
        /// </summary>
        /// <param name="claimType">The claim type to be checked.</param>
        /// <param name="educationalOrganizationId">The educational organization that is the LEA or the school for determining the LEA</param>
        /// <returns><b>true</b> if the user has the claim on the LEA or higher; otherwise <b>false</b>.</returns>
        bool HasClaimForLocalEducationAgencyWithinEducationOrganizationHierarchy(string claimType, int educationalOrganizationId);

        /// <summary>
        /// Indicates whether the claim supplied is present for the current user for
        /// the current education organization or at an educational organization above this one in the education organization hierarchy.
        /// e.g. School => LocalEducationAgency => StateAgency etc.
        /// </summary>
        /// <param name="claimType">The claim type to be checked.</param>
        /// <param name="educationOrganizationId">The educational organization that is the base for the hierarchical check.</param>
        /// <returns><b>true</b> if the user has the claim within the hierarchy of the specified education organization; otherwise <b>false</b>.</returns>
        bool HasClaimWithinEducationOrganizationHierarchy(string claimType, int educationOrganizationId);

        /// <summary>
        /// This method provides enumerable integers for all the educational organizations in the hierarchy starting at the
        /// provided base and progressing to the top level.
        /// </summary>
        /// <param name="educationOrganizationId">The educational organization that the base for the hierarchical check</param>
        /// <returns>Enumerable integers representing the educational organizations in the hierarchy</returns>
        IEnumerable<int> GetEducationOrganizationHierarchy(int educationOrganizationId);

        /// <summary>
        /// Indicates whether the user has a specific claim explicitly on any of the specified education organizations.
        /// </summary>
        /// <param name="claimType">The claim type to be checked.</param>
        /// <param name="educationOrganizationIds">Specific education organization ids for which the claim should exist.</param>
        /// <returns><b>true</b> if the user has the claim on any the specified education organizations; otherwise <b>false</b>.</returns>
        bool HasClaimWithinEducationOrganizations(string claimType, IEnumerable<int> educationOrganizationIds);

        /// <summary>
        /// Indicates whether the user has a specific claim explicitly on the specified education organization.
        /// </summary>
        /// <param name="educationOrganizationId">The identifier of the education organization that should be associated with the claim being sought.</param>
        /// <param name="claimType">The claim whose presence is being sought.</param>
        /// <returns><b>true</b> if the user has the claim on the specified education organization; otherwise <b>false</b>.</returns>
        bool HasClaimOnEducationOrganization(string claimType, int educationOrganizationId);

        /// <summary>
        /// Indicates whether the user has a specific claim explicitly on the state agency.
        /// </summary>
        /// <param name="claimType">The claim type to be checked.</param>
        /// <returns><b>true</b> if the user has the claim on the state agency; otherwise <b>false</b>.</returns>
        bool HasClaimForStateAgency(string claimType);

        /// <summary>
        /// Indicates whether the user has the claim at any of their associated educational organizations.
        /// </summary>
        /// <param name="claimType">The claim type to be checked.</param>
        /// <returns><b>true</b> if the user has the claim on any of their associated educational organizations; otherwise <b>false</b>.</returns>
        bool HasClaimForSearch(string claimType);
    }
}
