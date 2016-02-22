using System;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using System.Net;
using System.Text;
using System.IO;
using System.Web.Configuration;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Resources.Application;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
    public class QuadraLMSTokenAuthenticationProvider : IAuthenticationProvider
    {
        public static readonly string IncorrectPassword = "wrong-password";
        public static readonly string ErrorPassword = "error-password";

        private readonly string emailDomain;
        public string UserName { get; set; } // VIN25112015        

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadraLMSAuthenticationProvider"/> class.
        /// </summary>
        public QuadraLMSTokenAuthenticationProvider() : this("domain.com") { }

        /// <summary>
        /// Initializes a new instance of the <see cref="QuadraLMSAuthenticationProvider"/> class using the 
        /// specified email domain name.
        /// </summary>
        /// <param name="emailDomain"></param>
        public QuadraLMSTokenAuthenticationProvider(string emailDomain)
        {
            this.emailDomain = emailDomain;                        
        }

        /// <summary>
        /// Attempts to validate the user's credentials with the username and password provided.
        /// </summary>
        /// <param name="username">The username of the user.</param>
        /// <param name="password">The password of the user.</param>
        /// <returns><b>true</b> if authentication is successful; otherwise <b>false</b>.</returns>
        public bool ValidateUser(string username, string password)
        {
            //// Make user type something, so authentication failure condition can be seen.
            //if (string.IsNullOrEmpty(password) || password == IncorrectPassword)
            //    return false;

            //if (password == ErrorPassword)
            //    throw new ArgumentOutOfRangeException("password", "Error password was provided.");

            //return true;

            //var json = client.MakeRequest("?username=" + username + "&password=" + password + "&service=moodle_mobile_app");

            string endPoint = Convert.ToString(WebConfigurationManager.AppSettings["MoodleTokenAuthendpoint"]); //@"http://demo.qforedu.com/login/token.php";            
            var client = new RestClient(endpoint: endPoint, method: HttpVerb.GET);
            var result = client.MakeRequest("&wsfunction=local_edfi_validate_token&token=" + password);

            //Token Service Not found alert : Saravanan
            if (result.ToUpper().Contains("INVALIDTOKEN"))
            {
                throw new Exception(result);
            }

            System.Xml.XmlDocument xDoc = new System.Xml.XmlDocument();
            xDoc.LoadXml(result);

            if (xDoc.SelectNodes("RESPONSE/VALUE").Item(0).InnerText.ToLower() == "invalid")
                return false;
            else
            {
                UserName = xDoc.SelectNodes("RESPONSE/VALUE").Item(0).InnerText.ToString();

                return true;
            }
            /*var newjson = json;

            if (String.Equals(json.Split(':')[0].Split('\"')[1], "token") && !String.IsNullOrEmpty(json.Split(':')[0].Split('\"')[1]))
                return true;
            else
                return false;*/

        }


        public string ResolveUsernameToLookupValue(string username, string staffInfoLookupKey)
        {
            switch (staffInfoLookupKey.ToUpper())
            {
                case "EMAILADDRESS":
                    return username + "@" + emailDomain;

                default:
                    return username;
            }
        }

        public string ResolveLookupValueToUsername(string lookupValue)
        {
            return lookupValue.IndexOf('@') == -1 ? lookupValue : lookupValue.Remove(lookupValue.IndexOf('@'));
        }
    }
}
