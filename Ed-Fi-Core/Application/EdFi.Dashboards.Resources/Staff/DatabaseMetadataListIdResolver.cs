using System;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Models.Student;
using SubSonic.Repository;

namespace EdFi.Dashboards.Resources.Staff
{
    public interface IDatabaseMetadataListIdResolver
    {
        int GetListId(string listType, SchoolCategory schoolCategory);
    }

    public class DatabaseMetadataListIdResolver :  IDatabaseMetadataListIdResolver, IMetadataListIdResolver
    {
        private readonly IService<MetadataListRequest, MetadataListModel> metadataListService;

        public DatabaseMetadataListIdResolver(IService<MetadataListRequest, MetadataListModel> metadataListService)
        {
            this.metadataListService = metadataListService;
        }

        public virtual int GetListId(ListType listType, SchoolCategory schoolCategory)
        {
            switch(schoolCategory)
            {
                case SchoolCategory.None:
                case SchoolCategory.Ungraded:
                    schoolCategory = SchoolCategory.HighSchool;
                    break;
            }

            return GetListId(listType.ToString(), schoolCategory);
        }

        public int GetListId(ListType listType, SchoolCategory schoolCategory, string subjectArea)
        {
            return GetListId(listType, schoolCategory);
        }

        public int GetListId(string listType, SchoolCategory schoolCategory)
        {
            var metadataListModel =
                metadataListService.Get(new MetadataListRequest
                {
                    MetadataListName = string.Format("{0}{1}", listType, schoolCategory)
                });

            metadataListModel = metadataListModel ?? metadataListService.Get(new MetadataListRequest {MetadataListName = listType});

            return metadataListModel.MetadataListId;
        }
    }
}
