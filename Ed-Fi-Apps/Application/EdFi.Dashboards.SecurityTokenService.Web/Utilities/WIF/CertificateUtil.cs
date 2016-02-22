// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
//-----------------------------------------------------------------------------
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A
// PARTICULAR PURPOSE.
//
// Copyright (c) Microsoft Corporation. All rights reserved.
//
//
//-----------------------------------------------------------------------------

using System;
using System.Security.Cryptography.X509Certificates;

/// <summary>
/// A utility class which helps to retrieve an x509 certificate
/// </summary>
public class CertificateUtil
{
    public static X509Certificate2 GetCertificate( StoreName name, StoreLocation location, string certLookupValue )
    {
        var store = new X509Store( name, location );
        X509Certificate2Collection certificates = null;
        store.Open( OpenFlags.ReadOnly );

        try
        {
            X509Certificate2 result = null;

            //
            // Every time we call store.Certificates property, a new collection will be returned.
            //
            certificates = store.Certificates;

            //Try to match based on thumbprint first.
            foreach (X509Certificate2 cert in certificates)
            {
                if (cert.Thumbprint != null && cert.Thumbprint.ToLower() == certLookupValue.ToLower())
                {
                    if (result != null)
                    {
						throw new InvalidOperationException(string.Format("There are multiple certificates for subject Name {0}", certLookupValue));
                    }

                    result = new X509Certificate2(cert);
                }
            }
            //If nothing was matched...try matching on the subjectname.
            if (result == null)
            {
                foreach (X509Certificate2 cert in certificates)
                {
                    if (cert.SubjectName.Name != null && cert.SubjectName.Name.ToLower() == certLookupValue.ToLower())
                    {
                        if (result != null)
                        {
							throw new InvalidOperationException(
                                string.Format("There are multiple certificates for subject Name {0}", certLookupValue));
                        }

                        result = new X509Certificate2(cert);
                    }
                }
            }
            //If we still didn't find anything...
            if ( result == null )
            {
				throw new InvalidOperationException(string.Format("No certificate was found for subject Name {0}", certLookupValue));
            }

            return result;
        }
        finally
        {
            if ( certificates != null )
            {
                foreach (X509Certificate2 cert in certificates)
                {
                    cert.Reset();
                }
            }

            store.Close();
        }
    }
}

