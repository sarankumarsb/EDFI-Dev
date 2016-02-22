// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
	public class TextFileAuthenticationProvider : IAuthenticationProvider
	{
		private readonly string credentialsFilePath;
        public string UserName { get; set; } // VIN25112015

		public enum AuthenticationInfo
		{
			Username,
			Password,
			EffectiveDate,
			ExpirationDate,
			UserEmailAddress,
			Roles
		}

		public TextFileAuthenticationProvider(string credentialsFilePath)
		{
			this.credentialsFilePath = credentialsFilePath;
		}

		

		private IEnumerable<string> GetAuthenticationFileLines()
		{
			if (!File.Exists(credentialsFilePath))
                throw new UserAccessDeniedException(string.Format("Security credentials file not found {0}.  Request execution has been aborted.", credentialsFilePath));

			return File.ReadAllLines(credentialsFilePath);
		}

        //public string GetEmailAddress(string username)
        //{
        //    IEnumerable<string> lines = GetAuthenticationFileLines();

        //    return
        //        (from parts in lines.Select(i => i.Split(','))
        //         where parts[(int) AuthInfo.Username].ToLower() == username.ToLower()
        //         select parts[(int) AuthInfo.UserEmailAddress])
        //            .SingleOrDefault();
        //}

        public string[] GetRoles(string email)
		{
			IEnumerable<string> lines = GetAuthenticationFileLines();

			var roles = 
				(from parts in lines.Select(i => i.Split(','))
                 where parts[(int)AuthenticationInfo.UserEmailAddress].ToLower() == email.ToLower()
				 select parts[(int)AuthenticationInfo.Roles])
					.SingleOrDefault();

            if(String.IsNullOrEmpty(roles))
                return new string[]{};

            //Special logic for School Leader
            if (roles.Contains("School Leader"))
            {
                roles = roles.Replace("School Leader", "Principal");
                roles += "|Teacher";
            }

            return roles.Split('|');
		}

        

        #region IAuthenticationProvider Methods
        public bool ValidateUser(string userName, string password)
        {
            IEnumerable<string> lines = GetAuthenticationFileLines();

            return
                (from parts in lines.Select(i => i.Split(',', (char)9))
                 where parts[(int)AuthenticationInfo.Username].ToLower() == userName.ToLower()
                       && parts[(int)AuthenticationInfo.Password] == password
                       && DateTime.Now > DateTime.Parse(parts[(int)AuthenticationInfo.EffectiveDate])
                       && DateTime.Now < DateTime.Parse(parts[(int)AuthenticationInfo.ExpirationDate])
                 select 0).Any();
        }

        //public string GetUserName(string emailAddress)
        //{
        //    return "Replace this";
        //}

        public string ResolveUsernameToLookupValue(string username, string staffInfoLookupKey)
        {
            IEnumerable<string> lines = GetAuthenticationFileLines();

            return
                (from parts in lines.Select(i => i.Split(','))
                 where parts[(int)AuthenticationInfo.Username].ToLower() == username.ToLower()
                 select parts[(int)AuthenticationInfo.UserEmailAddress])
                    .SingleOrDefault();
        }

        public string ResolveLookupValueToUsername(string lookupValue)
        {
            //tjm: this is here because when we renamed the methods this is all that GetUserName(string emailAddress) did
            return "Replace this";
        }
        #endregion
    }
}