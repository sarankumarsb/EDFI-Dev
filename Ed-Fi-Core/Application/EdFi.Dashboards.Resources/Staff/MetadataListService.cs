using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Security.Common;

namespace EdFi.Dashboards.Resources.Staff
{
    public class MetadataListRequest
    {
        public string MetadataListName { get; set; }

        public static MetadataListRequest Create(string metadataListName)
        {
            return new MetadataListRequest { MetadataListName = metadataListName };
        }
    }

    public interface IMetadataListService : IService<MetadataListRequest, MetadataListModel> { }


    public class MetadataListService : IMetadataListService
    {
        private readonly IRepository<MetadataList> repository;

        public MetadataListService(IRepository<MetadataList> repository)
        {
            this.repository = repository;
        }

        [AuthenticationIgnore("Everyone should be able to get metadata")]
        public MetadataListModel Get(MetadataListRequest request)
        {
            var model =
                    (from list in repository.GetAll()
                     where list.Name == request.MetadataListName
                     select new MetadataListModel
                     {
                         MetadataListId = list.MetadataListId,
                         Name = list.Name
                     }).SingleOrDefault();

            return model;
        }
    }
}
