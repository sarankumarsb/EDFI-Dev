


using System;
using System.Linq;
using SubSonic.SqlGeneration.Schema;


namespace EdFi.Dashboards.Warehouse.Data.Entities
{
    
    
    /// <summary>
    /// A class which represents the domain.EdFiDWException table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[EdFiDWException")]
    public class EdFiDWException 
    {                          
		 
        public EdFiDWException() 
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
    /// A class which represents the domain.LocalEducationAgencyMetricComponent table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[LocalEducationAgencyMetricComponent")]
    public class LocalEducationAgencyMetricComponent 
    {                          
		 
        public LocalEducationAgencyMetricComponent() 
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
          
        public int MetricId {get; set;}
          
        public string Name {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public string Value {get; set;}
          
        public string ValueTypeName {get; set;}
          
        public string Format {get; set;}
          
        public int? TrendDirection {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.LocalEducationAgencyMetricInstance table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[LocalEducationAgencyMetricInstance")]
    public class LocalEducationAgencyMetricInstance 
    {                          
		 
        public LocalEducationAgencyMetricInstance() 
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
          
        public int MetricId {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public string Context {get; set;}
          
        public string Value {get; set;}
          
        public string ValueTypeName {get; set;}
          
        public bool? Flag {get; set;}
          
        public int? TrendDirection {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.LocalEducationAgencyMetricInstanceExtendedProperty table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[LocalEducationAgencyMetricInstanceExtendedProperty")]
    public class LocalEducationAgencyMetricInstanceExtendedProperty 
    {                          
		 
        public LocalEducationAgencyMetricInstanceExtendedProperty() 
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
          
        public int MetricId {get; set;}
          
        public string Name {get; set;}
          
        public string Value {get; set;}
          
        public string ValueTypeName {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.LocalEducationAgencyMetricInstanceSchoolList table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[LocalEducationAgencyMetricInstanceSchoolList")]
    public class LocalEducationAgencyMetricInstanceSchoolList 
    {                          
		 
        public LocalEducationAgencyMetricInstanceSchoolList() 
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
          
        public short SchoolYear {get; set;}
          
        public string Value {get; set;}
          
        public string ValueType {get; set;}
          
        public long? StaffUSI {get; set;}
          
        public string StaffFullName {get; set;}
          
        public decimal SchoolGoal {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.PriorYearStudentList table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[PriorYearStudentList")]
    public class PriorYearStudentList 
    {                          
		 
        public PriorYearStudentList() 
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
          
        public int LocalEducationAgencyId {get; set;}
          
        public short SchoolYear {get; set;}
          
        public int? PriorYearAttendanceMetricId {get; set;}
          
        public int? PriorYearAttendanceMetricStateTypeId {get; set;}
          
        public string PriorYearAttendanceValue {get; set;}
          
        public int? PriorYearAttendanceTrendDirection {get; set;}
          
        public string PriorYearAttendanceValueTypeName {get; set;}
          
        public int? PriorYearAbsencesMetricId {get; set;}
          
        public int? PriorYearAbsencesMetricStateTypeId {get; set;}
          
        public string PriorYearAbsencesValue {get; set;}
          
        public int? PriorYearAbsencesTrendDirection {get; set;}
          
        public string PriorYearAbsencesValueTypeName {get; set;}
          
        public int? PriorYearNumberOfDaysAbsentMetricId {get; set;}
          
        public int? PriorYearNumberOfDaysAbsentMetricStateTypeId {get; set;}
          
        public string PriorYearNumberOfDaysAbsentValue {get; set;}
          
        public int? PriorYearNumberOfDaysAbsentTrendDirection {get; set;}
          
        public string PriorYearNumberOfDaysAbsentValueTypeName {get; set;}
          
        public int? PriorYearDisciplineIncidentsMetricId {get; set;}
          
        public int? PriorYearDisciplineIncidentsMetricStateTypeId {get; set;}
          
        public string PriorYearDisciplineIncidentsValue {get; set;}
          
        public int? PriorYearDisciplineIncidentsTrendDirection {get; set;}
          
        public string PriorYearDisciplineIncidentsValueTypeName {get; set;}
          
        public int? PriorYearCodeOfConductMetricId {get; set;}
          
        public int? PriorYearCodeOfConductMetricStateTypeId {get; set;}
          
        public string PriorYearCodeOfConductValue {get; set;}
          
        public int? PriorYearCodeOfConductTrendDirection {get; set;}
          
        public string PriorYearCodeOfConductValueTypeName {get; set;}
          
        public int? PriorYearDIBELSMetricId {get; set;}
          
        public int? PriorYearDIBELSMetricStateTypeId {get; set;}
          
        public string PriorYearDIBELSValue {get; set;}
          
        public int? PriorYearDIBELSTrendDirection {get; set;}
          
        public string PriorYearDIBELSValueTypeName {get; set;}
          
        public int? PriorYearBelowCMetricId {get; set;}
          
        public int? PriorYearBelowCMetricStateTypeId {get; set;}
          
        public string PriorYearBelowCValue {get; set;}
          
        public int? PriorYearBelowCTrendDirection {get; set;}
          
        public string PriorYearBelowCValueTypeName {get; set;}
          
        public int? PriorYearFailingGradesMetricId {get; set;}
          
        public int? PriorYearFailingGradesMetricStateTypeId {get; set;}
          
        public string PriorYearFailingGradesValue {get; set;}
          
        public int? PriorYearFailingGradesTrendDirection {get; set;}
          
        public string PriorYearFailingGradesValueTypeName {get; set;}
          
        public int? PriorYearFailingGradesReadingMetricId {get; set;}
          
        public int? PriorYearFailingGradesReadingMetricStateTypeId {get; set;}
          
        public string PriorYearFailingGradesReadingValue {get; set;}
          
        public int? PriorYearFailingGradesReadingTrendDirection {get; set;}
          
        public string PriorYearFailingGradesReadingValueTypeName {get; set;}
          
        public int? PriorYearFailingGradesMathMetricId {get; set;}
          
        public int? PriorYearFailingGradesMathMetricStateTypeId {get; set;}
          
        public string PriorYearFailingGradesMathValue {get; set;}
          
        public int? PriorYearFailingGradesMathTrendDirection {get; set;}
          
        public string PriorYearFailingGradesMathValueTypeName {get; set;}
          
        public int? PriorYearFailingGradesWritingMetricId {get; set;}
          
        public int? PriorYearFailingGradesWritingMetricStateTypeId {get; set;}
          
        public string PriorYearFailingGradesWritingValue {get; set;}
          
        public int? PriorYearFailingGradesWritingTrendDirection {get; set;}
          
        public string PriorYearFailingGradesWritingValueTypeName {get; set;}
          
        public int? PriorYearFailingGradesScienceMetricId {get; set;}
          
        public int? PriorYearFailingGradesScienceMetricStateTypeId {get; set;}
          
        public string PriorYearFailingGradesScienceValue {get; set;}
          
        public int? PriorYearFailingGradesScienceTrendDirection {get; set;}
          
        public string PriorYearFailingGradesScienceValueTypeName {get; set;}
          
        public int? PriorYearFailingGradesSocialStudiesMetricId {get; set;}
          
        public int? PriorYearFailingGradesSocialStudiesMetricStateTypeId {get; set;}
          
        public string PriorYearFailingGradesSocialStudiesValue {get; set;}
          
        public int? PriorYearFailingGradesSocialStudiesTrendDirection {get; set;}
          
        public string PriorYearFailingGradesSocialStudiesValueTypeName {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolMetricComponent table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolMetricComponent")]
    public class SchoolMetricComponent 
    {                          
		 
        public SchoolMetricComponent() 
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
			        
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int LocalEducationAgencyId {get; set;}
          
        public short SchoolYear {get; set;}
          
        public int MetricId {get; set;}
          
        public string Name {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public string Value {get; set;}
          
        public string ValueTypeName {get; set;}
          
        public string Format {get; set;}
          
        public int? TrendDirection {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolMetricHistorical table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolMetricHistorical")]
    public class SchoolMetricHistorical 
    {                          
		 
        public SchoolMetricHistorical() 
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
			        
        public int PeriodIdentifierId {get; set;}
          
        public int PeriodTypeId {get; set;}
          
        public string PeriodType {get; set;}
          
        public DateTime StartDate {get; set;}
          
        public DateTime EndDate {get; set;}
          
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int MetricId {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public string Context {get; set;}
          
        public string Value {get; set;}
          
        public string ValueType {get; set;}
          
        public int? Numerator {get; set;}
          
        public int? Denominator {get; set;}
          
        public bool? Flag {get; set;}
          
        public int? TrendDirection {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolMetricInstance table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolMetricInstance")]
    public class SchoolMetricInstance 
    {                          
		 
        public SchoolMetricInstance() 
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
			        
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int LocalEducationAgencyId {get; set;}
          
        public short SchoolYear {get; set;}
          
        public int MetricId {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public string Context {get; set;}
          
        public string Value {get; set;}
          
        public string ValueTypeName {get; set;}
          
        public bool? Flag {get; set;}
          
        public int? TrendDirection {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolMetricInstanceExtendedProperty table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolMetricInstanceExtendedProperty")]
    public class SchoolMetricInstanceExtendedProperty 
    {                          
		 
        public SchoolMetricInstanceExtendedProperty() 
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
			        
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int LocalEducationAgencyId {get; set;}
          
        public short SchoolYear {get; set;}
          
        public int MetricId {get; set;}
          
        public string Name {get; set;}
          
        public string Value {get; set;}
          
        public string ValueTypeName {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolMetricInstanceStudentList table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolMetricInstanceStudentList")]
    public class SchoolMetricInstanceStudentList 
    {                          
		 
        public SchoolMetricInstanceStudentList() 
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
          
        public short SchoolYear {get; set;}
          
        public string Value {get; set;}
          
        public string ValueType {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.SchoolMetricInstanceTeacherList table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[SchoolMetricInstanceTeacherList")]
    public class SchoolMetricInstanceTeacherList 
    {                          
		 
        public SchoolMetricInstanceTeacherList() 
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
          
        public short SchoolYear {get; set;}
          
        public string Value {get; set;}
          
        public string ValueType {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StaffEducationOrgCategory table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StaffEducationOrgCategory")]
    public class StaffEducationOrgCategory 
    {                          
		 
        public StaffEducationOrgCategory() 
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
          
        public short SchoolYear {get; set;}
          
        public string StaffCategory {get; set;}
          
        public string PositionTitle {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentMetricBenchmarkAssessmentHistorical table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetricBenchmarkAssessmentHistorical")]
    public class StudentMetricBenchmarkAssessmentHistorical 
    {                          
		 
        public StudentMetricBenchmarkAssessmentHistorical() 
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
          
        public short SchoolYear {get; set;}
          
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
    /// A class which represents the domain.StudentMetricHistorical table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetricHistorical")]
    public class StudentMetricHistorical 
    {                          
		 
        public StudentMetricHistorical() 
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
			        
        public int PeriodIdentifierId {get; set;}
          
        public int PeriodTypeId {get; set;}
          
        public string PeriodType {get; set;}
          
        public DateTime StartDate {get; set;}
          
        public DateTime EndDate {get; set;}
          
        public long StudentUSI {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int MetricId {get; set;}
          
        public int SchoolId {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public string Context {get; set;}
          
        public string Value {get; set;}
          
        public string ValueType {get; set;}
          
        public int? Numerator {get; set;}
          
        public int? Denominator {get; set;}
          
        public bool? Flag {get; set;}
          
        public int? TrendDirection {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentMetricLearningStandardHistorical table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentMetricLearningStandardHistorical")]
    public class StudentMetricLearningStandardHistorical 
    {                          
		 
        public StudentMetricLearningStandardHistorical() 
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
          
        public short SchoolYear {get; set;}
          
        public string GradeLevel {get; set;}
          
        public DateTime DateAdministration {get; set;}
                  
		[SubSonicPrimaryKey]        
        public string AssessmentTitle {get; set;}
          
        public int Version {get; set;}
          
        public string LearningStandard {get; set;}
          
        public string LearningObjective {get; set;}
          
        public string Description {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public string Value {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentSchoolMetricComponent table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentSchoolMetricComponent")]
    public class StudentSchoolMetricComponent 
    {                          
		 
        public StudentSchoolMetricComponent() 
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
			        
        public long StudentUSI {get; set;}
          
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int LocalEducationAgencyId {get; set;}
          
        public short SchoolYear {get; set;}
          
        public int MetricId {get; set;}
          
        public string Name {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public string Value {get; set;}
          
        public string ValueTypeName {get; set;}
          
        public string Format {get; set;}
          
        public int? TrendDirection {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentSchoolMetricInstance table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentSchoolMetricInstance")]
    public class StudentSchoolMetricInstance 
    {                          
		 
        public StudentSchoolMetricInstance() 
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
			        
        public long StudentUSI {get; set;}
          
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int LocalEducationAgencyId {get; set;}
          
        public short SchoolYear {get; set;}
          
        public int MetricId {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public string Context {get; set;}
          
        public string Value {get; set;}
          
        public string ValueTypeName {get; set;}
          
        public bool? Flag {get; set;}
          
        public int? TrendDirection {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the domain.StudentSchoolMetricInstanceExtendedProperty table in the DashboardDW Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("domain].[StudentSchoolMetricInstanceExtendedProperty")]
    public class StudentSchoolMetricInstanceExtendedProperty 
    {                          
		 
        public StudentSchoolMetricInstanceExtendedProperty() 
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
			        
        public long StudentUSI {get; set;}
          
        public int SchoolId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int LocalEducationAgencyId {get; set;}
          
        public short SchoolYear {get; set;}
          
        public int MetricId {get; set;}
          
        public string Name {get; set;}
          
        public string Value {get; set;}
          
        public string ValueTypeName {get; set;}
         
    } 
}
