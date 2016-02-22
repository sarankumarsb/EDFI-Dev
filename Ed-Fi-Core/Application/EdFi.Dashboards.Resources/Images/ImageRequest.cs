using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Images
{
    //Base Request
    public abstract class ImageRequestBase
    {
        public string DisplayType { get; set; }
    }

    //Student Request
    public class StudentSchoolImageRequest : ImageRequestBase
    {
        public int LocalEducationAgencyId { get; set; }
        public int SchoolId { get; set; }
        public long StudentUSI { get; set; }
        public string Gender { get; set; }
    }

    //Staff Request
    public class StaffSchoolImageRequest : ImageRequestBase
    {
        public int SchoolId { get; set; }
        public long StaffUSI { get; set; }
        public string Gender { get; set; }
    }

    //School Request
    public class SchoolImageRequest : ImageRequestBase
    {
        public int SchoolId { get; set; }
    }

    //Local Education Agency Request
    public class LocalEducationAgencyImageRequest : ImageRequestBase
    {
        public int LocalEducationAgencyId { get; set; }
    }
}
