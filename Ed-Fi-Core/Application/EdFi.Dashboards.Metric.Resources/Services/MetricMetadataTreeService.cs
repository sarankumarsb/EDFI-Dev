// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using System.Linq;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources;
using MetricAction = EdFi.Dashboards.Metric.Resources.Models.MetricAction;
using MetricState = EdFi.Dashboards.Metric.Resources.Models.MetricState;

namespace EdFi.Dashboards.Metric.Resources.Services
{
    public class MetricMetadataTreeRequest
    {
        public static MetricMetadataTreeRequest Create()
        {
            return new MetricMetadataTreeRequest();
        }
    }

    public interface IMetricMetadataTreeService : IService<MetricMetadataTreeRequest, MetricMetadataTree> { }

    public class MetricMetadataTreeService : IMetricMetadataTreeService
    {
        private class MetricMetadataMetricJoin
        {
            public int MetricNodeId { get; set; }
            public int MetricVariantId { get; set; }
            public int MetricId { get; set; }
            public int? ParentNodeId { get; set; }
            public int RootNodeId { get; set; }
            public string DisplayName { get; set; }
            public int DisplayOrder { get; set; }
            public Models.MetricType MetricType { get; set; }
            public Models.MetricVariantType MetricVariantType { get; set; }
            public string DomainEntityType { get; set; }
            public string Name { get; set; }
            public string ShortName { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }
            public string Tooltip { get; set; }
            public string Format { get; set; }
            public string ListFormat { get; set; }
            public string ListDataLabel { get; set; }
            public string NumeratorDenominatorFormat { get; set; }
            public int? TrendInterpretation { get; set; }
            public bool Enabled { get; set; }
            public int ChildMetricId { get; set; }
        }

        private readonly IRepository<Metric.Data.Entities.Metric> metricRepository;
        private readonly IRepository<Metric.Data.Entities.MetricVariant> metricVariantRepository;
        private readonly IRepository<MetricNode> metricNodeRepository;
        private readonly IRepository<Metric.Data.Entities.MetricState> metricStateRepository;
        private readonly IRepository<Metric.Data.Entities.MetricAction> metricActionRepository;
        private readonly IRepository<DomainEntityType> domainEntityTypeRepository;

        public MetricMetadataTreeService(IRepository<Metric.Data.Entities.Metric> metricRepository, IRepository<Metric.Data.Entities.MetricVariant> metricVariantRepository, IRepository<MetricNode> metricNodeRepository, IRepository<Metric.Data.Entities.MetricState> metricStateRepository, IRepository<Metric.Data.Entities.MetricAction> metricActionRepository, IRepository<DomainEntityType> domainEntityTypeRepository)
        {
            this.domainEntityTypeRepository = domainEntityTypeRepository;
            this.metricRepository = metricRepository;
            this.metricVariantRepository = metricVariantRepository;
            this.metricNodeRepository = metricNodeRepository;
            this.metricStateRepository = metricStateRepository;
            this.metricActionRepository = metricActionRepository;
        }

