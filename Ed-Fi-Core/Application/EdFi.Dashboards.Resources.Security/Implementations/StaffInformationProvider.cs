using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    public class StaffInformationProvider : IStaffInformationProvider
    {
        private const string NoLookupValueErrorMessageFormat = "We were able to verify your password, but were unable to resolve your lookup value from username '{0}' to field '{1}'.  If you feel this is incorrect, please contact an administrator.";
        private const string NoStaffUSIAssociationErrorMessageFormat = "We were able to resolve username '{0}', but we were not able to find this resolved value in the StudentGPS system.  If you feel this is incorrect, please contact an administrator.";
        private const string MultipleStaffUSIAssociationErrorMessageFormat = "We were able to resolve username '{0}', but we found multiple staff users for this resolved value in the StudentGPS system.  If you feel this is incorrect, please contact an administrator.";

        public int UserType { get; set; } // VIN22012016
        private readonly Dictionary<string, string> _usernamesByStaffUSI = new Dictionary<string, string>();
        private readonly IRepository<StaffInformation> _staffInformationRepository;
        private readonly IStaffInformationLookupKeyProvider _staffInformationLookupKeyProvider;

        public StaffInformationProvider(IRepository<StaffInformation> staffInformationRepository, IStaffInformationLookupKeyProvider staffInformationLookupKeyProvider)
        {
            _staffInformationRepository = staffInformationRepository;
            _staffInformationLookupKeyProvider = staffInformationLookupKeyProvider;
        }

        public long ResolveStaffUSI(IAuthenticationProvider authenticationProvider, string username)
        {
            var staffInfoLookupKey = _staffInformationLookupKeyProvider.GetStaffInformationLookupKey();
            var staffInfoLookupValue = authenticationProvider.ResolveUsernameToLookupValue(username, staffInfoLookupKey);
            
            if (string.IsNullOrEmpty(staffInfoLookupValue))
                throw new DistrictProtocolAuthenticationException(string.Format(NoLookupValueErrorMessageFormat, username, staffInfoLookupKey)) { Name = username };

            var staffUSIs = (from staffInfo in CreateDynamicWhere(_staffInformationRepository.GetAll(), staffInfoLookupValue, staffInfoLookupKey)
                            select new 
                            {
                                staffInfo.StaffUSI
                            }).ToList();

            if (staffUSIs.Count() == 0)
                throw new DashboardsMissingStaffAuthenticationException(string.Format(NoStaffUSIAssociationErrorMessageFormat, username)) { Name = username };

            if (staffUSIs.Count() > 1)
                throw new DashboardsMultipleStaffAuthenticationException(string.Format(MultipleStaffUSIAssociationErrorMessageFormat, username)) { Name = username };

            var staffUSI = staffUSIs.First().StaffUSI;

            if (!_usernamesByStaffUSI.ContainsKey(staffUSI.ToString()))
                _usernamesByStaffUSI[staffUSI.ToString()] = username;

            return staffUSI;
        }

        public string ResolveUsername(IAuthenticationProvider authenticationProvider, string staffUSI, int userType) // VIN22012016
        {
            if (!_usernamesByStaffUSI.ContainsKey(staffUSI))
            {
                //get the staff info key
                var staffInfoLookupKey = _staffInformationLookupKeyProvider.GetStaffInformationLookupKey();

                //using the staffUSI get the StaffInformation object and grab the staff info Key's value out of it
                var staffInformations = (from staffInfo in _staffInformationRepository.GetAll()
                            .Where(x => x.StaffUSI == Convert.ToInt32(staffUSI))
                            select staffInfo).ToList();

                //if not equal to 1 return null
                if (staffInformations.Count() != 1)
					throw new InvalidOperationException(String.Format("No staff information with StaffUSI of {0} was found.", staffUSI));

                //using reflection get the value of the staffInfoKey's value
                var staffInformation = staffInformations.First();
                var staffInformationProperty = staffInformation.GetType().GetProperty(staffInfoLookupKey);
                var lookupValueObj = staffInformationProperty.GetValue(staffInformation, null);

                //having to do this as StaffUSI as string is still null even if there is a value
                //  and I can't ToString a null object
                if (lookupValueObj == null)
					throw new InvalidOperationException(String.Format("StaffUSI {0} has a null value for property {1}. This is required to resolve the user.", staffUSI, staffInfoLookupKey));

                var lookupValue = lookupValueObj.ToString();

                if (string.IsNullOrEmpty(lookupValue))
					throw new InvalidOperationException(String.Format("StaffUSI {0} has a empty string value for property {1}. This is required to resolve the user.", staffUSI, staffInfoLookupKey));

                //pass that value into Resolve lookup Value to Username
                //Get back the username (if there is one)...if not one return null
                var username = authenticationProvider.ResolveLookupValueToUsername(lookupValue);

                if (string.IsNullOrEmpty(username))
					throw new InvalidOperationException(String.Format("StaffUSI {0} has a value of '{2}' for property {1}. However, the value '{2}' did not resolve to any user names through the authentication provider.", staffUSI, staffInfoLookupKey, lookupValue));

                _usernamesByStaffUSI[staffUSI] = username;
            }

            return _usernamesByStaffUSI[staffUSI];
        }

        #region Private Methods
        private static IQueryable<T1> CreateDynamicWhere<T1, T2>(IQueryable<T1> queryResult, T2 propertyValue, string propertyName)
        {
            var runtimeGenericType = queryResult.GetType().GetGenericArguments()[0];
            // Compose the expression tree that represents the parameter to the predicate.
            var pe = Expression.Parameter(runtimeGenericType, runtimeGenericType.Name);
            var left = Expression.Property(pe, runtimeGenericType.GetProperty(propertyName));
            object pv = Convert.ChangeType(propertyValue, left.Type);
            var right = Expression.Constant(pv, left.Type);
            var expression = Expression.Equal(left, right);

            // Create an expression tree that represents the expression
            var whereCallExpression = Expression.Call(
                typeof(Queryable),
                "Where",
                new Type[] { runtimeGenericType },
                queryResult.Expression,
                Expression.Lambda(expression, new[] { pe }));

            return queryResult.Provider.CreateQuery<T1>(whereCallExpression);
        }
        #endregion
    }
}