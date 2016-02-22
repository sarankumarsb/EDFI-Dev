


using System;
using System.Data.Linq.Mapping;
using SubSonic.SqlGeneration.Schema;


namespace EdFi.Dashboards.Data.Entities
{
    
    
    /// <summary>
    /// A class which represents the domain.EdFiDashboardException table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[EdFiDashboardException")]
	[Table(Name = "[domain].[EdFiDashboardException]")]
    public class EdFiDashboardException
    {                           
        public EdFiDashboardException() 
        {
        
        }      
        
		private string _keyName = "EdFiExceptionId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.EdFiExceptionId;
        }  
        
        public static string GetKeyColumn()
        {
            return "EdFiExceptionId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int EdFiExceptionId {get; set;}
          
        public string TableName {get; set;}
          
        public string ColumnNameList {get; set;}
          
        public string ColumnValueList {get; set;}
          
        public string ErrorMessage {get; set;}
          
        public string IdentifierCondition {get; set;}
          
        public string LookupCondition {get; set;}
          
        public string ExceptionLevel {get; set;}
          
        public DateTime? StartTime {get; set;}
          
        public string PackageName {get; set;}
          
        public string TaskName {get; set;}
          
        public string ComponentName {get; set;}
          
        public int? ErrorCode {get; set;}
          
        public string ErrorDescription {get; set;}
          
        public int? ErrorColumn {get; set;}
          
        public string ErrorColumnName {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.EnhancedStudentInformation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[EnhancedStudentInformation")]
	[Table(Name = "[domain].[EnhancedStudentInformation]")]
    public class EnhancedStudentInformation
    {                           
        public EnhancedStudentInformation() 
        {
        
        }      
        
		private string _keyName = "StudentUSI";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.StudentUSI;
        }  
        
        public static string GetKeyColumn()
        {
            return "StudentUSI";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public long StudentUSI {get; set;}
          
        public string LastSurname {get; set;}
          
        public string FirstName {get; set;}
          
        public string MiddleName {get; set;}
          
        public string FullName {get; set;}
          
        public string ProfileThumbnail {get; set;}
          
        public string Gender {get; set;}
          
        public string Race {get; set;}
          
        public string HispanicLatinoEthnicity {get; set;}
          
        public int SchoolId {get; set;}
          
        public string GradeLevel {get; set;}
          
        public int GradeLevelSortOrder {get; set;}
          
        public string LateEnrollment {get; set;}
          
        public string SchoolName {get; set;}
          
        public string SchoolCategory {get; set;}
          
        public int LocalEducationAgencyId {get; set;}
          
        public string StudentUniqueID {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.LocalEducationAgencyAccountabilityByCategory table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[LocalEducationAgencyAccountabilityByCategory")]
	[Table(Name = "[domain].[LocalEducationAgencyAccountabilityByCategory]")]
    public class LocalEducationAgencyAccountabilityByCategory
    {                           
        public LocalEducationAgencyAccountabilityByCategory() 
        {
        
        }      
        
		private string _keyName = "Attribute";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.Attribute;
        }  
        
        public static string GetKeyColumn()
        {
            return "Attribute";
        }                                 
               
        
        public int LocalEducationAgencyId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string Attribute {get; set;}
          
        public string Value {get; set;}
          
        public int? DisplayOrder {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.LocalEducationAgencyAccountabilityInformation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[LocalEducationAgencyAccountabilityInformation")]
	[Table(Name = "[domain].[LocalEducationAgencyAccountabilityInformation]")]
    public class LocalEducationAgencyAccountabilityInformation
    {                           
        public LocalEducationAgencyAccountabilityInformation() 
        {
        
        }      
        
		private string _keyName = "Attribute";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.Attribute;
        }  
        
        public static string GetKeyColumn()
        {
            return "Attribute";
        }                                 
               
        
        public int LocalEducationAgencyId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string Attribute {get; set;}
          
        public string Value {get; set;}
          
        public int? DisplayOrder {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.LocalEducationAgencyAdministrationInformation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[LocalEducationAgencyAdministrationInformation")]
	[Table(Name = "[domain].[LocalEducationAgencyAdministrationInformation]")]
    public class LocalEducationAgencyAdministrationInformation
    {                           
        public LocalEducationAgencyAdministrationInformation() 
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
          
        public string Role {get; set;}
          
        public string Name {get; set;}
          
        public int DisplayOrder {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.LocalEducationAgencyCharacteristicsInformation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[LocalEducationAgencyCharacteristicsInformation")]
	[Table(Name = "[domain].[LocalEducationAgencyCharacteristicsInformation]")]
    public class LocalEducationAgencyCharacteristicsInformation
    {                           
        public LocalEducationAgencyCharacteristicsInformation() 
        {
        
        }      
        
		private string _keyName = "Attribute";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.Attribute;
        }  
        
        public static string GetKeyColumn()
        {
            return "Attribute";
        }                                 
               
        
        public int LocalEducationAgencyId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string Attribute {get; set;}
          
        public string Value {get; set;}
          
        public int? CharacteristicsId {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.LocalEducationAgencyIndicatorPopulation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[LocalEducationAgencyIndicatorPopulation")]
	[Table(Name = "[domain].[LocalEducationAgencyIndicatorPopulation]")]
    public class LocalEducationAgencyIndicatorPopulation
    {                           
        public LocalEducationAgencyIndicatorPopulation() 
        {
        
        }      
        
		private string _keyName = "Attribute";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.Attribute;
        }  
        
        public static string GetKeyColumn()
        {
            return "Attribute";
        }                                 
               
        
        public int LocalEducationAgencyId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string Attribute {get; set;}
          
        public decimal? Value {get; set;}
          
        public int? TrendDirection {get; set;}
          
        public int? DisplayOrder {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.LocalEducationAgencyInformation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[LocalEducationAgencyInformation")]
	[Table(Name = "[domain].[LocalEducationAgencyInformation]")]
    public class LocalEducationAgencyInformation
    {                           
        public LocalEducationAgencyInformation() 
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
          
        public string Name {get; set;}
          
        public string AddressLine1 {get; set;}
          
        public string AddressLine2 {get; set;}
          
        public string AddressLine3 {get; set;}
          
        public string City {get; set;}
          
        public string State {get; set;}
          
        public string ZipCode {get; set;}
          
        public string TelephoneNumber {get; set;}
          
        public string FaxNumber {get; set;}
          
        public string ProfileThumbnail {get; set;}
          
        public string WebSite {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.LocalEducationAgencyMetricInstanceSet table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[LocalEducationAgencyMetricInstanceSet")]
	[Table(Name = "[domain].[LocalEducationAgencyMetricInstanceSet]")]
    public class LocalEducationAgencyMetricInstanceSet
    {                           
        public LocalEducationAgencyMetricInstanceSet() 
        {
        
        }      
        
		private string _keyName = "MetricInstanceSetKey";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.MetricInstanceSetKey;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricInstanceSetKey";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public Guid MetricInstanceSetKey {get; set;}
          
        public int LocalEducationAgencyId {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.LocalEducationAgencyMetricSchoolList table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[LocalEducationAgencyMetricSchoolList")]
	[Table(Name = "[domain].[LocalEducationAgencyMetricSchoolList]")]
    public class LocalEducationAgencyMetricSchoolList
    {                           
        public LocalEducationAgencyMetricSchoolList() 
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
          
        public int MetricId {get; set;}
          
        public int SchoolId {get; set;}
          
        public string Value {get; set;}
          
        public string ValueType {get; set;}
          
        public long? StaffUSI {get; set;}
          
        public decimal SchoolGoal {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.LocalEducationAgencyProgramPopulation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[LocalEducationAgencyProgramPopulation")]
	[Table(Name = "[domain].[LocalEducationAgencyProgramPopulation]")]
    public class LocalEducationAgencyProgramPopulation
    {                           
        public LocalEducationAgencyProgramPopulation() 
        {
        
        }      
        
		private string _keyName = "Attribute";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.Attribute;
        }  
        
        public static string GetKeyColumn()
        {
            return "Attribute";
        }                                 
               
        
        public int LocalEducationAgencyId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string Attribute {get; set;}
          
        public decimal? Value {get; set;}
          
        public int? TrendDirection {get; set;}
          
        public int? DisplayOrder {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.LocalEducationAgencyStudentDemographic table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[LocalEducationAgencyStudentDemographic")]
	[Table(Name = "[domain].[LocalEducationAgencyStudentDemographic]")]
    public class LocalEducationAgencyStudentDemographic
    {                           
        public LocalEducationAgencyStudentDemographic() 
        {
        
        }      
        
		private string _keyName = "Attribute";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.Attribute;
        }  
        
        public static string GetKeyColumn()
        {
            return "Attribute";
        }                                 
               
        
        public int LocalEducationAgencyId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string Attribute {get; set;}
          
        public decimal? Value {get; set;}
          
        public int? TrendDirection {get; set;}
          
        public int? DisplayOrder {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.LoginDetails table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[LoginDetails")]
	[Table(Name = "[domain].[LoginDetails]")]
    public class LoginDetails
    {                           
        public LoginDetails() 
        {
        
        }      
        
		private string _keyName = "USI";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.USI;
        }  
        
        public static string GetKeyColumn()
        {
            return "USI";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int USI {get; set;}
          
        public string LoginId {get; set;}
          
        public int UserType {get; set;}
          
        public DateTime CreatedOn {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolAccountabilityInformation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolAccountabilityInformation")]
	[Table(Name = "[domain].[SchoolAccountabilityInformation]")]
    public class SchoolAccountabilityInformation
    {                           
        public SchoolAccountabilityInformation() 
        {
        
        }      
        
		private string _keyName = "Attribute";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.Attribute;
        }  
        
        public static string GetKeyColumn()
        {
            return "Attribute";
        }                                 
               
        
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string Attribute {get; set;}
          
        public string Value {get; set;}
          
        public int? DisplayOrder {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolAdministrationInformation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolAdministrationInformation")]
	[Table(Name = "[domain].[SchoolAdministrationInformation]")]
    public class SchoolAdministrationInformation
    {                           
        public SchoolAdministrationInformation() 
        {
        
        }      
        
		private string _keyName = "Name";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.Name;
        }  
        
        public static string GetKeyColumn()
        {
            return "Name";
        }                                 
               
        
        public int SchoolId {get; set;}
          
        public string Role {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string Name {get; set;}
          
        public int DisplayOrder {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolFeederSchool table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolFeederSchool")]
	[Table(Name = "[domain].[SchoolFeederSchool]")]
    public class SchoolFeederSchool
    {                           
        public SchoolFeederSchool() 
        {
        
        }      
        
		private string _keyName = "Attribute";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.Attribute;
        }  
        
        public static string GetKeyColumn()
        {
            return "Attribute";
        }                                 
               
        
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string Attribute {get; set;}
          
        public decimal? Value {get; set;}
          
        public int? TrendDirection {get; set;}
          
        public int? DisplayOrder {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolGradePopulation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolGradePopulation")]
	[Table(Name = "[domain].[SchoolGradePopulation]")]
    public class SchoolGradePopulation
    {                           
        public SchoolGradePopulation() 
        {
        
        }      
        
		private string _keyName = "Attribute";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.Attribute;
        }  
        
        public static string GetKeyColumn()
        {
            return "Attribute";
        }                                 
               
        
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string Attribute {get; set;}
          
        public decimal? LateEnrollment {get; set;}
          
        public decimal? Value {get; set;}
          
        public int? TrendDirection {get; set;}
          
        public int? DisplayOrder {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolIndicatorPopulation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolIndicatorPopulation")]
	[Table(Name = "[domain].[SchoolIndicatorPopulation]")]
    public class SchoolIndicatorPopulation
    {                           
        public SchoolIndicatorPopulation() 
        {
        
        }      
        
		private string _keyName = "Attribute";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.Attribute;
        }  
        
        public static string GetKeyColumn()
        {
            return "Attribute";
        }                                 
               
        
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string Attribute {get; set;}
          
        public decimal? Value {get; set;}
          
        public int? TrendDirection {get; set;}
          
        public int? DisplayOrder {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolInformation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolInformation")]
	[Table(Name = "[domain].[SchoolInformation]")]
    public class SchoolInformation
    {                           
        public SchoolInformation() 
        {
        
        }      
        
		private string _keyName = "SchoolId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.SchoolId;
        }  
        
        public static string GetKeyColumn()
        {
            return "SchoolId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int SchoolId {get; set;}
          
        public int LocalEducationAgencyId {get; set;}
          
        public string Name {get; set;}
          
        public string SchoolCategory {get; set;}
          
        public string AddressLine1 {get; set;}
          
        public string AddressLine2 {get; set;}
          
        public string AddressLine3 {get; set;}
          
        public string City {get; set;}
          
        public string State {get; set;}
          
        public string ZipCode {get; set;}
          
        public string TelephoneNumber {get; set;}
          
        public string FaxNumber {get; set;}
          
        public string ProfileThumbnail {get; set;}
          
        public string WebSite {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolMetricAssessmentRate table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolMetricAssessmentRate")]
	[Table(Name = "[domain].[SchoolMetricAssessmentRate]")]
    public class SchoolMetricAssessmentRate
    {                           
        public SchoolMetricAssessmentRate() 
        {
        
        }      
        
		private string _keyName = "GradeLevelTypeId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.GradeLevelTypeId;
        }  
        
        public static string GetKeyColumn()
        {
            return "GradeLevelTypeId";
        }                                 
               
        
        public int SchoolId {get; set;}
          
        public int MetricId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int GradeLevelTypeId {get; set;}
          
        public string GradeLevel {get; set;}
          
        public decimal? CommendedRate {get; set;}
          
        public decimal? MetStandardRate {get; set;}
          
        public decimal? BelowRate {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolMetricGradeDistribution table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolMetricGradeDistribution")]
	[Table(Name = "[domain].[SchoolMetricGradeDistribution]")]
    public class SchoolMetricGradeDistribution
    {                           
        public SchoolMetricGradeDistribution() 
        {
        
        }      
        
		private string _keyName = "GradeLevelTypeId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.GradeLevelTypeId;
        }  
        
        public static string GetKeyColumn()
        {
            return "GradeLevelTypeId";
        }                                 
               
        
        public int SchoolId {get; set;}
          
        public int MetricId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int GradeLevelTypeId {get; set;}
          
        public string GradeLevel {get; set;}
          
        public decimal? Numerator {get; set;}
          
        public int? Denominator {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolMetricInstanceSet table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolMetricInstanceSet")]
	[Table(Name = "[domain].[SchoolMetricInstanceSet]")]
    public class SchoolMetricInstanceSet
    {                           
        public SchoolMetricInstanceSet() 
        {
        
        }      
        
		private string _keyName = "MetricInstanceSetKey";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.MetricInstanceSetKey;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricInstanceSetKey";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public Guid MetricInstanceSetKey {get; set;}
          
        public int SchoolId {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolMetricStudentList table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolMetricStudentList")]
	[Table(Name = "[domain].[SchoolMetricStudentList]")]
    public class SchoolMetricStudentList
    {                           
        public SchoolMetricStudentList() 
        {
        
        }      
        
		private string _keyName = "MetricId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.MetricId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricId";
        }                                 
               
        
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int MetricId {get; set;}
          
        public long StudentUSI {get; set;}
          
        public string Value {get; set;}
          
        public string ValueType {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolMetricTeacherList table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolMetricTeacherList")]
	[Table(Name = "[domain].[SchoolMetricTeacherList]")]
    public class SchoolMetricTeacherList
    {                           
        public SchoolMetricTeacherList() 
        {
        
        }      
        
		private string _keyName = "MetricId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.MetricId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricId";
        }                                 
               
        
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int MetricId {get; set;}
          
        public long StaffUSI {get; set;}
          
        public string Value {get; set;}
          
        public string ValueType {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolPeerSchool table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolPeerSchool")]
	[Table(Name = "[domain].[SchoolPeerSchool]")]
    public class SchoolPeerSchool
    {                           
        public SchoolPeerSchool() 
        {
        
        }      
        
		private string _keyName = "Name";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.Name;
        }  
        
        public static string GetKeyColumn()
        {
            return "Name";
        }                                 
               
        
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string Name {get; set;}
          
        public string Location {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolProgramPopulation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolProgramPopulation")]
	[Table(Name = "[domain].[SchoolProgramPopulation]")]
    public class SchoolProgramPopulation
    {                           
        public SchoolProgramPopulation() 
        {
        
        }      
        
		private string _keyName = "Attribute";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.Attribute;
        }  
        
        public static string GetKeyColumn()
        {
            return "Attribute";
        }                                 
               
        
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string Attribute {get; set;}
          
        public decimal? Value {get; set;}
          
        public int? TrendDirection {get; set;}
          
        public int? DisplayOrder {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolSectionCalendar table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolSectionCalendar")]
	[Table(Name = "[domain].[SchoolSectionCalendar]")]
    public class SchoolSectionCalendar
    {                           
        public SchoolSectionCalendar() 
        {
        
        }      
        
		private string _keyName = "ClassPeriodName";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.ClassPeriodName;
        }  
        
        public static string GetKeyColumn()
        {
            return "ClassPeriodName";
        }                                 
               
        
        public int SchoolId {get; set;}
          
        public string TermType {get; set;}
          
        public string LocalCourseCode {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string ClassPeriodName {get; set;}
          
        public string ClassroomIdentificationCode {get; set;}
          
        public DateTime DateValue {get; set;}
          
        public string SubjectArea {get; set;}
          
        public string DayOfTheWeek {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolStudentDemographic table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolStudentDemographic")]
	[Table(Name = "[domain].[SchoolStudentDemographic]")]
    public class SchoolStudentDemographic
    {                           
        public SchoolStudentDemographic() 
        {
        
        }      
        
		private string _keyName = "Attribute";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.Attribute;
        }  
        
        public static string GetKeyColumn()
        {
            return "Attribute";
        }                                 
               
        
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string Attribute {get; set;}
          
        public decimal? Value {get; set;}
          
        public int? TrendDirection {get; set;}
          
        public int? DisplayOrder {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StaffCohort table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StaffCohort")]
	[Table(Name = "[domain].[StaffCohort]")]
    public class StaffCohort
    {                           
        public StaffCohort() 
        {
        
        }      
        
		private string _keyName = "StaffCohortId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.StaffCohortId;
        }  
        
        public static string GetKeyColumn()
        {
            return "StaffCohortId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public long StaffCohortId {get; set;}
          
        public long StaffUSI {get; set;}
          
        public int EducationOrganizationId {get; set;}
          
        public string CohortIdentifier {get; set;}
          
        public string CohortDescription {get; set;}
          
        public string CohortType {get; set;}
          
        public string CohortScopeType {get; set;}
          
        public string AcademicSubjectType {get; set;}
          
        public string ProgramType {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StaffEducationOrgInformation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StaffEducationOrgInformation")]
	[Table(Name = "[domain].[StaffEducationOrgInformation]")]
    public class StaffEducationOrgInformation
    {                           
        public StaffEducationOrgInformation() 
        {
        
        }      
        
		private string _keyName = "EducationOrganizationId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.EducationOrganizationId;
        }  
        
        public static string GetKeyColumn()
        {
            return "EducationOrganizationId";
        }                                 
               
        
        public long StaffUSI {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int EducationOrganizationId {get; set;}
          
        public string StaffCategory {get; set;}
          
        public string PositionTitle {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StaffIdentificationCode table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StaffIdentificationCode")]
	[Table(Name = "[domain].[StaffIdentificationCode]")]
    public class StaffIdentificationCode
    {                           
        public StaffIdentificationCode() 
        {
        
        }      
        
		private string _keyName = "StaffIdentificationSystemType";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.StaffIdentificationSystemType;
        }  
        
        public static string GetKeyColumn()
        {
            return "StaffIdentificationSystemType";
        }                                 
               
        
        public long StaffUSI {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string StaffIdentificationSystemType {get; set;}
          
        public string AssigningOrganizationCode {get; set;}
          
        public string IdentificationCode {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StaffInformation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StaffInformation")]
	[Table(Name = "[domain].[StaffInformation]")]
    public class StaffInformation
    {                           
        public StaffInformation() 
        {
        
        }      
        
		private string _keyName = "StaffUSI";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.StaffUSI;
        }  
        
        public static string GetKeyColumn()
        {
            return "StaffUSI";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public long StaffUSI {get; set;}
          
        public string FullName {get; set;}
          
        public string FirstName {get; set;}
          
        public string MiddleName {get; set;}
          
        public string LastSurname {get; set;}
          
        public string EmailAddress {get; set;}
          
        public DateTime? DateOfBirth {get; set;}
          
        public string Gender {get; set;}
          
        public string OldEthnicity {get; set;}
          
        public string HispanicLatinoEthnicity {get; set;}
          
        public string Race {get; set;}
          
        public string HighestLevelOfEducationCompleted {get; set;}
          
        public decimal? YearsOfPriorProfessionalExperience {get; set;}
          
        public bool? HighlyQualifiedTeacher {get; set;}
          
        public string ProfileThumbnail {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StaffSchoolMetricInstanceSet table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StaffSchoolMetricInstanceSet")]
	[Table(Name = "[domain].[StaffSchoolMetricInstanceSet]")]
    public class StaffSchoolMetricInstanceSet
    {                           
        public StaffSchoolMetricInstanceSet() 
        {
        
        }      
        
		private string _keyName = "MetricInstanceSetKey";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.MetricInstanceSetKey;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricInstanceSetKey";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public Guid MetricInstanceSetKey {get; set;}
          
        public long StaffUSI {get; set;}
          
        public int SchoolId {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StaffSectionCohortAssociation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StaffSectionCohortAssociation")]
	[Table(Name = "[domain].[StaffSectionCohortAssociation]")]
    public class StaffSectionCohortAssociation
    {                           
        public StaffSectionCohortAssociation() 
        {
        
        }      
        
		private string _keyName = "StaffUSI";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.StaffUSI;
        }  
        
        public static string GetKeyColumn()
        {
            return "StaffUSI";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public long StaffUSI {get; set;}
          
        public long SectionIdOrCohortId {get; set;}
          
        public int EducationOrganizationId {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StaffStudentAssociation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StaffStudentAssociation")]
	[Table(Name = "[domain].[StaffStudentAssociation]")]
    public class StaffStudentAssociation
    {                           
        public StaffStudentAssociation() 
        {
        
        }      
        
		private string _keyName = "StaffUSI";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.StaffUSI;
        }  
        
        public static string GetKeyColumn()
        {
            return "StaffUSI";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public long StaffUSI {get; set;}
          
        public long StudentUSI {get; set;}
          
        public int SchoolId {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StaffStudentCohort table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StaffStudentCohort")]
	[Table(Name = "[domain].[StaffStudentCohort]")]
    public class StaffStudentCohort
    {                           
        public StaffStudentCohort() 
        {
        
        }      
        
		private string _keyName = "StaffCohortId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.StaffCohortId;
        }  
        
        public static string GetKeyColumn()
        {
            return "StaffCohortId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public long StaffCohortId {get; set;}
          
        public long StudentUSI {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentAccommodationCountAndMaxValue table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentAccommodationCountAndMaxValue")]
	[Table(Name = "[domain].[StudentAccommodationCountAndMaxValue]")]
    public class StudentAccommodationCountAndMaxValue
    {                           
        public StudentAccommodationCountAndMaxValue() 
        {
        
        }      
        
		private string _keyName = "StudentUSI";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.StudentUSI;
        }  
        
        public static string GetKeyColumn()
        {
            return "StudentUSI";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public long StudentUSI {get; set;}
          
        public int SchoolId {get; set;}
          
        public int? AccomodationCount {get; set;}
          
        public int? MaxAccomodationValue {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentIdentificationCode table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentIdentificationCode")]
	[Table(Name = "[domain].[StudentIdentificationCode]")]
    public class StudentIdentificationCode
    {                           
        public StudentIdentificationCode() 
        {
        
        }      
        
		private string _keyName = "AssigningOrganizationCode";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.AssigningOrganizationCode;
        }  
        
        public static string GetKeyColumn()
        {
            return "AssigningOrganizationCode";
        }                                 
               
        
        public long StudentUSI {get; set;}
          
        public string StudentIdentificationSystemType {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string AssigningOrganizationCode {get; set;}
          
        public string IdentificationCode {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentIndicator table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentIndicator")]
	[Table(Name = "[domain].[StudentIndicator]")]
    public class StudentIndicator
    {                           
        public StudentIndicator() 
        {
        
        }      
        
		private string _keyName = "EducationOrganizationId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.EducationOrganizationId;
        }  
        
        public static string GetKeyColumn()
        {
            return "EducationOrganizationId";
        }                                 
               
        
        public long StudentUSI {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int EducationOrganizationId {get; set;}
          
        public string Type {get; set;}
          
        public string ParentName {get; set;}
          
        public string Name {get; set;}
          
        public bool Status {get; set;}
          
        public int? DisplayOrder {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentInformation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentInformation")]
	[Table(Name = "[domain].[StudentInformation]")]
    public class StudentInformation
    {                           
        public StudentInformation() 
        {
        
        }      
        
		private string _keyName = "StudentUSI";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.StudentUSI;
        }  
        
        public static string GetKeyColumn()
        {
            return "StudentUSI";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public long StudentUSI {get; set;}
          
        public string FullName {get; set;}
          
        public string FirstName {get; set;}
          
        public string MiddleName {get; set;}
          
        public string LastSurname {get; set;}
          
        public string AddressLine1 {get; set;}
          
        public string AddressLine2 {get; set;}
          
        public string AddressLine3 {get; set;}
          
        public string City {get; set;}
          
        public string State {get; set;}
          
        public string ZipCode {get; set;}
          
        public string TelephoneNumber {get; set;}
          
        public string EmailAddress {get; set;}
          
        public DateTime DateOfBirth {get; set;}
          
        public string PlaceOfBirth {get; set;}
          
        public int? CurrentAge {get; set;}
          
        public int? CohortYear {get; set;}
          
        public string Gender {get; set;}
          
        public string OldEthnicity {get; set;}
          
        public string HispanicLatinoEthnicity {get; set;}
          
        public string Race {get; set;}
          
        public string HomeLanguage {get; set;}
          
        public string Language {get; set;}
          
        public string ParentMilitary {get; set;}
          
        public string SingleParentPregnantTeen {get; set;}
          
        public string ProfileThumbnail {get; set;}
          
        public string StudentUniqueID {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentMetric table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetric")]
	[Table(Name = "[domain].[StudentMetric]")]
    public class StudentMetric
    {                           
        public StudentMetric() 
        {
        
        }      
        
		private string _keyName = "StudentUSI";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.StudentUSI;
        }  
        
        public static string GetKeyColumn()
        {
            return "StudentUSI";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public long StudentUSI {get; set;}
          
        public int SchoolId {get; set;}
          
        public Guid MetricInstanceSetKey {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public int? MetricStateTypeIdSortOrder {get; set;}
          
        public string Value {get; set;}
          
        public int? TrendDirection {get; set;}
          
        public string ValueTypeName {get; set;}
          
        public double? ValueSortOrder {get; set;}
          
        public int MetricId {get; set;}
          
        public int? TrendInterpretation {get; set;}
          
        public int MetricVariantId {get; set;}
          
        public string Format {get; set;}
          
        public int? IndicatorTypeId {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentMetricAbsencesByClass table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetricAbsencesByClass")]
	[Table(Name = "[domain].[StudentMetricAbsencesByClass]")]
    public class StudentMetricAbsencesByClass
    {                           
        public StudentMetricAbsencesByClass() 
        {
        
        }      
        
		private string _keyName = "StudentUSI";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.StudentUSI;
        }  
        
        public static string GetKeyColumn()
        {
            return "StudentUSI";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public long StudentUSI {get; set;}
          
        public int SchoolId {get; set;}
          
        public string SubjectArea {get; set;}
          
        public DateTime DateValue {get; set;}
          
        public string DayOfTheWeek {get; set;}
          
        public int? AttendanceEventDescriptorTypeId {get; set;}
          
        public string AttendanceEventReason {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentMetricAbsencesBySection table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetricAbsencesBySection")]
	[Table(Name = "[domain].[StudentMetricAbsencesBySection]")]
    public class StudentMetricAbsencesBySection
    {                           
        public StudentMetricAbsencesBySection() 
        {
        
        }      
        
		private string _keyName = "ClassPeriodName";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.ClassPeriodName;
        }  
        
        public static string GetKeyColumn()
        {
            return "ClassPeriodName";
        }                                 
               
        
        public long StudentUSI {get; set;}
          
        public int SchoolId {get; set;}
          
        public string TermType {get; set;}
          
        public string LocalCourseCode {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string ClassPeriodName {get; set;}
          
        public string ClassroomIdentificationCode {get; set;}
          
        public DateTime DateValue {get; set;}
          
        public int? AttendanceEventDescriptorTypeId {get; set;}
          
        public string AttendanceEventReason {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentMetricAssessmentHistorical table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetricAssessmentHistorical")]
	[Table(Name = "[domain].[StudentMetricAssessmentHistorical]")]
    public class StudentMetricAssessmentHistorical
    {                           
        public StudentMetricAssessmentHistorical() 
        {
        
        }      
        
		private string _keyName = "DisplayOrder";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.DisplayOrder;
        }  
        
        public static string GetKeyColumn()
        {
            return "DisplayOrder";
        }                                 
               
        
        public long StudentUSI {get; set;}
          
        public string SubContext {get; set;}
          
        public int MetricId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int DisplayOrder {get; set;}
          
        public string Context {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public string Value {get; set;}
          
        public string ValueTypeName {get; set;}
          
        public string ToolTipContext {get; set;}
          
        public string ToolTipSubContext {get; set;}
          
        public decimal? PerformanceLevelRatio {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentMetricAssessmentHistoricalMetaData table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetricAssessmentHistoricalMetaData")]
	[Table(Name = "[domain].[StudentMetricAssessmentHistoricalMetaData]")]
    public class StudentMetricAssessmentHistoricalMetaData
    {                           
        public StudentMetricAssessmentHistoricalMetaData() 
        {
        
        }      
        
		private string _keyName = "MetricId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.MetricId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricId";
        }                                 
               
        
        public long StudentUSI {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int MetricId {get; set;}
          
        public string Context {get; set;}
          
        public string SubContext {get; set;}
          
        public string DisplayType {get; set;}
          
        public string LabelType {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentMetricAttendanceRate table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetricAttendanceRate")]
	[Table(Name = "[domain].[StudentMetricAttendanceRate]")]
    public class StudentMetricAttendanceRate
    {                           
        public StudentMetricAttendanceRate() 
        {
        
        }      
        
		private string _keyName = "MetricId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.MetricId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricId";
        }                                 
               
        
        public long StudentUSI {get; set;}
          
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int MetricId {get; set;}
          
        public int PeriodSequence {get; set;}
          
        public string Context {get; set;}
          
        public decimal? AttendanceRate {get; set;}
          
        public decimal? ExcusedRate {get; set;}
          
        public decimal? UnexcusedRate {get; set;}
          
        public int? TotalDays {get; set;}
          
        public int? AttendanceDays {get; set;}
          
        public int? ExcusedDays {get; set;}
          
        public int? UnexcusedDays {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentMetricBenchmarkAssessment table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetricBenchmarkAssessment")]
	[Table(Name = "[domain].[StudentMetricBenchmarkAssessment]")]
    public class StudentMetricBenchmarkAssessment
    {                           
        public StudentMetricBenchmarkAssessment() 
        {
        
        }      
        
		private string _keyName = "AssessmentTitle";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.AssessmentTitle;
        }  
        
        public static string GetKeyColumn()
        {
            return "AssessmentTitle";
        }                                 
               
        
        public long StudentUSI {get; set;}
          
        public int SchoolId {get; set;}
          
        public int MetricId {get; set;}
          
        public DateTime Date {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string AssessmentTitle {get; set;}
          
        public int Version {get; set;}
          
        public string Value {get; set;}
          
        public string ValueType {get; set;}
          
        public int? TrendDirection {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentMetricCollegeReadinessAssessment table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetricCollegeReadinessAssessment")]
	[Table(Name = "[domain].[StudentMetricCollegeReadinessAssessment]")]
    public class StudentMetricCollegeReadinessAssessment
    {                           
        public StudentMetricCollegeReadinessAssessment() 
        {
        
        }      
        
		private string _keyName = "Date";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.Date;
        }  
        
        public static string GetKeyColumn()
        {
            return "Date";
        }                                 
               
        
        public long StudentUSI {get; set;}
          
        public int SchoolId {get; set;}
          
        public int MetricId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public DateTime Date {get; set;}
          
        public string Subject {get; set;}
          
        public string Score {get; set;}
          
        public string StateCriteria {get; set;}
          
        public bool? Flag {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentMetricCreditAccumulation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetricCreditAccumulation")]
	[Table(Name = "[domain].[StudentMetricCreditAccumulation]")]
    public class StudentMetricCreditAccumulation
    {                           
        public StudentMetricCreditAccumulation() 
        {
        
        }      
        
		private string _keyName = "MetricId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.MetricId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricId";
        }                                 
               
        
        public long StudentUSI {get; set;}
          
        public int SchoolId {get; set;}
          
        public short SchoolYear {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int MetricId {get; set;}
          
        public string GradeLevel {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public decimal? CumulativeCredits {get; set;}
          
        public decimal? RecommendedCredits {get; set;}
          
        public decimal? MinimumCredits {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentMetricDisciplineReferral table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetricDisciplineReferral")]
	[Table(Name = "[domain].[StudentMetricDisciplineReferral]")]
    public class StudentMetricDisciplineReferral
    {                           
        public StudentMetricDisciplineReferral() 
        {
        
        }      
        
		private string _keyName = "Action";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.Action;
        }  
        
        public static string GetKeyColumn()
        {
            return "Action";
        }                                 
               
        
        public long StudentUSI {get; set;}
          
        public int SchoolId {get; set;}
          
        public int MetricId {get; set;}
          
        public DateTime Date {get; set;}
          
        public string IncidentCode {get; set;}
          
        public string IncidentDescription {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string Action {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentMetricLearningObjective table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetricLearningObjective")]
	[Table(Name = "[domain].[StudentMetricLearningObjective]")]
    public class StudentMetricLearningObjective
    {                           
        public StudentMetricLearningObjective() 
        {
        
        }      
        
		private string _keyName = "AssessmentTitle";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.AssessmentTitle;
        }  
        
        public static string GetKeyColumn()
        {
            return "AssessmentTitle";
        }                                 
               
        
        public long StudentUSI {get; set;}
          
        public int MetricId {get; set;}
          
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string AssessmentTitle {get; set;}
          
        public string LearningObjective {get; set;}
          
        public string ObjectiveName {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public string Value {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentMetricLearningStandard table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetricLearningStandard")]
	[Table(Name = "[domain].[StudentMetricLearningStandard]")]
    public class StudentMetricLearningStandard
    {                           
        public StudentMetricLearningStandard() 
        {
        
        }      
        
		private string _keyName = "AssessmentTitle";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.AssessmentTitle;
        }  
        
        public static string GetKeyColumn()
        {
            return "AssessmentTitle";
        }                                 
               
        
        public long StudentUSI {get; set;}
          
        public int MetricId {get; set;}
          
        public int SchoolId {get; set;}
          
        public DateTime DateAdministration {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string AssessmentTitle {get; set;}
          
        public int Version {get; set;}
          
        public string GradeLevel {get; set;}
          
        public string LearningObjective {get; set;}
          
        public string LearningStandard {get; set;}
          
        public string Description {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public string Value {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentMetricLearningStandardMetaData table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetricLearningStandardMetaData")]
	[Table(Name = "[domain].[StudentMetricLearningStandardMetaData]")]
    public class StudentMetricLearningStandardMetaData
    {                           
        public StudentMetricLearningStandardMetaData() 
        {
        
        }      
        
		private string _keyName = "GradeLevel";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.GradeLevel;
        }  
        
        public static string GetKeyColumn()
        {
            return "GradeLevel";
        }                                 
               
        
        public int MetricId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string GradeLevel {get; set;}
          
        public string LearningObjective {get; set;}
          
        public string LearningStandard {get; set;}
          
        public string Description {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentMetricObjective table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetricObjective")]
	[Table(Name = "[domain].[StudentMetricObjective]")]
    public class StudentMetricObjective
    {                           
        public StudentMetricObjective() 
        {
        
        }      
        
		private string _keyName = "Description";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.Description;
        }  
        
        public static string GetKeyColumn()
        {
            return "Description";
        }                                 
               
        
        public long StudentUSI {get; set;}
          
        public int MetricId {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string Description {get; set;}
          
        public string Value {get; set;}
          
        public bool? Flag {get; set;}
          
        public int? ObjectiveItem {get; set;}
          
        public string ObjectiveName {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentMetricStateAssessmentHistorical table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetricStateAssessmentHistorical")]
	[Table(Name = "[domain].[StudentMetricStateAssessmentHistorical]")]
    public class StudentMetricStateAssessmentHistorical
    {                           
        public StudentMetricStateAssessmentHistorical() 
        {
        
        }      
        
		private string _keyName = "AdministrationYear";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.AdministrationYear;
        }  
        
        public static string GetKeyColumn()
        {
            return "AdministrationYear";
        }                                 
               
        
        public long StudentUSI {get; set;}
          
        public int SchoolId {get; set;}
          
        public int MetricId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public short AdministrationYear {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public string Context {get; set;}
          
        public int? NonNormalized {get; set;}
          
        public int? Normalized {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentParentInformation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentParentInformation")]
	[Table(Name = "[domain].[StudentParentInformation]")]
    public class StudentParentInformation
    {                           
        public StudentParentInformation() 
        {
        
        }      
        
		private string _keyName = "ParentUSI";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.ParentUSI;
        }  
        
        public static string GetKeyColumn()
        {
            return "ParentUSI";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int ParentUSI {get; set;}
          
        public long StudentUSI {get; set;}
          
        public string FullName {get; set;}
          
        public string Relation {get; set;}
          
        public string AddressLine1 {get; set;}
          
        public string AddressLine2 {get; set;}
          
        public string AddressLine3 {get; set;}
          
        public string City {get; set;}
          
        public string State {get; set;}
          
        public string ZipCode {get; set;}
          
        public string TelephoneNumber {get; set;}
          
        public string WorkTelephoneNumber {get; set;}
          
        public string EmailAddress {get; set;}
          
        public bool? PrimaryContact {get; set;}
          
        public bool? LivesWith {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentProfileDetail table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentProfileDetail")]
	[Table(Name = "[domain].[StudentProfileDetail]")]
    public class StudentProfileDetail
    {                           
        public StudentProfileDetail() 
        {
        
        }      
        
		private string _keyName = "ProfileDetail";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.ProfileDetail;
        }  
        
        public static string GetKeyColumn()
        {
            return "ProfileDetail";
        }                                 
               
        
        public long StudentUSI {get; set;}
          
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string ProfileDetail {get; set;}
          
        public int? DisplayOrder {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentRecordAssessmentHistory table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentRecordAssessmentHistory")]
	[Table(Name = "[domain].[StudentRecordAssessmentHistory]")]
    public class StudentRecordAssessmentHistory
    {                           
        public StudentRecordAssessmentHistory() 
        {
        
        }      
        
		private string _keyName = "AcademicSubject";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.AcademicSubject;
        }  
        
        public static string GetKeyColumn()
        {
            return "AcademicSubject";
        }                                 
               
        
        public long StudentUSI {get; set;}
          
        public short SchoolYear {get; set;}
          
        public DateTime AdministrationDate {get; set;}
          
        public string GradeLevel {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string AcademicSubject {get; set;}
          
        public string AssessmentTitle {get; set;}
          
        public string AssessmentCategory {get; set;}
          
        public bool Accommodations {get; set;}
          
        public string Score {get; set;}
          
        public int? MetStandardScore {get; set;}
          
        public int? CommendedScore {get; set;}
          
        public int? MetMinimum {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentRecordCourseHistory table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentRecordCourseHistory")]
	[Table(Name = "[domain].[StudentRecordCourseHistory]")]
    public class StudentRecordCourseHistory
    {                           
        public StudentRecordCourseHistory() 
        {
        
        }      
        
		private string _keyName = "GradeLevel";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.GradeLevel;
        }  
        
        public static string GetKeyColumn()
        {
            return "GradeLevel";
        }                                 
               
        
        public long StudentUSI {get; set;}
          
        public short SchoolYear {get; set;}
          
        public string TermType {get; set;}
          
        public string LocalCourseCode {get; set;}
          
        public string CourseTitle {get; set;}
          
        public string SubjectArea {get; set;}
          
        public string Instructor {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string GradeLevel {get; set;}
          
        public decimal? CreditsEarned {get; set;}
          
        public string FinalLetterGradeEarned {get; set;}
          
        public decimal? FinalNumericGradeEarned {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentRecordCurrentCourse table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentRecordCurrentCourse")]
	[Table(Name = "[domain].[StudentRecordCurrentCourse]")]
    public class StudentRecordCurrentCourse
    {                           
        public StudentRecordCurrentCourse() 
        {
        
        }      
        
		private string _keyName = "GradingPeriod";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.GradingPeriod;
        }  
        
        public static string GetKeyColumn()
        {
            return "GradingPeriod";
        }                                 
               
        
        public long StudentUSI {get; set;}
          
        public int SchoolId {get; set;}
          
        public int TermTypeId {get; set;}
          
        public string TermType {get; set;}
          
        public string LocalCourseCode {get; set;}
          
        public string CourseTitle {get; set;}
          
        public string SubjectArea {get; set;}
          
        public decimal? CreditsToBeEarned {get; set;}
          
        public string Instructor {get; set;}
          
        public string GradeLevel {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int GradingPeriod {get; set;}
          
        public string LetterGradeEarned {get; set;}
          
        public decimal? NumericGradeEarned {get; set;}
          
        public int? TrendDirection {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentSchoolInformation table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentSchoolInformation")]
	[Table(Name = "[domain].[StudentSchoolInformation]")]
    public class StudentSchoolInformation
    {                           
        public StudentSchoolInformation() 
        {
        
        }      
        
		private string _keyName = "SchoolId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.SchoolId;
        }  
        
        public static string GetKeyColumn()
        {
            return "SchoolId";
        }                                 
               
        
        public long StudentUSI {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int SchoolId {get; set;}
          
        public string GradeLevel {get; set;}
          
        public string Homeroom {get; set;}
          
        public string LateEnrollment {get; set;}
          
        public string IncompleteTranscript {get; set;}
          
        public DateTime? DateOfEntry {get; set;}
          
        public DateTime? DateOfWithdrawal {get; set;}
          
        public string WithdrawalCode {get; set;}
          
        public string WithdrawalDescription {get; set;}
          
        public string GraduationPlan {get; set;}
          
        public string ExpectedGraduationYear {get; set;}
          
        public string FeederSchools {get; set;}
          
        public string StudentCategory {get; set;}
          
        public string PositionTitle {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentSchoolMetricInstanceSet table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentSchoolMetricInstanceSet")]
	[Table(Name = "[domain].[StudentSchoolMetricInstanceSet]")]
    public class StudentSchoolMetricInstanceSet
    {                           
        public StudentSchoolMetricInstanceSet() 
        {
        
        }      
        
		private string _keyName = "MetricInstanceSetKey";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.MetricInstanceSetKey;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricInstanceSetKey";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public Guid MetricInstanceSetKey {get; set;}
          
        public long StudentUSI {get; set;}
          
        public int SchoolId {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.TeacherSection table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[TeacherSection")]
	[Table(Name = "[domain].[TeacherSection]")]
    public class TeacherSection
    {                           
        public TeacherSection() 
        {
        
        }      
        
		private string _keyName = "TeacherSectionId";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.TeacherSectionId;
        }  
        
        public static string GetKeyColumn()
        {
            return "TeacherSectionId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int TeacherSectionId {get; set;}
          
        public long StaffUSI {get; set;}
          
        public int SchoolId {get; set;}
          
        public string TermType {get; set;}
          
        public string ClassPeriod {get; set;}
          
        public string LocalCourseCode {get; set;}
          
        public string CourseTitle {get; set;}
          
        public string SubjectArea {get; set;}
          
        public string GradeLevel {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.TeacherStudentSection table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[TeacherStudentSection")]
	[Table(Name = "[domain].[TeacherStudentSection]")]
    public class TeacherStudentSection
    {                           
        public TeacherStudentSection() 
        {
        
        }      
        
		private string _keyName = "StudentUSI";
        public string KeyName()
        {
            return _keyName;
        }

        public object KeyValue()
        {
			            return this.StudentUSI;
        }  
        
        public static string GetKeyColumn()
        {
            return "StudentUSI";
        }                                 
               
        
        public int TeacherSectionId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public long StudentUSI {get; set;}
         
    } 
}
