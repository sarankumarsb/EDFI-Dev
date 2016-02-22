// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Images;
using EdFi.Dashboards.Resources.Models;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Resources.Models.LocalEducationAgency.Overview;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using AutoMapper;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Common;

namespace EdFi.Dashboards.Resources.LocalEducationAgency
{
    public class OverviewRequest
    {
        public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="OverviewRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="OverviewRequest"/> instance.</returns>
        public static OverviewRequest Create(int localEducationAgencyId) 
        {
            return new OverviewRequest { LocalEducationAgencyId = localEducationAgencyId };
        }
    }

    public abstract class OverviewServiceBase<TRequest, TResponse, TAccountabilityRating> : IService<TRequest, TResponse>
        where TRequest : OverviewRequest
        where TResponse : OverviewModel, new()
        where TAccountabilityRating : AccountabilityRating, new()
    {
        public IRepository<LocalEducationAgencyInformation> LocalEducationAgencyInformationRepository { get; set; }
        public IRepository<LocalEducationAgencyAccountabilityInformation> LocalEducationAgencyAccountabilityInformationRepository { get; set; }
        public ILocalEducationAgencyAreaLinks LocalEducationAgencyAreaLinks { get; set; }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllMetrics, EdFiClaimTypes.ViewMyMetrics)]
        public virtual TResponse Get(TRequest request)
        {
            var leaInformationData = (from a in LocalEducationAgencyInformationRepository.GetAll()
                                      where a.LocalEducationAgencyId == request.LocalEducationAgencyId
                                      select a).ToList();
            var accountabilityRatingsData = (from l in LocalEducationAgencyAccountabilityInformationRepository.GetAll()
                                            where l.LocalEducationAgencyId == request.LocalEducationAgencyId
                                            select l).ToList();
            InitializeMappings();

            var model = Mapper.Map<TResponse>(leaInformationData.Single());

            model.LocalEducationAgencyId = request.LocalEducationAgencyId;
            model.MetricVariantId = (int)LocalEducationAgencyMetricEnum.Overview;
            model.RenderingMode = "Overview";
            model.ProfileThumbnail = LocalEducationAgencyAreaLinks.Image(request.LocalEducationAgencyId);

            var accountabilityRatings = Mapper.Map<List<TAccountabilityRating>>(accountabilityRatingsData);
            model.AccountabilityRatings = accountabilityRatings;
            OnAccountabilityRatingsMapped(model, accountabilityRatingsData);

            OnLocalEducationOverviewMapped(model, leaInformationData);
            return model;
        }

        protected virtual void InitializeMappings()
        {
            AutoMapperHelper.EnsureMapping<LocalEducationAgencyInformation, OverviewModel, TResponse>
                (LocalEducationAgencyInformationRepository,
                 mapping => mapping.ForMember(x => x.LocalEducationAgencyName, opt=>opt.MapFrom(source=>source.Name)),
                 ignore => ignore.MetricVariantId,
                 ignore => ignore.RenderingMode,
                 ignore => ignore.AccountabilityRatings);
                    
           AutoMapperHelper.EnsureMapping<LocalEducationAgencyAccountabilityInformation, AccountabilityRating, TAccountabilityRating>(LocalEducationAgencyAccountabilityInformationRepository);
        }

        protected virtual void OnLocalEducationOverviewMapped(TResponse model, IEnumerable<LocalEducationAgencyInformation> localEducationAgencyInformationData)
        {
        }

        protected virtual void OnAccountabilityRatingsMapped(TResponse model, IEnumerable<LocalEducationAgencyAccountabilityInformation> accountabilityRatingsData)
        {
        }
    }

    public interface IOverviewService : IService<OverviewRequest, OverviewModel> { }

    public sealed class OverviewService : OverviewServiceBase<OverviewRequest, OverviewModel, AccountabilityRating>, IOverviewService { }
}
