using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using Rhino.Mocks.Constraints;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;

namespace EdFi.Dashboards.Resources.Security
{
    /// <summary>
    /// Typically used when overriding WSFederationAuthenticationModule_RedirectingToIdentityProvider in global.asax.cs.
    /// </summary>
    public interface ISignInRequestMessageProvider
    {
        void Adorn(Microsoft.IdentityModel.Protocols.WSFederation.SignInRequestMessage signInRequestMessage, ISignInRequestAdornModel signInRequestAdornModel);
    }

    public interface ISignInRequestAdornModel
    {
        int LocalEducationAgencyId { get; set; }
        string LocalEducationAgencyCode { get; set; }
        string LocalEducationAgencyName { get; set; }
        string Wimp { get; set; }
        string idofuser { get; set; } // VINLOGINOUT
        string idoftoken { get; set; } // VINLOGINOUT
    }

    public class SignInRequestAdornModel : ISignInRequestAdornModel
    {
        public int LocalEducationAgencyId { get; set; }
        public string LocalEducationAgencyCode { get; set; }
        public string LocalEducationAgencyName { get; set; }
        public string Wimp { get; set; }
        public string idofuser { get; set; } // VINLOGINOUT
        public string idoftoken { get; set; } // VINLOGINOUT

        private static SignInRequestAdornModel _empty;
        public static SignInRequestAdornModel Empty
        {
            get
            {
                if( _empty == null)
                    _empty = new SignInRequestAdornModel();
                return _empty;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var objAs = obj as SignInRequestAdornModel;

            if (objAs == null)
                return false;

            return objAs.GetHashCode() == this.GetHashCode();
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override string ToString()
        {
            return this.ToUrlQuery();
        }
    }

    public static class SignInRequestAdornUtility
    {
        public static readonly string Lea_Id = "LocalEducationAgencyId";
        public static readonly string Lea_Code = "LocalEducationAgencyCode";
        public static readonly string Lea_Name = "LocalEducationAgencyName";
        public static readonly string Wimp = "wimp";
        public static readonly string Idofuser = "idofuser"; // VINLOGINOUT
        public static readonly string Idoftoken = "idoftoken"; // VINLOGINOUT

        public static string ToUrlQuery(this ISignInRequestAdornModel signInRequestAdornModel)
        {
            var sb = new StringBuilder();
            using (var sw = new StringWriter(sb))
            {
                if( signInRequestAdornModel.LocalEducationAgencyId > 0)
                    sw.Write(string.Format("{0}={1}&", Lea_Id, signInRequestAdornModel.LocalEducationAgencyId));
                if( !string.IsNullOrEmpty(signInRequestAdornModel.LocalEducationAgencyCode))
                    sw.Write(string.Format("{0}={1}&", Lea_Code, signInRequestAdornModel.LocalEducationAgencyCode));
                if (!string.IsNullOrEmpty(signInRequestAdornModel.LocalEducationAgencyName))
                    sw.Write(string.Format("{0}={1}&", Lea_Name, signInRequestAdornModel.LocalEducationAgencyName));
                if (!string.IsNullOrEmpty(signInRequestAdornModel.Wimp))
                    sw.Write(string.Format("{0}={1}&", Wimp, signInRequestAdornModel.Wimp));
                // VINLOGINOUT
                if (!string.IsNullOrEmpty(signInRequestAdornModel.idofuser))
                    sw.Write(string.Format("{0}={1}&", Idofuser, signInRequestAdornModel.idofuser));
                // VINLOGINOUT
                if (!string.IsNullOrEmpty(signInRequestAdornModel.idofuser))
                    sw.Write(string.Format("{0}={1}&", Idoftoken, signInRequestAdornModel.idoftoken));
            }
            var url = sb.ToString();
            return url.Length > 0 ? url.Substring(0, url.Length - 1) : url;//Trim the very end
        }

        public static ISignInRequestAdornModel FromUrlQuery(string url)
        {
            int iqs = url.IndexOf('?');
            NameValueCollection variables;

            if (iqs <= 0)
            {
                if (url.IndexOf("=") <= 0)
                    return SignInRequestAdornModel.Empty;

                variables = HttpUtility.ParseQueryString(url);
            }

            var querystring = (iqs < url.Length - 1) ? url.Substring(iqs + 1) : String.Empty;
            variables = HttpUtility.ParseQueryString(querystring);
            var modelToReturn = new SignInRequestAdornModel();

            int leaId;
            string temp;
            if (!string.IsNullOrEmpty(variables[Lea_Id]) && int.TryParse(variables[Lea_Id], out leaId))
                modelToReturn.LocalEducationAgencyId = leaId;
            if ((temp = variables[Lea_Code]) != null)
                modelToReturn.LocalEducationAgencyCode = temp;
            if ((temp = variables[Lea_Name]) != null)
                modelToReturn.LocalEducationAgencyName = temp;
            if ((temp = variables[Wimp]) != null)
                modelToReturn.Wimp = temp;
            // VINLOGINOUT
            if ((temp = variables[Idofuser]) != null)
                modelToReturn.idofuser = temp;
            // VINLOGINOUT
            if ((temp = variables[Idoftoken]) != null)
                modelToReturn.idoftoken = temp;

            return modelToReturn;
        }
    }
}
