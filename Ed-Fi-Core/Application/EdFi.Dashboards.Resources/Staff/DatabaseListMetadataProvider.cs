using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Staff;
using SubSonic.Linq.Structure;

namespace EdFi.Dashboards.Resources.Staff
{
    //public interface IListMetadataProvider
    //{
    //    /// <summary>
    //    /// Gets the metadata needed to build a list of entities and the metric columns.
    //    /// </summary>
    //    /// <param name="metadataListId">the id to pick from the DB.</param>
    //    /// <returns></returns>
    //    List<MetadataColumnGroup> GetListMetadata(int metadataListId);

    //    /// <summary>
    //    /// Gets the metadata needed to build a list of entities and the metric columns.
    //    /// </summary>
    //    /// <param name="metadataListId">the id to pick from the DB.</param>
    //    /// <param name="subjectArea"></param>
    //    /// <returns></returns>
    //    List<MetadataColumnGroup> GetListMetadata(int metadataListId, string subjectArea);
    //}

    public class DatabaseListMetadataProvider : IListMetadataProvider
    {
        private readonly IService<MetadataListColumnRequest, MetadataListColumnModel> metadataListColumnService;

        public DatabaseListMetadataProvider(
            IService<MetadataListColumnRequest, MetadataListColumnModel> metadataListColumnService)
        {
            this.metadataListColumnService = metadataListColumnService;
        }

        public List<MetadataColumnGroup> GetListMetadata(int metadataListId)
        {
            return GetListMetadata(metadataListId, null);
        }

        public List<MetadataColumnGroup> GetListMetadata(int metadataListId, string subjectArea)
        {
            var request = new MetadataListColumnRequest 
                                                        {MetadataListId = metadataListId, SubjectArea = subjectArea};

            return metadataListColumnService.Get(request).ColumnGroups.Where(g => g.Columns.Any()).ToList();
        }

        /// <summary>
        /// Gets the current version of IListMetadataProvider
        /// </summary>
        /// <remarks>Version number of the metadata.  When this value changes, the client should clear all saved settings from
        /// prior versions of the list metadata.  This value can be any kind of string, a randomly generated GUID, or a
        /// fingerprint of the source data.
        /// 
        /// This is most useful when you make a change to the metadata, and you want to force all users to use the new
        /// metadata instead of their saved version.
        /// </remarks>
        public string GetMetadataVersion()
        {
            //TODO: We could use a thumbprint of the data instead, so that we don't have to change the guid manually.
            return "5140208c-4b09-4747-bee8-5ffe8d47bc9f";
        }
    }
}