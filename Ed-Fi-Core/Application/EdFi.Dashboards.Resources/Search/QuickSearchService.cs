// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Infrastructure.Implementations.Caching;
using EdFi.Dashboards.Resources.Models.Search;
using EdFi.Dashboards.Resources.Security.Common;

namespace EdFi.Dashboards.Resources.Search
{
    public class QuickSearchRequest
    {
        public string TextToFind { get; set; }
        public int RowCountToReturn { get; set; }
        public bool MatchContains { get; set; }
        public SearchFilter PageFilter { get; set; }
        public Dictionary<SearchFilter, HashSet<Tuple<int, Type>>> SearchFilters { get; set; }
    }

    public interface IQuickSearchService : IService<QuickSearchRequest, QuickSearchModel> {}

    public class QuickSearchService : IQuickSearchService
    {
        private readonly IService<SearchRequest, SearchModel> service;

        public QuickSearchService(IService<SearchRequest, SearchModel> service)
        {
            this.service = service;
        }

        [CanBeAuthorizedBy(AuthorizationDelegate.Search)]
        [NoCache]
        public QuickSearchModel Get(QuickSearchRequest request)
        {
            var searchRequest = new SearchRequest
                                                {
                                                    SearchFilters = request.SearchFilters,
                                                    PageFilter = request.PageFilter,
                                                    MatchContains = request.MatchContains,
                                                    RowCountToReturn = request.RowCountToReturn,
                                                    TextToFind = request.TextToFind,
                                                };

            var searchResults = service.Get(searchRequest);

            var result = new QuickSearchModel
                             {
                                 Schools = (from x in searchResults.Schools
                                            select new QuickSearchModel.SearchItem
                                                       {
                                                           Id = x.SchoolId,
                                                           Text = x.Text,
                                                           Link = x.Link
                                                       }).ToList(),
                                 Teachers = (from x in searchResults.Teachers
                                             select new QuickSearchModel.SearchItem
                                                        {
                                                            Id = x.StaffUSI,
                                                            Text = x.Text,
                                                            Link = x.Link
                                                        }).ToList(),
                                 StudentQuery = searchResults.StudentQuery
                             };
            return result;
        }
    }
}
