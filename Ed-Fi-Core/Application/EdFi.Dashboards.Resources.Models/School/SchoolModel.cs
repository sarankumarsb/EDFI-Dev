using System;

namespace EdFi.Dashboards.Resources.Models.School
{
    [Serializable]
    public class SchoolModel
    {
        public int SchoolId { get; set; }
        public string Name { get; set; }
        public string SchoolCategory { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string TelephoneNumber { get; set; }
        public string FaxNumber { get; set; }
        public string ProfileThumbnail { get; set; }
        public string WebSite { get; set; }
    }
}