        ///<summary>
        ///This will return a list of root MetricMetadataNodes that contains the metrics tree structure
        ///</summary>
        ///<returns></returns>
        //[CacheNoCopy]
        //[CacheBehavior(copyOnSet: true, copyOnGet: false, absoluteExpirationInSecondsPastMidnight: 60*60*2)]  // Expire cache entry at 2 a.m.
        [CacheBehavior(copyOnSet: false, copyOnGet: false, absoluteExpirationInSecondsPastMidnight: 60*60*2)]  // Expire cache entry at 2 a.m.
        public MetricMetadataTree Get(MetricMetadataTreeRequest request)
        {
            if (request == null)
                throw new ArgumentNullException("Request cannot be null.", "request");

            // executing latent linq queries
            var domainEntityTypeData = domainEntityTypeRepository.GetAll().ToList();
            var metricNodeToMetricJoinData = GetMetricNodeToMetricJoinData(domainEntityTypeData).ToList();
            var metricStateData = GetMetricStateData().ToList();
            var metricActionData = GetMetricActionData().ToList();

            // instantiate return object
            var metadataTree = new MetricMetadataTree();
            var treeRootChildren = new List<MetricMetadataNode>();

            MetricMetadataNode lastMetadataNode = null;

            foreach (var metricNodeMetric in metricNodeToMetricJoinData.Where(x => x.MetricNodeId == x.RootNodeId))
            {
                var child = new MetricMetadataNode(metadataTree)
                                        {
                                            DisplayName = metricNodeMetric.DisplayName,
                                            DisplayOrder = metricNodeMetric.DisplayOrder,
                                            ChildDomainEntityMetricId = metricNodeMetric.ChildMetricId,
                                            Enabled = metricNodeMetric.Enabled,
                                            Format = metricNodeMetric.Format,
                                            ListFormat = metricNodeMetric.ListFormat,
                                            ListDataLabel = metricNodeMetric.ListDataLabel,
                                            Description = metricNodeMetric.Description,
                                            MetricId = metricNodeMetric.MetricId,
                                            Name = metricNodeMetric.Name,
                                            MetricNodeId = metricNodeMetric.MetricNodeId,
                                            MetricVariantId = metricNodeMetric.MetricVariantId,
                                            ShortName = metricNodeMetric.ShortName,
                                            Tooltip = metricNodeMetric.Tooltip,
                                            MetricType = metricNodeMetric.MetricType,
                                            MetricVariantType = metricNodeMetric.MetricVariantType,
                                            Url = metricNodeMetric.Url,
                                            NumeratorDenominatorFormat = metricNodeMetric.NumeratorDenominatorFormat,
                                            Parent = null,
                                            //ParentNodeId = metricNodeMetric.ParentNodeId,
                                            RootNodeId = metricNodeMetric.RootNodeId,
                                            DomainEntityType = metricNodeMetric.DomainEntityType,
                                            TrendInterpretation = metricNodeMetric.TrendInterpretation,
                                            Actions = GetMetricActions(metricNodeMetric.MetricVariantId, metricActionData),
                                            States = GetMetricStates(metricNodeMetric.MetricId, metricStateData)
                                        };

                if (lastMetadataNode != null)
                    lastMetadataNode.NextSibling = child;

                lastMetadataNode = child;

                //set children to this node
                child.Children = GetChildren(metadataTree, metricNodeMetric.MetricNodeId, metricNodeToMetricJoinData, metricActionData, metricStateData, child);
                
                //add this child to the root node
                treeRootChildren.Add(child);
            }

            metadataTree.Children = treeRootChildren;

            return metadataTree;
        }


        //private static MetricMetadataNode GetRootNode()
        //{
        //    //root node holder 
        //    return new MetricMetadataNode
        //               {
        //                   Name = "Root",
        //                   MetricNodeId = int.MinValue,
        //                   Parent = null
        //               };
        //}

