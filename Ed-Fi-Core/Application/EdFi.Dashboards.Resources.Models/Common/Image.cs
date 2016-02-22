// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;

namespace EdFi.Dashboards.Resources.Models.Common
{
    [Serializable]
    public class ImageModel
    {
        public byte[] Bytes { get; set; }
        public string ContentType { get; set; }
    }
}
