// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using EdFi.Dashboards.Resources.Photo.Models;

namespace EdFi.Dashboards.Resources.Photo
{
    public class IdentifierRequest
    {
        public int LocalEducationAgencyId { get; set; }
        public int SchoolId { get; set; }
        public OriginalPhoto OriginalPhoto { get; set; }
    }

    public interface IIdentifierProvider
    {
        Identifier Get(IdentifierRequest identifierRequest);
    }

    public class NullIdentifierProvider : IIdentifierProvider
    {
        public int LocalEducationAgencyId { get; set; }
        public int SchoolId { get; set; }

        public Identifier Get(IdentifierRequest identifierRequest)
        {
            throw new NotImplementedException(string.Format("Unhandled request {0}", identifierRequest.OriginalPhoto.GetType().Name));
        }
    }
}