        private IEnumerable<MetricMetadataNode> GetChildren(MetricMetadataTree tree, int metricNodeId, IEnumerable<MetricMetadataMetricJoin> metricNodeToMetricJoinData, List<Metric.Data.Entities.MetricAction> metricActionData, List<Metric.Data.Entities.MetricState> metricStateData, MetricMetadataNode parent)
        {
            var returnList = new List<MetricMetadataNode>();
            MetricMetadataNode lastMetadataNode = null;

            foreach (var metricJoin in metricNodeToMetricJoinData.Where(x => x.ParentNodeId == metricNodeId).OrderBy(x => x.DisplayOrder).ToList())
            {
                var metadataNode = new MetricMetadataNode(tree)
                                                      {
                                                          Parent = parent,
                                                          DisplayName = metricJoin.DisplayName,
                                                          DisplayOrder = metricJoin.DisplayOrder,
                                                          ChildDomainEntityMetricId = metricJoin.ChildMetricId,
                                                          Enabled = metricJoin.Enabled,
                                                          Format = metricJoin.Format,
                                                          ListFormat = metricJoin.ListFormat,
                                                          ListDataLabel = metricJoin.ListDataLabel,
                                                          Description = metricJoin.Description,
                                                          MetricId = metricJoin.MetricId,
                                                          Name = metricJoin.Name,
                                                          MetricNodeId = metricJoin.MetricNodeId,
                                                          MetricVariantId = metricJoin.MetricVariantId,
                                                          ShortName = metricJoin.ShortName,
                                                          Tooltip = metricJoin.Tooltip,
                                                          MetricType = metricJoin.MetricType,
                                                          MetricVariantType = metricJoin.MetricVariantType,
                                                          Url = metricJoin.Url,
                                                          NumeratorDenominatorFormat = metricJoin.NumeratorDenominatorFormat,
                                                          RootNodeId = metricJoin.RootNodeId,
                                                          TrendInterpretation = metricJoin.TrendInterpretation,
                                                          DomainEntityType = metricJoin.DomainEntityType,
                                                          Actions = GetMetricActions(metricJoin.MetricVariantId, metricActionData),
                                                          States = GetMetricStates(metricJoin.MetricId, metricStateData)
                                                      };

                if (lastMetadataNode != null)
                    lastMetadataNode.NextSibling = metadataNode;

                lastMetadataNode = metadataNode;

                metadataNode.Children = GetChildren(tree, metricJoin.MetricNodeId, metricNodeToMetricJoinData, metricActionData, metricStateData, metadataNode);
                returnList.Add(metadataNode);
            }
            return returnList;
        }

        private static IEnumerable<MetricState> GetMetricStates(int metricId, IEnumerable<Metric.Data.Entities.MetricState> metricStateData)
        {
            return (from ms in metricStateData
                    where ms.MetricId == metricId
                    select new MetricState
                    {
                        StateType = (Models.MetricStateType)ms.MetricStateTypeId,
                        Format = ms.Format,
                        StateText = ms.StateText,
                        MinValue = ms.MinValue,
                        MaxValue = ms.MaxValue,
                        IsMinValueInclusive = ms.IsMinValueInclusive.GetValueOrDefault(),
                        IsMaxValueInclusive = ms.IsMaxValueInclusive.GetValueOrDefault()
                    }).ToList();
        }

        private static IEnumerable<MetricAction> GetMetricActions(int metricVariantId, IEnumerable<Metric.Data.Entities.MetricAction> metricActionData)
        {
            var retval = (from mActionData in metricActionData
                          where mActionData.MetricVariantId == metricVariantId
                          select new MetricAction
                          {
                              MetricVariantId = mActionData.MetricVariantId,
                              ActionType = (Models.MetricActionType)mActionData.MetricActionTypeId,
                              Title = mActionData.Title,
                              Tooltip = mActionData.Tooltip,
                              Url = mActionData.Url,
                              DrilldownHeader = mActionData.DrilldownHeader,
                              DrilldownFooter = mActionData.DrilldownHeader,
                              Icon = mActionData.Icon
                          }).ToList();

            return retval;
        }

        private IEnumerable<Metric.Data.Entities.MetricState> GetMetricStateData()
        {
            return (from ms in metricStateRepository.GetAll()
                    select ms);
        }

        private IEnumerable<Metric.Data.Entities.MetricAction> GetMetricActionData()
        {
            return (from ma in metricActionRepository.GetAll()
                    select ma);
        }

