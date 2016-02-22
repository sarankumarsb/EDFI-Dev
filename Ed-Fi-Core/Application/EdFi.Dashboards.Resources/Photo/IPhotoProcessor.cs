// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Photo
{
    public class PhotoProcessorRequest
    {
        public int LocalEducationAgencyId { get; set; }
        public int SchoolId { get; set; }
        public byte[] FileBytes { get; set; }
    }

    public class PhotoProcessorResponse
    {
        public int TotalRecords { get; set; }
        public int SuccessfullyProcessedPhotos { get; set; }
        public List<string> ErrorMessages { get; private set; }

        public PhotoProcessorResponse()
        {
            ErrorMessages = new List<string>();
        }
    }

    public interface IPhotoProcessor
    {
        PhotoProcessorResponse Process(PhotoProcessorRequest photoProcessorRequest);
    }
}
