


using System;
using System.Linq;
using SubSonic.SqlGeneration.Schema;


namespace EdFi.Dashboards.Application.Data.Entities
{
    /// <summary>
    /// A class which represents the dashboard.BalancingGroup table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[BalancingGroup")]
    public class BalancingGroup 
    {                          
		 
        public BalancingGroup() 
        {
        
        }      
        
        private string _keyName = "BalancingGroupId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.BalancingGroupId;
        }  
        
        public static string GetKeyColumn()
        {
            return "BalancingGroupId";
        }         
			                
		[SubSonicPrimaryKey]        
        public int BalancingGroupId {get; set;}
          
        public string BalancingGroupName {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the dashboard.ClaimSetMapping table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[ClaimSetMapping")]
    public class ClaimSetMapping 
    {                          
		 
        public ClaimSetMapping() 
        {
        
        }      
        
        private string _keyName = "ClaimSetMappingId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.ClaimSetMappingId;
        }  
        
        public static string GetKeyColumn()
        {
            return "ClaimSetMappingId";
        }         
			                
		[SubSonicPrimaryKey]        
        public int ClaimSetMappingId {get; set;}
          
        public int LocalEducationAgencyId {get; set;}
          
        public string PositionTitle {get; set;}
          
        public string ClaimSet {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the dashboard.EducationOrganizationGoal table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[EducationOrganizationGoal")]
    public class EducationOrganizationGoal 
    {                          
		 
        public EducationOrganizationGoal() 
        {
        
        }      
        
        private string _keyName = "EducationOrganizationGoalId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.EducationOrganizationGoalId;
        }  
        
        public static string GetKeyColumn()
        {
            return "EducationOrganizationGoalId";
        }         
			                
		[SubSonicPrimaryKey]        
        public int EducationOrganizationGoalId {get; set;}
          
        public int EducationOrganizationId {get; set;}
          
        public int MetricId {get; set;}
          
        public decimal Goal {get; set;}
          
        public bool IsUpdated {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the dashboard.EducationOrganizationGoalPlanning table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[EducationOrganizationGoalPlanning")]
    public class EducationOrganizationGoalPlanning 
    {                          
		 
        public EducationOrganizationGoalPlanning() 
        {
        
        }      
        
        private string _keyName = "EducationOrganizationGoalPlanningId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.EducationOrganizationGoalPlanningId;
        }  
        
        public static string GetKeyColumn()
        {
            return "EducationOrganizationGoalPlanningId";
        }         
			                
		[SubSonicPrimaryKey]        
        public int EducationOrganizationGoalPlanningId {get; set;}
          
        public int EducationOrganizationId {get; set;}
          
        public int MetricId {get; set;}
          
        public decimal Goal {get; set;}
         
    }


    /// <summary>
    /// A class which represents the dashboard.LocalEducationAgency table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[LocalEducationAgency")]
    public class LocalEducationAgency 
    {                          
		 
        public LocalEducationAgency() 
        {
        
        }      
        
        private string _keyName = "LocalEducationAgencyId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.LocalEducationAgencyId;
        }  
        
        public static string GetKeyColumn()
        {
            return "LocalEducationAgencyId";
        }         
			                
		[SubSonicPrimaryKey]        
        public int LocalEducationAgencyId {get; set;}
          
        public string Code {get; set;}
          
        public string Name {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the dashboard.LocalEducationAgencyAdministration table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[LocalEducationAgencyAdministration")]
    public class LocalEducationAgencyAdministration 
    {                          
		 
        public LocalEducationAgencyAdministration() 
        {
        
        }      
        
        private string _keyName = "LocalEducationAgencyId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.LocalEducationAgencyId;
        }  
        
        public static string GetKeyColumn()
        {
            return "LocalEducationAgencyId";
        }         
			                
		[SubSonicPrimaryKey]        
        public int LocalEducationAgencyId {get; set;}
          
        public bool? IsKillSwitchActivated {get; set;}
          
        public string SystemMessage {get; set;}
          
        public string DefaultClassroomView {get; set;}

    } 
    
    
    /// <summary>
    /// A class which represents the dashboard.LocalEducationAgencyAuthentication table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[LocalEducationAgencyAuthentication")]
    public class LocalEducationAgencyAuthentication 
    {                          
		 
        public LocalEducationAgencyAuthentication() 
        {
        
        }      
        
        private string _keyName = "LocalEducationAgencyId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.LocalEducationAgencyId;
        }  
        
        public static string GetKeyColumn()
        {
            return "LocalEducationAgencyId";
        }         
			                
		[SubSonicPrimaryKey]        
        public int LocalEducationAgencyId {get; set;}
          
        public string StaffInformationLookupKey {get; set;}
          
        public string LdapLookupKey {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the dashboard.LocalEducationAgencyBalancingGroupAssociation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[LocalEducationAgencyBalancingGroupAssociation")]
    public class LocalEducationAgencyBalancingGroupAssociation 
    {                          
		 
        public LocalEducationAgencyBalancingGroupAssociation() 
        {
        
        }      
        
        private string _keyName = "BalancingGroupId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.BalancingGroupId;
        }  
        
        public static string GetKeyColumn()
        {
            return "BalancingGroupId";
        }         
			        
        public int LocalEducationAgencyId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int BalancingGroupId {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the dashboard.LocalEducationAgencyGrade table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[LocalEducationAgencyGrade")]
    public class LocalEducationAgencyGrade 
    {                          
		 
        public LocalEducationAgencyGrade() 
        {
        
        }      
        
        private string _keyName = "LetterGrade";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.LetterGrade;
        }  
        
        public static string GetKeyColumn()
        {
            return "LetterGrade";
        }         
			        
        public int LocalEducationAgencyId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string LetterGrade {get; set;}
          
        public int? NumericGrade {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the dashboard.LocalEducationAgencyMetricThreshold table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[LocalEducationAgencyMetricThreshold")]
    public class LocalEducationAgencyMetricThreshold 
    {                          
		 
        public LocalEducationAgencyMetricThreshold() 
        {
        
        }      
        
        private string _keyName = "LocalEducationAgencyMetricThresholdId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.LocalEducationAgencyMetricThresholdId;
        }  
        
        public static string GetKeyColumn()
        {
            return "LocalEducationAgencyMetricThresholdId";
        }         
			                
		[SubSonicPrimaryKey]        
        public int LocalEducationAgencyMetricThresholdId {get; set;}
          
        public int LocalEducationAgencyId {get; set;}
          
        public int MetricId {get; set;}
          
        public decimal Threshold {get; set;}
          
        public bool IsInclusive {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the dashboard.LocalEducationAgencySchoolYear table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[LocalEducationAgencySchoolYear")]
    public class LocalEducationAgencySchoolYear 
    {                          
		 
        public LocalEducationAgencySchoolYear() 
        {
        
        }      
        
        private string _keyName = "LocalEducationAgencyId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.LocalEducationAgencyId;
        }  
        
        public static string GetKeyColumn()
        {
            return "LocalEducationAgencyId";
        }         
			                
		[SubSonicPrimaryKey]        
        public int LocalEducationAgencyId {get; set;}
          
        public short SchoolYear {get; set;}
          
        public DateTime EffectiveDate {get; set;}
          
        public string SchoolYearDescription {get; set;}
         
    } 
    
        /// <summary>
    /// A class which represents the dashboard.LocalEducationAgencySearch table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[LocalEducationAgencySearch")]
    public class LocalEducationAgencySearch 
    {                          
		 
        public LocalEducationAgencySearch() 
        {
        
        }      
        
        public string KeyName()
        {
            return "LocalEducationAgencySearchId";
        }

        public object KeyValue()
        {
			            return this.LocalEducationAgencySearchId;
        }  
        
        public static string GetKeyColumn()
        {
            return "LocalEducationAgencySearchId";
        }         
			                
		[SubSonicPrimaryKey]        
        public int LocalEducationAgencySearchId {get; set;}
          
        public int LocalEducationAgencyId {get; set;}
          
        public string SearchCategory {get; set;}
          
        public string SystemCode {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the dashboard.LocalEducationAgencySupport table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[LocalEducationAgencySupport")]
    public class LocalEducationAgencySupport 
    {                          
		 
        public LocalEducationAgencySupport() 
        {
        
        }      
        
        private string _keyName = "LocalEducationAgencyId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.LocalEducationAgencyId;
        }  
        
        public static string GetKeyColumn()
        {
            return "LocalEducationAgencyId";
        }         
			                
		[SubSonicPrimaryKey]        
        public int LocalEducationAgencyId {get; set;}
          
        public string SupportContact {get; set;}
          
        public string SupportEmail {get; set;}
          
        public string SupportPhone {get; set;}
          
        public string TrainingAndPlanningUrl {get; set;}
          
        public string TicketOrgDescription {get; set;}
          
        public string TicketAssignee {get; set;}
          
        public string TicketSecurityLevel {get; set;}
          
        public string ProjectName {get; set;}
          
        public string IssueType {get; set;}
          
        public string MissingSchoolName {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the dashboard.MetricBasedWatchList table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[MetricBasedWatchList")]
    public class MetricBasedWatchList 
    {                          
		 
        public MetricBasedWatchList() 
        {
        
        }      
        
        private string _keyName = "MetricBasedWatchListId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.MetricBasedWatchListId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricBasedWatchListId";
        }         
			                
		[SubSonicPrimaryKey]        
        public int MetricBasedWatchListId {get; set;}
          
        public long StaffUSI {get; set;}
          
        public int EducationOrganizationId {get; set;}
          
        public string WatchListName {get; set;}
          
        public string WatchListDescription {get; set;}
          
        public bool IsWatchListShared {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the dashboard.MetricBasedWatchListNotificationStudent table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[MetricBasedWatchListNotificationStudent")]
    public class MetricBasedWatchListNotificationStudent 
    {                          
		 
        public MetricBasedWatchListNotificationStudent() 
        {
        
        }      
        
        private string _keyName = "MetricBasedWatchListNotificationStudentId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.MetricBasedWatchListNotificationStudentId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricBasedWatchListNotificationStudentId";
        }         
			                
		[SubSonicPrimaryKey]        
        public long MetricBasedWatchListNotificationStudentId {get; set;}
          
        public int MetricBasedWatchListId {get; set;}
          
        public long StudentUSI {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the dashboard.MetricBasedWatchListOption table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[MetricBasedWatchListOption")]
    public class MetricBasedWatchListOption 
    {                          
		 
        public MetricBasedWatchListOption() 
        {
        
        }      
        
        private string _keyName = "MetricBasedWatchListOptionId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.MetricBasedWatchListOptionId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricBasedWatchListOptionId";
        }         
			                
		[SubSonicPrimaryKey]        
        public int MetricBasedWatchListOptionId {get; set;}
          
        public int MetricBasedWatchListId {get; set;}
          
        public string Name {get; set;}
          
        public string Value {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the dashboard.MetricBasedWatchListSelectedOption table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[MetricBasedWatchListSelectedOption")]
    public class MetricBasedWatchListSelectedOption 
    {                          
		 
        public MetricBasedWatchListSelectedOption() 
        {
        
        }      
        
        private string _keyName = "MetricBasedWatchListSelectedOptionId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.MetricBasedWatchListSelectedOptionId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricBasedWatchListSelectedOptionId";
        }         
			                
		[SubSonicPrimaryKey]        
        public int MetricBasedWatchListSelectedOptionId {get; set;}
          
        public int MetricBasedWatchListId {get; set;}
          
        public string Name {get; set;}
          
        public string Value {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the dashboard.StaffCustomStudentList table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[StaffCustomStudentList")]
    public class StaffCustomStudentList 
    {                          
		 
        public StaffCustomStudentList() 
        {
        
        }      
        
        private string _keyName = "StaffCustomStudentListId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.StaffCustomStudentListId;
        }  
        
        public static string GetKeyColumn()
        {
            return "StaffCustomStudentListId";
        }         
			                
		[SubSonicPrimaryKey]        
        public int StaffCustomStudentListId {get; set;}
          
        public long StaffUSI {get; set;}
          
        public int EducationOrganizationId {get; set;}
          
        public string CustomStudentListIdentifier {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the dashboard.StaffCustomStudentListStudent table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("dashboard].[StaffCustomStudentListStudent")]
    public class StaffCustomStudentListStudent 
    {                          
		 
        public StaffCustomStudentListStudent() 
        {
        
        }      
        
        private string _keyName = "StaffCustomStudentListStudentId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.StaffCustomStudentListStudentId;
        }  
        
        public static string GetKeyColumn()
        {
            return "StaffCustomStudentListStudentId";
        }         
			                
		[SubSonicPrimaryKey]        
        public int StaffCustomStudentListStudentId {get; set;}
          
        public int StaffCustomStudentListId {get; set;}
          
        public long StudentUSI {get; set;}
         
    } 
}
