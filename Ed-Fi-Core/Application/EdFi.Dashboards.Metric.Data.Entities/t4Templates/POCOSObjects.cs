


using System;
using SubSonic.SqlGeneration.Schema;


namespace EdFi.Dashboards.Metric.Data.Entities
{
    
    
    /// <summary>
    /// A class which represents the metric.DomainEntityType table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[DomainEntityType")]
    public class DomainEntityType
    {                           
        public DomainEntityType() 
        {
        
        }      
        
        public string KeyName()
        {
            return "DomainEntityTypeId";
        }

        public object KeyValue()
        {
			            return this.DomainEntityTypeId;
        }  
        
        public static string GetKeyColumn()
        {
            return "DomainEntityTypeId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int DomainEntityTypeId {get; set;}
          
        public string DomainEntityTypeName {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.GranularMetricsWithMeaningfulHierarchicalName table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[GranularMetricsWithMeaningfulHierarchicalName")]
    public class GranularMetricsWithMeaningfulHierarchicalName
    {                           
        public GranularMetricsWithMeaningfulHierarchicalName() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricId";
        }

        public object KeyValue()
        {
			            return this.MetricId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetricId {get; set;}
          
        public int MetricTypeId {get; set;}
          
        public string MetricTypeName {get; set;}
          
        public string MetricName {get; set;}
          
        public int? RootNodeId {get; set;}
          
        public string RootMetricName {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetadataGroupType table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetadataGroupType")]
    public class MetadataGroupType
    {                           
        public MetadataGroupType() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetadataGroupTypeId";
        }

        public object KeyValue()
        {
			            return this.MetadataGroupTypeId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetadataGroupTypeId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetadataGroupTypeId {get; set;}
          
        public string Name {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetadataList table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetadataList")]
    public class MetadataList
    {                           
        public MetadataList() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetadataListId";
        }

        public object KeyValue()
        {
			            return this.MetadataListId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetadataListId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetadataListId {get; set;}
          
        public string Name {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetadataListColumn table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetadataListColumn")]
    public class MetadataListColumn
    {                           
        public MetadataListColumn() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetadataListColumnId";
        }

        public object KeyValue()
        {
			            return this.MetadataListColumnId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetadataListColumnId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetadataListColumnId {get; set;}
          
        public int MetadataListColumnGroupId {get; set;}
          
        public string ColumnName {get; set;}
          
        public string ColumnPrefix {get; set;}
          
        public bool? IsVisibleByDefault {get; set;}
          
        public bool? IsFixedColumn {get; set;}
          
        public int MetadataMetricCellListTypeId {get; set;}
          
        public int MetricVariantId {get; set;}
          
        public int ColumnOrder {get; set;}
          
        public string SortAscending {get; set;}
          
        public string SortDescending {get; set;}
          
        public string Tooltip {get; set;}
          
        public int UniqueIdentifier {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetadataListColumnGroup table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetadataListColumnGroup")]
    public class MetadataListColumnGroup
    {                           
        public MetadataListColumnGroup() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetadataListColumnGroupId";
        }

        public object KeyValue()
        {
			            return this.MetadataListColumnGroupId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetadataListColumnGroupId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetadataListColumnGroupId {get; set;}
          
        public int MetadataListId {get; set;}
          
        public int? MetadataSubjectAreaId {get; set;}
          
        public string Title {get; set;}
          
        public int MetadataGroupTypeId {get; set;}
          
        public bool IsVisibleByDefault {get; set;}
          
        public bool IsFixedColumnGroup {get; set;}
          
        public int GroupOrder {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetadataMetricCellListType table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetadataMetricCellListType")]
    public class MetadataMetricCellListType
    {                           
        public MetadataMetricCellListType() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetadataMetricCellListTypeId";
        }

        public object KeyValue()
        {
			            return this.MetadataMetricCellListTypeId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetadataMetricCellListTypeId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetadataMetricCellListTypeId {get; set;}
          
        public string Name {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetadataSubjectArea table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetadataSubjectArea")]
    public class MetadataSubjectArea
    {                           
        public MetadataSubjectArea() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetadataSubjectAreaId";
        }

        public object KeyValue()
        {
			            return this.MetadataSubjectAreaId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetadataSubjectAreaId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetadataSubjectAreaId {get; set;}
          
        public string Name {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.Metric table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[Metric")]
    public class Metric
    {                           
        public Metric() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricId";
        }

        public object KeyValue()
        {
			            return this.MetricId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetricId {get; set;}
          
        public int MetricTypeId {get; set;}
          
        public int? DomainEntityTypeId {get; set;}
          
        public string MetricName {get; set;}
          
        public int? TrendInterpretation {get; set;}
          
        public bool? Enabled {get; set;}
          
        public int? ChildDomainEntityMetricId {get; set;}
          
        public int? OverriddenByMetricId {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricAction table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricAction")]
    public class MetricAction
    {                           
        public MetricAction() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricVariantId";
        }

        public object KeyValue()
        {
			            return this.MetricVariantId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricVariantId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetricVariantId {get; set;}
          
        public string Title {get; set;}
          
        public string Tooltip {get; set;}
          
        public string Url {get; set;}
          
        public int MetricActionTypeId {get; set;}
          
        public string DrilldownHeader {get; set;}
          
        public string DrilldownFooter {get; set;}
          
        public string Icon {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricActionType table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricActionType")]
    public class MetricActionType
    {                           
        public MetricActionType() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricActionTypeId";
        }

        public object KeyValue()
        {
			            return this.MetricActionTypeId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricActionTypeId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetricActionTypeId {get; set;}
          
        public string MetricActionTypeName {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricComponent table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricComponent")]
    public class MetricComponent
    {                           
        public MetricComponent() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricId";
        }

        public object KeyValue()
        {
			            return this.MetricId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricId";
        }                                 
               
        
        public Guid MetricInstanceSetKey {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int MetricId {get; set;}
          
        public string Name {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public string Value {get; set;}
          
        public string ValueTypeName {get; set;}
          
        public string Format {get; set;}
          
        public int? TrendDirection {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricFootnoteDescriptionType table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricFootnoteDescriptionType")]
    public class MetricFootnoteDescriptionType
    {                           
        public MetricFootnoteDescriptionType() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricFootnoteDescriptionTypeId";
        }

        public object KeyValue()
        {
			            return this.MetricFootnoteDescriptionTypeId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricFootnoteDescriptionTypeId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetricFootnoteDescriptionTypeId {get; set;}
          
        public string CodeValue {get; set;}
          
        public string Description {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricGoal table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricGoal")]
    public class MetricGoal
    {                           
        public MetricGoal() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricId";
        }

        public object KeyValue()
        {
			            return this.MetricId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricId";
        }                                 
               
        
        public Guid MetricInstanceSetKey {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int MetricId {get; set;}
          
        public decimal Value {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricIndicator table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricIndicator")]
    public class MetricIndicator
    {                           
        public MetricIndicator() 
        {
        
        }      
        
        public string KeyName()
        {
            return "IndicatorTypeId";
        }

        public object KeyValue()
        {
			            return this.IndicatorTypeId;
        }  
        
        public static string GetKeyColumn()
        {
            return "IndicatorTypeId";
        }                                 
               
        
        public Guid MetricInstanceSetKey {get; set;}
          
        public int MetricId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int IndicatorTypeId {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricInstance table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricInstance")]
    public class MetricInstance
    {                           
        public MetricInstance() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricId";
        }

        public object KeyValue()
        {
			            return this.MetricId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricId";
        }                                 
               
        
        public Guid MetricInstanceSetKey {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int MetricId {get; set;}
          
        public int? MetricStateTypeId {get; set;}
          
        public string Context {get; set;}
          
        public string Value {get; set;}
          
        public string ValueTypeName {get; set;}
          
        public bool? Flag {get; set;}
          
        public int? TrendDirection {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricInstanceExtendedProperty table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricInstanceExtendedProperty")]
    public class MetricInstanceExtendedProperty
    {                           
        public MetricInstanceExtendedProperty() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricId";
        }

        public object KeyValue()
        {
			            return this.MetricId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricId";
        }                                 
               
        
        public Guid MetricInstanceSetKey {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int MetricId {get; set;}
          
        public string Name {get; set;}
          
        public string Value {get; set;}
          
        public string ValueTypeName {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricInstanceExtendedPropertyWithValueToFloat table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricInstanceExtendedPropertyWithValueToFloat")]
    public class MetricInstanceExtendedPropertyWithValueToFloat
    {                           
        public MetricInstanceExtendedPropertyWithValueToFloat() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricInstanceSetKey";
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
          
        public int MetricId {get; set;}
          
        public string Name {get; set;}
          
        public string Value {get; set;}
          
        public string ValueTypeName {get; set;}
          
        public double? ValueToFloat {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricInstanceFootnote table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricInstanceFootnote")]
    public class MetricInstanceFootnote
    {                           
        public MetricInstanceFootnote() 
        {
        
        }      
        
        public string KeyName()
        {
            return "FootnoteTypeId";
        }

        public object KeyValue()
        {
			            return this.FootnoteTypeId;
        }  
        
        public static string GetKeyColumn()
        {
            return "FootnoteTypeId";
        }                                 
               
        
        public Guid MetricInstanceSetKey {get; set;}
          
        public int MetricId {get; set;}
                  
		[SubSonicPrimaryKey]        
        public int FootnoteTypeId {get; set;}
          
        public int MetricFootnoteDescriptionTypeId {get; set;}
          
        public int? Count {get; set;}
          
        public string FootnoteText {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricInstanceSet table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricInstanceSet")]
    public class MetricInstanceSet
    {                           
        public MetricInstanceSet() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricInstanceSetKey";
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
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricNode table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricNode")]
    public class MetricNode
    {                           
        public MetricNode() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricNodeId";
        }

        public object KeyValue()
        {
			            return this.MetricNodeId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricNodeId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetricNodeId {get; set;}
          
        public int MetricVariantId {get; set;}
          
        public int MetricId {get; set;}
          
        public int? ParentNodeId {get; set;}
          
        public int? RootNodeId {get; set;}
          
        public string DisplayName {get; set;}
          
        public int DisplayOrder {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricState table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricState")]
    public class MetricState
    {                           
        public MetricState() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricId";
        }

        public object KeyValue()
        {
			            return this.MetricId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetricId {get; set;}
          
        public int MetricStateTypeId {get; set;}
          
        public string StateText {get; set;}
          
        public decimal? MinValue {get; set;}
          
        public int? IsMinValueInclusive {get; set;}
          
        public decimal? MaxValue {get; set;}
          
        public int? IsMaxValueInclusive {get; set;}
          
        public string Format {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricStateType table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricStateType")]
    public class MetricStateType
    {                           
        public MetricStateType() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricStateTypeId";
        }

        public object KeyValue()
        {
			            return this.MetricStateTypeId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricStateTypeId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetricStateTypeId {get; set;}
          
        public string MetricStateTypeName {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricThreshold table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricThreshold")]
    public class MetricThreshold
    {                           
        public MetricThreshold() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricId";
        }

        public object KeyValue()
        {
			            return this.MetricId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetricId {get; set;}
          
        public Guid MetricInstanceSetKey {get; set;}
          
        public decimal Threshold {get; set;}
          
        public bool IsInclusive {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricType table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricType")]
    public class MetricType
    {                           
        public MetricType() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricTypeId";
        }

        public object KeyValue()
        {
			            return this.MetricTypeId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricTypeId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetricTypeId {get; set;}
          
        public string MetricTypeName {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricVariant table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricVariant")]
    public class MetricVariant
    {                           
        public MetricVariant() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricVariantId";
        }

        public object KeyValue()
        {
			            return this.MetricVariantId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricVariantId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetricVariantId {get; set;}
          
        public int MetricId {get; set;}
          
        public int MetricVariantTypeId {get; set;}
          
        public string MetricName {get; set;}
          
        public string MetricShortName {get; set;}
          
        public string MetricDescription {get; set;}
          
        public string MetricUrl {get; set;}
          
        public string MetricTooltip {get; set;}
          
        public string Format {get; set;}
          
        public string ListFormat {get; set;}
          
        public string ListDataLabel {get; set;}
          
        public string NumeratorDenominatorFormat {get; set;}
          
        public bool? Enabled {get; set;}
         
    } 
    
    
    /// <summary>
    /// A class which represents the metric.MetricVariantType table in the Dashboard Database.
    /// </summary>
    [SubSonicUseSingularTableName]
	[SubSonicTableNameOverride("metric].[MetricVariantType")]
    public class MetricVariantType
    {                           
        public MetricVariantType() 
        {
        
        }      
        
        public string KeyName()
        {
            return "MetricVariantTypeId";
        }

        public object KeyValue()
        {
			            return this.MetricVariantTypeId;
        }  
        
        public static string GetKeyColumn()
        {
            return "MetricVariantTypeId";
        }                                 
               
                
		[SubSonicPrimaryKey]        
        public int MetricVariantTypeId {get; set;}
          
        public string MetricVariantTypeName {get; set;}
         
    } 
}
