// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Principal;
using System.Threading;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using Microsoft.IdentityModel.Claims;
using Newtonsoft.Json;

namespace EdFi.Dashboards.SecurityTokenService.Authentication
{
    /// <summary>
    /// Contains information about a user, their associated organizations, and claims for the enforcement of application security.
    /// </summary>
    public class UserInformation
    {
        private readonly IClaimsIdentity claimsIdentity;

        public UserInformation()
        {
            this.claimsIdentity = new ClaimsIdentity();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserInformation"/> class from the provided <see cref="IClaimsIdentity"/> instance.
        /// </summary>
        /// <param name="claimsIdentity">The claims-based identity to use to initialize the <see cref="UserInformation"/> instance.</param>
        public UserInformation(IClaimsIdentity claimsIdentity)
        {
            this.claimsIdentity = claimsIdentity;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="UserInformation"/> class from the provided <see cref="IIdentity"/> instance.
        /// </summary>
        /// <param name="identity">The identity to use to initialize the <see cref="UserInformation"/> instance.</param>
        /// <exception cref="SecurityException">Occurs if the identity provided is not an <see cref="IClaimsIdentity"/>.</exception>
        public UserInformation(IIdentity identity)
        {
            if (identity is IClaimsIdentity)
                this.claimsIdentity = identity as IClaimsIdentity;
            else
                throw new SecurityException("Current principal is not a claims-based principal.");
        }

        /// <summary>
        /// Gets a new <see cref="UserInformation"/> instance using the thread's current principal/identity.
        /// </summary>
        /// <returns>A new <see cref="UserInformation"/> instance.</returns>
        /// <exception cref="SecurityException">Occurs if the thread's current principal is not an <see cref="IClaimsPrincipal"/>.</exception>
        public static UserInformation Current
        {
            get
            {
                const string userInformationContextKey = "UserInformation.Current";
                const string userInformationPrincipalHashKey = "UserInformation.PrincipalHashCode";
                
                // Try to get the UserInformation from the current call context
                var existingUserInformation = CallContext.GetData(userInformationContextKey) as UserInformation;
                var principalHash = (int?) CallContext.GetData(userInformationPrincipalHashKey);

                // No existing user information found in context, or current principal has changed?
                if ((principalHash ?? 0) != Thread.CurrentPrincipal.GetHashCode()
                    || existingUserInformation == null)
                {
                    var identity = Thread.CurrentPrincipal.Identity as IClaimsIdentity;

                    if (identity == null || identity.Claims == null || identity.Claims.Count == 0)
                        return null;

                    // Create the UserInformation from the Principal, and store it in context
                    existingUserInformation = new UserInformation(identity);
                    CallContext.SetData(userInformationContextKey, existingUserInformation);
                    CallContext.SetData(userInformationPrincipalHashKey, Thread.CurrentPrincipal.GetHashCode());
                }

                return existingUserInformation;
            }
        }

        /// <summary>
        /// Creates a <see cref="ClaimsPrincipal"/> instance from the information contained in the current <see cref="UserInformation"/> instance.
        /// </summary>
        /// <remarks>Since the UserInformation does not expose a username property, the "Name" claim will not be added to the returned <see cref="IClaimsPrincipal"/> instance.</remarks>
        /// <returns>A new <see cref="IClaimsPrincipal"/> instance.</returns>
        public IClaimsPrincipal ToClaimsPrincipal()
        {
            var identity = new ClaimsIdentity("UserInformation-Generated-Principal");

            AddBasicUserClaims(identity);

            //Create State level Claims
            AddStateAgencyClaims(identity);

            // Create Local Education Agency claims
            AddLocalEducationAgencyClaims(identity);

            // Create School claims
            AddSchoolClaims(identity);

            // Return the claims principal
            return new ClaimsPrincipal(identity.ToEnumerable());
        }

        /// <summary>
        /// Creates a <see cref="IClaimsPrincipal"/> instance from the information contained in the current <see cref="UserInformation"/> instance, augmented with externally defined username and roles.
        /// </summary>
        /// <param name="username">The "Name" claim to be added to the principal.</param>
        /// <param name="roleNames">The "ClaimsSet" claims to be added to the principal.</param>
        /// <returns>A new <see cref="IClaimsPrincipal"/> instance.</returns>
        public IClaimsPrincipal ToClaimsPrincipal(string username, IEnumerable<string> roleNames)
        {
            var principal = ToClaimsPrincipal();
            var id = principal.Identity as ClaimsIdentity;

            // Add the supplied username to the claims
            id.Claims.Add(new Claim(ClaimTypes.Name, username));

            // Add the ClaimsSet claims
            id.Claims.AddRange(
                from r in roleNames
                select new Claim(ClaimTypes.Role, r));

            return principal;
        }

        private void AddBasicUserClaims(ClaimsIdentity identity)
        {
            if (StaffUSI != 0)
                identity.Claims.Add(new Claim(EdFiClaimTypes.StaffUSI, StaffUSI.ToString()));

            if (!string.IsNullOrEmpty(FullName))
                identity.Claims.Add(new Claim(EdFiClaimTypes.FullName, FullName));

            if (!string.IsNullOrEmpty(FirstName))
                identity.Claims.Add(new Claim(ClaimTypes.GivenName, FirstName));

            if (!string.IsNullOrEmpty(LastName))
                identity.Claims.Add(new Claim(ClaimTypes.Surname, LastName));

            if (!string.IsNullOrEmpty(EmailAddress))
                identity.Claims.Add(new Claim(ClaimTypes.Email, EmailAddress));
        }

        private void AddSchoolClaims(ClaimsIdentity identity)
        {
            foreach (var school in AssociatedSchools)
            {
                if (school.ClaimTypes.Any())
                {
                    foreach (var claimType in school.ClaimTypes)
                        identity.Claims.Add(CreateSchoolClaim(school, claimType));
                }
                else
                {
                    // No claim was provided, but there are still orgs that need to be reflected in the outbound UserInformation object
                    identity.Claims.Add(CreateSchoolClaim(school, EdFiClaimTypes._OrgClaimNamespace + "Nothing"));
                }
            }
        }

        private void AddLocalEducationAgencyClaims(ClaimsIdentity identity)
        {
            foreach (var lea in AssociatedLocalEducationAgencies)
            {
                if (lea.ClaimTypes.Any())
                {
                    foreach (var claimType in lea.ClaimTypes)
                        identity.Claims.Add(CreateLocalEducationAgencyClaim(lea, claimType));
                }
                else
                {
                    // No claim was provided, but there are still orgs that need to be reflected in the outbound UserInformation object
                    identity.Claims.Add(CreateLocalEducationAgencyClaim(lea, EdFiClaimTypes._OrgClaimNamespace + "Nothing"));
                }
            }
        }

        private void AddStateAgencyClaims(ClaimsIdentity identity)
        {
            foreach (var stateAgency in AssociatedStateAgencies)
            {
                if (stateAgency.ClaimTypes.Any())
                {
                    foreach (var claimType in stateAgency.ClaimTypes)
                        identity.Claims.Add(CreateStateAgencyClaim(stateAgency, claimType));
                }
                else
                {
                    // No claim was provided, but there are still orgs that need to be reflected in the outbound UserInformation object
                    identity.Claims.Add(CreateStateAgencyClaim(stateAgency, EdFiClaimTypes._OrgClaimNamespace + "Nothing"));
                }
            }
        }

        private static Claim CreateStateAgencyClaim(StateAgency stateAgency, string claimType)
        {
            var properties = new Dictionary<string, string>();
            properties.Add(EdFiClaimProperties.StateAgencyId, stateAgency.StateAgencyId.ToString());
            properties.Add(EdFiClaimProperties.EducationOrganizationName, stateAgency.Name);

            string serializedProperties = JsonConvert.SerializeObject(properties);

            var claim = new Claim(claimType, serializedProperties);

            return claim;
        }

        private static Claim CreateLocalEducationAgencyClaim(LocalEducationAgency lea, string claimType)
        {
            var properties = new Dictionary<string, string>();
            //properties.Add(EdFiClaimProperties.EducationOrganizationName, lea.Name);
            properties.Add(EdFiClaimProperties.LocalEducationAgencyId, lea.LocalEducationAgencyId.ToString());
            properties.Add(EdFiClaimProperties.EducationOrganizationName, lea.Name);

            string serializedProperties = JsonConvert.SerializeObject(properties);

            var claim = new Claim(claimType, serializedProperties);

            return claim;
        }

        private static Claim CreateSchoolClaim(School school, string claimType)
        {
            var properties = new Dictionary<string, string>();
            //claim.Properties.Add(EdFiClaimProperties.EducationOrganizationName, school.Name);
            properties.Add(EdFiClaimProperties.SchoolId, school.SchoolId.ToString());
            properties.Add(EdFiClaimProperties.LocalEducationAgencyId, school.LocalEducationAgencyId.ToString());
            properties.Add(EdFiClaimProperties.EducationOrganizationName, school.Name);

            string serializedProperties = JsonConvert.SerializeObject(properties);

            var claim = new Claim(claimType, serializedProperties);

            return claim;
        }

        #region StaffUSI Property

        /// <summary>
        /// Holds the value for the <see cref="StaffUSI"/> property.
        /// </summary>
        private long staffUSI;

        /// <summary>
        /// Gets or sets the user's staff identifier.
        /// </summary>
        public long StaffUSI
        {
            // Note: This was left as type "int" rather than a nullable type because of downstream
            // issues of existing code in having to deal with a nullable type.  In the long term,
            // we could have other identifiers for the user than a StaffUSI (e.g. parents?), and 
            // thus a nullable type seems more appropriate.  Pragmatism rules.  It's an "int".
            get
            {
                if (staffUSI == 0)
                {
                    if (claimsIdentity.StaffUSI().HasValue)
                        staffUSI = claimsIdentity.StaffUSI().Value;
                }

                return staffUSI;
            }
            set { staffUSI = value; }
        }

        #endregion

        #region FullName Property

        /// <summary>
        /// Holds the value for the <see cref="FullName"/> property.
        /// </summary>
        private string fullName;

        /// <summary>
        /// Gets or sets the user's full name.
        /// </summary>
        public string FullName
        {
            get { return fullName ?? (fullName = claimsIdentity.FullName()); }
            set { fullName = value; }
        }

        #endregion

        #region FirstName Property

        /// <summary>
        /// Holds the value for the <see cref="FirstName"/> property.
        /// </summary>
        private string firstName;

        /// <summary>
        /// Gets or sets the user's first name.
        /// </summary>
        public string FirstName
        {
            get { return firstName ?? (firstName = claimsIdentity.FirstName()); }
            set { firstName = value; }
        }

        #endregion

        #region LastName Property

        /// <summary>
        /// Holds the value for the <see cref="LastName"/> property.
        /// </summary>
        private string lastName;

        /// <summary>
        /// Gets or sets the user's last name.
        /// </summary>
        public string LastName
        {
            get { return lastName ?? (lastName = claimsIdentity.LastName()); }
            set { lastName = value; }
        }

        #endregion

        #region EmailAddress Property

        /// <summary>
        /// Holds the value for the <see cref="EmailAddress"/> property.
        /// </summary>
        private string emailAddress;

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string EmailAddress
        {
            get
            {
                if (string.IsNullOrEmpty(emailAddress) && string.IsNullOrEmpty(claimsIdentity.Email()))
                    return string.Empty;

                return emailAddress ?? (emailAddress = claimsIdentity.Email());
            }
            set { emailAddress = value; }
        }

        #endregion

        #region AssociatedOrganizations Property

        /// <summary>
        /// Holds the value for the <see cref="AssociatedOrganizations"/> property.
        /// </summary>
        private IEnumerable<EducationOrganization> associatedOrganizations;

        /// <summary>
        /// Gets or sets the organizations (e.g. Educational Service Centers, Local Education Agencies, Schools) with which the user is associated.
        /// </summary>
        public IEnumerable<EducationOrganization> AssociatedOrganizations
        {
            get { return associatedOrganizations ?? (associatedOrganizations = GetEducationOrganizations()); }
            set { associatedOrganizations = value; }
        }

        private IEnumerable<EducationOrganization> GetEducationOrganizations()
        {
            var orgs = new List<EducationOrganization>();

            var claimsByOrg =
                from c in claimsIdentity.Claims
                where c.ClaimType.StartsWith(EdFiClaimTypes._OrgClaimNamespace)
                let props = JsonConvert.DeserializeObject<Dictionary<string, string>>(c.Value)
                group c by
                    new
                        {
                            StateAgencyId = props.ContainsKey(EdFiClaimProperties.StateAgencyId) ? props[EdFiClaimProperties.StateAgencyId].Convert<int?>() : (int?)null, //c.StateAgency()
                            LocalEducationAgencyId = props.ContainsKey(EdFiClaimProperties.LocalEducationAgencyId) ? props[EdFiClaimProperties.LocalEducationAgencyId].Convert<int?>() : (int?) null, // c.LocalEducationAgencyId(),
                            SchoolId = props.ContainsKey(EdFiClaimProperties.SchoolId) ? props[EdFiClaimProperties.SchoolId].Convert<int?>() : (int?)null, //c.SchoolId(),
                            EducationOrganizationName = props.ContainsKey(EdFiClaimProperties.EducationOrganizationName) ? props[EdFiClaimProperties.EducationOrganizationName] : null, // c.EducationOrganizationName(),
                        }
                into g
                select new
                        {
                            Key = g.Key, 
                            ClaimTypes = g.Select(c => c.ClaimType).ToList()
                        };

            foreach (var groupedClaim in claimsByOrg)
            {
                if (groupedClaim.Key.SchoolId != null)
                {
                    if (groupedClaim.Key.LocalEducationAgencyId == null)
                        throw new InvalidOperationException("Claim for School was supplied without the corresponding Local Education Agency Id.");

                    orgs.Add(
                        new School(groupedClaim.Key.LocalEducationAgencyId.Value, groupedClaim.Key.SchoolId.Value)
                            {
                                Name = groupedClaim.Key.EducationOrganizationName,
                                ClaimTypes = groupedClaim.ClaimTypes,
                            });
                }
                else if (groupedClaim.Key.LocalEducationAgencyId != null)
                {
                    orgs.Add(
                        new LocalEducationAgency(groupedClaim.Key.LocalEducationAgencyId.Value)
                            {
                                Name = groupedClaim.Key.EducationOrganizationName,
                                ClaimTypes = groupedClaim.ClaimTypes,
                            });
                }
                else if (groupedClaim.Key.StateAgencyId != null)
                {
                    orgs.Add(
                        new StateAgency(groupedClaim.Key.StateAgencyId.Value)
                        {
                            Name = groupedClaim.Key.EducationOrganizationName,
                            ClaimTypes = groupedClaim.ClaimTypes,
                        });
                }
            }

            return orgs;
        }

        #endregion

        #region AssociatedSchools Property

        /// <summary>
        /// Gets the schools with which the user is associated.
        /// </summary>
        public IEnumerable<School> AssociatedSchools
        {
            get { return AssociatedOrganizations.OfType<School>(); }
        }

        #endregion

        #region AssociatedLocalEducationAgencies Property

        /// <summary>
        /// Gets the schools with which the user is associated.
        /// </summary>
        public IEnumerable<LocalEducationAgency> AssociatedLocalEducationAgencies
        {
            get { return AssociatedOrganizations.OfType<LocalEducationAgency>(); }
        }

        #endregion

        #region AssociatedStateAgencies Property

        /// <summary>
        /// Gets the schools with which the user is associated.
        /// </summary>
        public IEnumerable<StateAgency> AssociatedStateAgencies
        {
            get { return AssociatedOrganizations.OfType<StateAgency>(); }
        }

        #endregion

        // VINLOGINTYP
        #region ServiceType Property

        /// <summary>
        /// Holds the value for the <see cref="ServiceType"/> property.
        /// </summary>
        private string serviceType;

        /// <summary>
        /// Gets or sets the Dashboard Service Type
        /// </summary>
        public string ServiceType
        {
            get
            {
                if (string.IsNullOrEmpty(serviceType) && string.IsNullOrEmpty(claimsIdentity.ServiceType()))
                    return string.Empty;

                return serviceType ?? (serviceType = claimsIdentity.ServiceType());
            }
            set { serviceType = value; }
        }

        #endregion

        // EDFIDB-139
        #region UserType Property

        /// <summary>
        /// Holds the value for the <see cref="UserType"/> property.
        /// </summary>
        private int userType;

        /// <summary>
        /// Gets or sets the user's type identifier.
        /// </summary>
        public int UserType
        {
            // Note: This was left as type "int" rather than a nullable type because of downstream
            // issues of existing code in having to deal with a nullable type.  In the long term,
            // we could have other identifiers for the user than a StaffUSI (e.g. parents?), and 
            // thus a nullable type seems more appropriate.  Pragmatism rules.  It's an "int".
            get
            {
                if (userType == 0)
                {
                    if (claimsIdentity.UserType().HasValue)
                        userType = claimsIdentity.UserType().Value;
                }

                return userType;
            }
            set { userType = value; }
        }

        #endregion

        // VIN05112015
        #region UserId

        /// <summary>
        /// Holds the value for the <see cref="UserId"/> property.
        /// </summary>
        private string userId;

        /// <summary>
        /// Gets or sets the Dashboard User Id
        /// </summary>
        public string UserId
        {
            get
            {
                if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(claimsIdentity.UserId()))
                    return string.Empty;

                return userId ?? (userId = claimsIdentity.UserId());
            }
            set { userId = value; }
        }

        #endregion

        // VIN05112015
        #region UserToken

        /// <summary>
        /// Holds the value for the <see cref="UserId"/> property.
        /// </summary>
        private string userToken;

        /// <summary>
        /// Gets or sets the Dashboard User Id
        /// </summary>
        public string UserToken
        {
            get
            {
                if (string.IsNullOrEmpty(userToken) && string.IsNullOrEmpty(claimsIdentity.UserToken()))
                    return string.Empty;

                return userToken ?? (userToken = claimsIdentity.UserToken());
            }
            set { UserToken = value; }
        }

        #endregion

        private string AssociatedOrganizationsToString()
        {
            return string.Format("[ {0}]", 
                string.Join(",", AssociatedOrganizations));
        }

        public bool HasNothingClaimType(IEnumerable<string> claimTypes)
        {
            var result = claimTypes.All(n => n.ToLower().EndsWith("nothing"));
            return result;
        }

        /// <summary>
        /// Indicates whether the user is affiliated with the specified local education agency (has any claims at any organization level associated with the local education agency).
        /// </summary>
        /// <param name="localEducationAgencyId">The identifier of the local education agency.</param>
        /// <returns><b>true</b> if the user is affiliated with the local education agency; otherwise <b>false</b>.</returns>
        public bool IsAffiliatedWithLocalEducationAgency(int? localEducationAgencyId)
        {
            if (localEducationAgencyId.HasValue)
                return AssociatedLocalEducationAgencies.Any(x => x.LocalEducationAgencyId == localEducationAgencyId)
                       || AssociatedSchools.Any(x => x.LocalEducationAgencyId == localEducationAgencyId)
                       || AssociatedStateAgencies.Any();
            return true;
        }

        /// <summary>
        /// Indicates whether the user has any local education agency-level claims for the local education agency with which they are affiliated.
        /// </summary>
        /// <returns><b>true</b> if the user has a claim for the local education agency; otherwise <b>false</b>.</returns>
        public bool HasAnyLocalEducationAgencyLevelClaims(int localEducationAgencyId)
        {
            var result = (AssociatedLocalEducationAgencies.Any(
               n => (n.LocalEducationAgencyId == localEducationAgencyId && ((n.ClaimTypes.Any()) && (!HasNothingClaimType(n.ClaimTypes)))
                         )));

            if (result)
                return result;

            var claimAtState = AssociatedStateAgencies.Any(n => n.ClaimTypes.Any() && !HasNothingClaimType(n.ClaimTypes));

            return claimAtState;
        }

        /// <summary>
        /// Indicates whether the user has a specific claim on any education organization.
        /// </summary>
        /// <param name="claimType">The claim whose presence is being sought.</param>
        /// <returns><b>true</b> if the user has the claim anywhere; otherwise <b>false</b>.</returns>
        public bool HasIdentityClaim(string claimType)
        {
            var result = claimsIdentity.Claims.Any(
                n => n.ClaimType == claimType);
            return result;
        }

        /// <summary>
        /// Gets a string containing the StaffUSI, the user's full name, and all associated education organizations.
        /// </summary>
        /// <returns>The formatted string.</returns>
        public override string ToString()
        {
            return string.Format("{0}[ {1}, {2}, {3}]", 
                                 typeof(UserInformation).Name, StaffUSI, FullName, AssociatedOrganizationsToString());
        }

        public abstract class EducationOrganization
        {
            /// <summary>
            /// Gets or sets the EducationOrganizationId with which the user is associated.
            /// </summary>
            public abstract int EducationOrganizationId { get; }

            /// <summary>
            /// Gets or sets the type of organization (ESC, LocalEducationAgency, School, etc.) with which the user is associated.
            /// </summary>
            public abstract EducationOrganizationCategory Category { get; }

            /// <summary>
            /// Gets or sets the name of the organization.
            /// </summary>
            public string Name { get; set; }

            ///// <summary>
            ///// Gets or sets the staff's category (i.e. Principal, Assistant Principal, LocalEducationAgency, Teacher, Other, etc.)
            ///// </summary>
            //public string StaffCategory { get; set; }

            ///// <summary>
            ///// Gets or sets the user's position title. This is the local education agency specific role.
            ///// </summary>
            //public string PositionTitle { get; set; }

            #region ClaimTypes Property

            /// <summary>
            /// Holds the value for the <see cref="ClaimTypes"/> property.
            /// </summary>
            private IEnumerable<string> _claimTypes;

            /// <summary>
            /// Gets the claims types associated with the user at the current organization.
            /// </summary>
            public IEnumerable<string> ClaimTypes
            {
                get
                {
                    if (_claimTypes == null)
                        return Enumerable.Empty<string>();

                    return _claimTypes;
                }
                set { _claimTypes = value; }
            }

            #endregion
        }

        public class StateAgency : EducationOrganization
        {
            public StateAgency(int stateAgencyId)
            {
                StateAgencyId = stateAgencyId;
            }

            /// <summary>
            /// Gets or sets the identifier of the state agency.
            /// </summary>
            public int StateAgencyId { get; set; }

            /// <summary>
            /// Gets or sets the EducationOrganizationId with which the user is associated.
            /// </summary>
            public override int EducationOrganizationId
            {
                get { return StateAgencyId; }
            }

            /// <summary>
            /// Gets or sets the type of organization (ESC, LocalEducationAgency, School, etc.) with which the user is associated.
            /// </summary>
            public override EducationOrganizationCategory Category
            {
                get { return EducationOrganizationCategory.StateAgency; }
            }
        }

        public class LocalEducationAgency : EducationOrganization
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="LocalEducationAgency"/> class with the specified identifier.
            /// </summary>
            /// <param name="localEducationAgencyId">The unique identifier of the Local Education Agency.</param>
            public LocalEducationAgency(int localEducationAgencyId)
            {
                LocalEducationAgencyId = localEducationAgencyId;
            }

            /// <summary>
            /// Gets or sets the identifier of the local education agency with which the school is associated.
            /// </summary>
            public int LocalEducationAgencyId { get; set; }

            /// <summary>
            /// Returns the <see cref="LocalEducationAgencyId"/>.
            /// </summary>
            public override int EducationOrganizationId
            {
                get { return LocalEducationAgencyId; }
            }

            /// <summary>
            /// Always returns <see cref="EducationOrganizationCategory.LocalEducationAgency"/>.
            /// </summary>
            public override EducationOrganizationCategory Category
            {
                get { return EducationOrganizationCategory.LocalEducationAgency; }
            }
        }

        public class School : EducationOrganization
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="School"/> class with the specified identifiers.
            /// </summary>
            /// <param name="localEducationAgencyId">The unique identifier of the Local Education Agency.</param>
            /// <param name="schoolId">The unique identifier of the School.</param>
            public School(int localEducationAgencyId, int schoolId)
            {
                SchoolId = schoolId;
                LocalEducationAgencyId = localEducationAgencyId;
            }

            /// <summary>
            /// Gets or sets the school identifier.
            /// </summary>
            public int SchoolId { get; set; }

            /// <summary>
            /// Gets or sets the identifier of the local education agency with which the school is associated.
            /// </summary>
            public int LocalEducationAgencyId { get; set; }

            /// <summary>
            /// Returns the <see cref="SchoolId"/>.
            /// </summary>
            public override int EducationOrganizationId
            {
                get { return SchoolId; }
            }

            /// <summary>
            /// Always returns <see cref="EducationOrganizationCategory.School"/>.
            /// </summary>
            public override EducationOrganizationCategory Category
            {
                get { return EducationOrganizationCategory.School; }
            }
        }
    }
}
