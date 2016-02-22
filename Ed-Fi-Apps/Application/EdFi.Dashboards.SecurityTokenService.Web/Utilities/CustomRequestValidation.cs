using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Util;

namespace EdFi.Dashboards.SecurityTokenService.Web.Utilities
{
    /// <summary>
    /// Summary description for CustomRequestValidation
    /// </summary>
    public class CustomRequestValidation : RequestValidator
    {
        protected override bool IsValidRequestString(HttpContext context, string value, RequestValidationSource requestValidationSource, string collectionKey, out int validationFailureIndex)
        {
            // if the user has selected some funky characters for their password, we don't want to keep them from logging in.
            if (requestValidationSource == RequestValidationSource.Form && collectionKey == "password")
            {
                validationFailureIndex = 0;
                return true;
            }

            return base.IsValidRequestString(context, value, requestValidationSource, collectionKey, out validationFailureIndex);
        }
    }
}