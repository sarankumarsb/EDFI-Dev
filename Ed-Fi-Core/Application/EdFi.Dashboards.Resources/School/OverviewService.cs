// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Linq;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Models.School.Overview;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using AutoMapper;
using System.Collections.Generic;
using EdFi.Dashboards.Common;

namespace EdFi.Dashboards.Resources.School
{
    public class OverviewRequest
    {
        public int SchoolId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverviewRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="OverviewRequest"/> instance.</returns>
        public static OverviewRequest Create(int schoolId)
        {
            return new OverviewRequest { SchoolId = schoolId };
        }
    }

    public abstract class OverviewServiceBase<TRequest, TResponse, TSchoolAccountability, TAccountabilityRating> : IService<TRequest, TResponse>
        where TRequest : OverviewRequest
        where TResponse : OverviewModel, new()
        where TSchoolAccountability : Accountability, new()
        where TAccountabilityRating : AccountabilityRating, new()
    {
        public IService<BriefRequest, BriefModel> SchoolBriefService { get; set; }
        public IRepository<SchoolAccountabilityInformation> SchoolAccountabilityInformationRepository { get; set; }
        public ISchoolAreaLinks SchoolAreaLinks { get; set; }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyMetrics)]
        public TResponse Get(TRequest request)
        {
            //Lets get the basic info...
            var schoolBrief = SchoolBriefService.Get(new BriefRequest { SchoolId = request.SchoolId });

            //Lets go for the accountability data...
            var accountabilityRatingsData = SchoolAccountabilityInformationRepository.GetAll().Where(x => x.SchoolId == request.SchoolId).OrderBy(x => x.DisplayOrder).ToList();

            InitializeMappings();

            var model = new TResponse()
                            {
                                MetricVariantId = (int)SchoolMetricEnum.Overview,
                                RenderingMode = "Overview"
                            };

            //Map as much as possible with Automapper.
            var schoolAccountability = Mapper.Map<TSchoolAccountability>(schoolBrief);

            //Some custom mapping.
            schoolAccountability.ProfileThumbnail = SchoolAreaLinks.Image(request.SchoolId);
            schoolAccountability.Url = SchoolAreaLinks.Overview(request.SchoolId);

            var accountabilityRatings = Mapper.Map<List<TAccountabilityRating>>(accountabilityRatingsData);

            schoolAccountability.AccountabilityRatings = accountabilityRatings as List<AccountabilityRating>;

            model.Accountability = schoolAccountability;

            OnAccountabilityRatingsMapped(model, accountabilityRatingsData);
            OnStudentOverviewMapped(model, schoolBrief, accountabilityRatingsData);

            return model;
        }

        protected virtual void InitializeMappings()
        {
            if (Mapper.FindTypeMapFor(typeof(BriefModel), typeof(TSchoolAccountability)) == null)
            {
                //Defines mapping for the core types.
                Mapper.CreateMap<BriefModel, Accountability>()
                    .ForMember(x => x.AccountabilityRatings, conf => conf.Ignore())
                    .Include(typeof(BriefModel), typeof(TSchoolAccountability));
                //Define mappings for the runtime/(possibly extended) types.
                Mapper.CreateMap(typeof(BriefModel), typeof(TSchoolAccountability));
            }

            AutoMapperHelper.EnsureMapping<SchoolAccountabilityInformation, AccountabilityRating, TAccountabilityRating>(SchoolAccountabilityInformationRepository);
        }

        protected virtual void OnStudentOverviewMapped(TResponse model, BriefModel schoolAccountabilityData, IEnumerable<SchoolAccountabilityInformation> accountabilityRatingsData)
        {
        }

        protected virtual void OnAccountabilityRatingsMapped(TResponse model, IEnumerable<SchoolAccountabilityInformation> accountabilityRatingsData)
        {
        }

    }

    public interface IOverviewService : IService<OverviewRequest, OverviewModel> { }

    public sealed class OverviewService : OverviewServiceBase<OverviewRequest, OverviewModel, Accountability, AccountabilityRating>, IOverviewService { }
}