        private IEnumerable<MetricMetadataMetricJoin> GetMetricNodeToMetricJoinData(IEnumerable<DomainEntityType> domainEntityTypeData)
        {
            //This temp is being created because subsonic cannot do the projection to properties that are not names the same as the column in the database
            var tempMetricNodeToMetricJoin = (from mi in metricNodeRepository.GetAll()
                                              join mui in metricVariantRepository.GetAll() on mi.MetricVariantId equals mui.MetricVariantId
                                              join m in metricRepository.GetAll() on mui.MetricId equals m.MetricId
                                              orderby mi.DisplayOrder
                                              select new
                                              {
                                                  mi.MetricNodeId,
                                                  mi.MetricVariantId,
                                                  mi.MetricId,
                                                  mi.ParentNodeId,
                                                  mi.RootNodeId,
                                                  mi.DisplayName,
                                                  mi.DisplayOrder,
                                                  m.MetricTypeId,
                                                  mui.MetricVariantTypeId,
                                                  mui.MetricName,
                                                  mui.MetricShortName,
                                                  mui.MetricDescription,
                                                  mui.MetricUrl,
                                                  mui.MetricTooltip,
                                                  mui.Format,
                                                  mui.ListFormat,
                                                  mui.ListDataLabel,
                                                  mui.NumeratorDenominatorFormat,
                                                  m.TrendInterpretation,
                                                  m.Enabled,
                                                  m.DomainEntityTypeId,
                                                  m.ChildDomainEntityMetricId
                                              }).ToList();

            return (from tm in tempMetricNodeToMetricJoin
                    select new MetricMetadataMetricJoin
                    {
                        MetricNodeId = tm.MetricNodeId,
                        MetricVariantId = tm.MetricVariantId,
                        MetricId = tm.MetricId,
                        ParentNodeId = tm.ParentNodeId.GetValueOrDefault(),
                        RootNodeId = tm.RootNodeId.GetValueOrDefault(),
                        DisplayName = String.IsNullOrEmpty(tm.DisplayName) ? tm.MetricName : tm.DisplayName,
                        DisplayOrder = tm.DisplayOrder,
                        MetricType = GetMetricType(tm.MetricTypeId),
                        MetricVariantType = (Models.MetricVariantType)tm.MetricVariantTypeId,
                        Name = tm.MetricName,
                        ShortName = tm.MetricShortName,
                        Description = tm.MetricDescription,
                        Url = tm.MetricUrl,
                        Tooltip = tm.MetricTooltip,
                        Format = tm.Format,
                        ListFormat = tm.ListFormat,
                        ListDataLabel = tm.ListDataLabel,
                        NumeratorDenominatorFormat = tm.NumeratorDenominatorFormat,
                        TrendInterpretation = tm.TrendInterpretation,
                        Enabled = tm.Enabled == null || tm.Enabled.GetValueOrDefault(),
                        DomainEntityType = GetDomainEntityName(tm.DomainEntityTypeId, domainEntityTypeData),
                        ChildMetricId = tm.ChildDomainEntityMetricId.GetValueOrDefault(),
                    }).ToList();
        }

        private static string GetDomainEntityName(int? domainEntityTypeId, IEnumerable<DomainEntityType> domainEntityTypeData)
        {
            if (domainEntityTypeId == null)
                return string.Empty;

            string entityTypeName = domainEntityTypeData
                .Where(x => x.DomainEntityTypeId == domainEntityTypeId)
                .Select(x => x.DomainEntityTypeName).SingleOrDefault();

            return entityTypeName;
        }

        private static Models.MetricType GetMetricType(int metricTypeId)
        {
            // TODO: Deferred - Remove this translation once the metricsDB metadata has been "refactored"
            // This switch statement translates from the current metadata which makes an unnecessary distinction between standard Aggregate/Container/Granular metrics and the campus variety
            // so that our code only looks at the core metric type (not flavors for each metric instance set type that is supported).
            switch (metricTypeId)
            {
                case 1:
                case 2:
                case 3:
                    return (Models.MetricType) metricTypeId;

                case 4: // Campus Aggregate -> Aggregate
                    return Models.MetricType.AggregateMetric;

                case 5: // Campus Granular -> Granular
                    return Models.MetricType.GranularMetric;
                
                default:
                    throw new NotSupportedException(string.Format("metricTypeId '{0}' is not supported.", metricTypeId));
            }
        }
    }
}
