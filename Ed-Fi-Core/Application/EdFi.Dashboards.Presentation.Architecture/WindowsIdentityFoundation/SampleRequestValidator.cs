// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Web;
using System.Web.Util;
using Microsoft.IdentityModel.Protocols.WSFederation;

namespace EdFi.Dashboards.Presentation.Architecture.WindowsIdentityFoundation
{
	//-----------------------------------------------------------------------------
	//
	// THIS CODE AND INFORMATION IS PROVIDED 'AS IS' WITHOUT WARRANTY OF
	// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
	// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
	// PARTICULAR PURPOSE.
	//
	// Copyright (c) Microsoft Corporation. All rights reserved.
	//
	//
	//-----------------------------------------------------------------------------
    
    /// <summary>
	/// This SampleRequestValidator validates the result parameter of the
	/// WS-Federation passive protocol by checking for a SignInResponse message
	/// in the form post. The SignInResponse message contents are verified later by
	/// the WSFederationPassiveAuthenticationModule or the WIF sign in controls.
	/// </summary>
	public class SampleRequestValidator : RequestValidator
	{
		protected override bool IsValidRequestString(HttpContext context, string value, RequestValidationSource requestValidationSource, string collectionKey, out int validationFailureIndex)
		{
			validationFailureIndex = 0;

			if (requestValidationSource == RequestValidationSource.Form && !String.IsNullOrEmpty(collectionKey) && collectionKey.Equals(WSFederationConstants.Parameters.Result, StringComparison.Ordinal))
			{
                var unvalidatedFormValues = System.Web.Helpers.Validation.Unvalidated(context.Request).Form;
                var message = WSFederationMessage.CreateFromNameValueCollection(WSFederationMessage.GetBaseUrl(context.Request.Url), unvalidatedFormValues) as SignInResponseMessage;

				if (message != null)
				{
					return true;
				}
			}

			return base.IsValidRequestString(context, value, requestValidationSource, collectionKey, out validationFailureIndex);
		}
	}
}