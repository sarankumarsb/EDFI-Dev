// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Security
{
    public interface IAuthorizationDelegate
    {
        /// <summary>
        /// Authorizes the current request.
        /// </summary>
        /// <param name="parameters"></param>
        void AuthorizeRequest(IEnumerable<ParameterInstance> parameters);
    }

    public class AuthorizationDelegateMap
    {
        private Dictionary<Type, IAuthorizationDelegate> authorizationDelegateDictionary = new Dictionary<Type, IAuthorizationDelegate>();

        public AuthorizationDelegateMap(IAuthorizationDelegate[] authorizationDelegates)
        {
            foreach (var authorizationDelegate in authorizationDelegates)
            {
                authorizationDelegateDictionary.Add(authorizationDelegate.GetType(), authorizationDelegate);
            }
        }

        public IAuthorizationDelegate GetAuthorizationDelegate(Type type)
        {
            return authorizationDelegateDictionary[type];
        }

        public Dictionary<Type, IAuthorizationDelegate> AuthorizationDelegateDictionary
        {
            get { return authorizationDelegateDictionary; }
        }
    }
}
