// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using System.Net;
using System.Text;
using System.IO;
using System.Web.Configuration;
using EdFi.Dashboards.Common.Utilities;

namespace EdFi.Dashboards.Resources.Security.Implementations
{
	public class QuadraLMSAuthenticationProvider : IAuthenticationProvider
	{
	    public static readonly string IncorrectPassword = "wrong-password";
	    public static readonly string ErrorPassword = "error-password";

		private readonly string emailDomain;
        public string UserName { get; set; } // VIN25112015
		/// <summary>
        /// Initializes a new instance of the <see cref="QuadraLMSAuthenticationProvider"/> class.
		/// </summary>
        public QuadraLMSAuthenticationProvider() : this("domain.com") { }

	    /// <summary>
        /// Initializes a new instance of the <see cref="QuadraLMSAuthenticationProvider"/> class using the 
	    /// specified email domain name.
	    /// </summary>
	    /// <param name="emailDomain"></param>
        public QuadraLMSAuthenticationProvider(string emailDomain)
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

            string endPoint = Convert.ToString(WebConfigurationManager.AppSettings["MoodleAuthendpoint"]); //@"http://demo.qforedu.com/login/token.php";                        
            string postData = "username=" + username + "&password=" + password + "&service=moodle_mobile_app"; // Fix : EDFIDB-136
            var client = new RestClient(endpoint: endPoint, method: HttpVerb.POST, contenttype:"application/x-www-form-urlencoded", postData: postData);
            //var json = client.MakeRequest("?username=" + username + "&password=" + password + "&service=moodle_mobile_app");
            var json = client.MakeRequest();
            var newjson = json;

            if (String.Equals(json.Split(':')[0].Split('\"')[1], "token") && !String.IsNullOrEmpty(json.Split(':')[0].Split('\"')[1]))
                return true;
            else
                return false;

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

    public enum HttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    public class RestClient
    {
        public string EndPoint { get; set; }
        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }

        public RestClient()
        {
            EndPoint = "";
            Method = HttpVerb.GET;
            ContentType = "text/xml";
            PostData = "";
        }
        public RestClient(string endpoint)
        {
            EndPoint = endpoint;
            Method = HttpVerb.GET;
            ContentType = "text/xml";
            PostData = "";
        }
        public RestClient(string endpoint, HttpVerb method)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = "text/xml";
            PostData = "";
        }

        // Fix : EDFIDB-136
        public RestClient(string endpoint, HttpVerb method, string contenttype, string postData)
        {
            EndPoint = endpoint;
            Method = method;
            ContentType = contenttype; // "application/json; charset=utf-8"; // "text/xml";
            PostData = postData;
        }


        public string MakeRequest()
        {
            return MakeRequest("");
        }

        public string MakeRequest(string parameters)
        {
            var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);

            request.Method = Method.ToString();
            request.ContentLength = 0;
            request.ContentType = ContentType;                        

            if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST)
            {
                var encoding = new UTF8Encoding();
                var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
                request.ContentLength = bytes.Length;

                using (var writeStream = request.GetRequestStream())
                {
                    writeStream.Write(bytes, 0, bytes.Length);
                }
            }

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                var responseValue = string.Empty;

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                    throw new ApplicationException(message);
                }

                // grab the response
                using (var responseStream = response.GetResponseStream())
                {
                    if (responseStream != null)
                        using (var reader = new StreamReader(responseStream))
                        {
                            responseValue = reader.ReadToEnd();
                        }
                }

                return responseValue;
            }
        }

    } // class




}
