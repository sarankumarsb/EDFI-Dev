// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
namespace EdFi.Dashboards.SecurityTokenService.Authentication
{
    /// <summary>
    /// Defines the claim types used by the EdFi dashboards application.
    /// </summary>
    public static class EdFiClaimTypes
    {
        // TODO: Deferred - Make sure federation metadata and web configs match these namespaces
        public const string _BaseNamespace = "http://edfi.org/dashboards/identity/claims/";
        public const string _OrgClaimNamespace = _BaseNamespace + "org/";

        public const string StaffUSI = _BaseNamespace + "staffUSI";								
        public const string LocalEducationAgencyId = _BaseNamespace + "localEducationAgencyId"; 
        public const string FullName = _BaseNamespace + "fullName";
        public const string ServiceType = _BaseNamespace + "serviceType";	// VINLOGINTYP	
        public const string UserType = _BaseNamespace + "userType";	// EDFIDB-139	

        public const string UserId = _BaseNamespace + "userId";	// VIN05112015
        public const string UserToken = _BaseNamespace + "userToken";	// VIN05112015

        public const string ViewAllMetrics = _OrgClaimNamespace + "viewAllMetrics";
        public const string ViewMyMetrics = _OrgClaimNamespace + "viewMyMetrics";
        public const string ViewAllStudents = _OrgClaimNamespace + "viewAllStudents";
        public const string ViewMyStudents = _OrgClaimNamespace + "viewMyStudents";
        public const string ViewAllTeachers = _OrgClaimNamespace + "viewAllTeachers";
        //public const string ViewMyTeachers = _OrgClaimNamespace + "viewMyTeachers"; // For future use?
		public const string ViewOperationalDashboard = _OrgClaimNamespace + "viewOperationalDashboard";
		public const string AdministerDashboard = _OrgClaimNamespace + "administerDashboard";
        public const string ManageGoals = _OrgClaimNamespace + "manageGoals";
        public const string ManageWatchList = _OrgClaimNamespace + "manageWatchList";
        //public const string ViewGoals = _OrgClaimNamespace + "viewGoals";
		//public const string ManageStudentDataAccess = _BaseNamespace + "administerStudentDataAccess"; // For future use?
	    public const string AccessOrganization = _OrgClaimNamespace + "AccessOrganization";
        public const string Impersonating = _BaseNamespace + "Impersonating";
    }

	public static class EdFiClaimProperties
	{
        public const string StateAgencyId = "stateAgencyId";
        public const string LocalEducationAgencyId = "localEducationAgencyId";
		public const string SchoolId = "schoolId";
		public const string EducationOrganizationName = "educationOrganizationName";
	}
}
