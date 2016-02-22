using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Castle.Core.Logging;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Application.Resources.Models.LocalEducationAgency;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Presentation.Architecture.Mvc.Core;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Application.Resources.LocalEducationAgency
{
    // NOTE: This is a reference implementation for exposing an updatable resource consistent with RESTful design principles
    // but is not actually used by the application (thus the 'DEBUG' conditional compilation).
#if DEBUG
    /// <summary>
    /// Represents a request for the collection-level "PlannedGoals" resource.
    /// </summary>
    public class PlannedGoalsCollectionRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier for the Local Education Agency associated with the request.
        /// </summary>
        public int LocalEducationAgencyId { get; set; }

        /// <summary>
        /// Creates and initializes a new <see cref="PlannedGoalsCollectionRequest"/> instance.
        /// </summary>
        /// <param name="localEducationAgencyId">The unique identifier for the Local Education Agency associated with the request.</param>
        /// <returns>The initialized <see cref="PlannedGoalsCollectionRequest"/> instance.</returns>
        public static PlannedGoalsCollectionRequest Create(int localEducationAgencyId)
        {
            return new PlannedGoalsCollectionRequest {LocalEducationAgencyId = localEducationAgencyId };
        }
    }

    /// <summary>
    /// Represents a request for the item-level "PlannedGoals" resource.
    /// </summary>
    public class PlannedGoalsItemRequest : PlannedGoalsCollectionRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier of the metric associated with the request.
        /// </summary>
        [BindAlias("resourceIdentifier")] // Custom binding from '{resourceIdentifier}' to least significant composite key value.
        public int MetricId { get; set; }

        /// <summary>
        /// Creates and initializes a new <see cref="PlannedGoalsItemRequest"/> instance.
        /// </summary>
        /// <param name="localEducationAgencyId">The unique identifier for the Local Education Agency associated with the request.</param>
        /// <param name="metricId">The unqiue identifier of the metric associated with the request.</param>
        /// <returns>The initialized <see cref="PlannedGoalsItemRequest"/> instance.</returns>
        public static PlannedGoalsItemRequest Create(int localEducationAgencyId, int metricId)
        {
            return new PlannedGoalsItemRequest {LocalEducationAgencyId = localEducationAgencyId, MetricId = metricId };
        }
    }

    /// <summary>
    /// Represents a PUT request for the item-level "PlannedGoals" resource, adding properties for the entity's data values.
    /// </summary>
    public class PlannedGoalsItemPutRequest : PlannedGoalsItemRequest
    {
        /// <summary>
        /// Gets or sets the value of the goal for the organization.
        /// </summary>
        [AuthenticationIgnore("This is the value to be updated.")]
        public decimal Goal { get; set; }

        /// <summary>
        /// Creates and initializes a new <see cref="PlannedGoalsItemPutRequest"/> instance.
        /// </summary>
        /// <param name="localEducationAgencyId">The unique identifier for the Local Education Agency associated with the request.</param>
        /// <param name="metricId">The unqiue identifier of the metric associated with the request.</param>
        /// <param name="goal">The value of the metric goal.</param>
        /// <returns>The initialized <see cref="PlannedGoalsItemRequest"/> instance.</returns>
        public static PlannedGoalsItemPutRequest Create(int localEducationAgencyId, int metricId, decimal goal)
        {
            return new PlannedGoalsItemPutRequest
                       {
                           LocalEducationAgencyId = localEducationAgencyId, 
                           MetricId = metricId,
                           Goal = goal,
                       };
        }
    }

    public interface IPlannedGoalsService : IService<PlannedGoalsCollectionRequest, IEnumerable<PlannedGoalModel>>,
                                        IService<PlannedGoalsItemRequest, PlannedGoalModel>,
                                        IPutHandler<PlannedGoalsItemPutRequest, PlannedGoalModel>,
                                        IDeleteHandler<PlannedGoalsItemRequest> { }

    public class PlannedGoalsService : IPlannedGoalsService
    {
        private readonly IPersistingRepository<EducationOrganizationGoalPlanning> goalPlanningRepo;
        private readonly ILocalEducationAgencyAreaLinks leaLinks;

        private readonly string resourceName;

        #region Logger Property

        /// <summary>
        /// Holds the value for the Logger property.
        /// </summary>
        private ILogger _logger = NullLogger.Instance;

        /// <summary>
        /// Gets or sets the logger instance (set automatically by Castle Windsor due to the logging facility
        /// added during IoC initialization).
        /// </summary>
        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        #endregion

        public PlannedGoalsService(IPersistingRepository<EducationOrganizationGoalPlanning> goalPlanningRepo,
            ILocalEducationAgencyAreaLinks leaLinks)
        {
            this.goalPlanningRepo = goalPlanningRepo;
            this.leaLinks = leaLinks;

            // Get the resource name for this service, based on convention.
            resourceName = this.GetType().GetResourceName();

            // Create AutoMapper mapping, with special handling around the LocalEducationAgencyId -> EducationOrganizationId
            Mapper.CreateMap<PlannedGoalsItemPutRequest, EducationOrganizationGoalPlanning>()
                .ForMember(entity => entity.EducationOrganizationId, 
                    request => request.MapFrom(x => x.LocalEducationAgencyId));
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ManageGoals)]
        public IEnumerable<PlannedGoalModel> Get(PlannedGoalsCollectionRequest request)
        {
            return GetModelProjection(GetCollectionQuery(request)).ToList();
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ManageGoals)]
        public PlannedGoalModel Get(PlannedGoalsItemRequest request)
        {
            return GetModelProjection(GetItemQuery(request)).SingleOrDefault();
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ManageGoals)]
        public PlannedGoalModel Put(PlannedGoalsItemPutRequest request, out bool created)
        {
            // Load the entity
            var entity = GetItemQuery(request).SingleOrDefault();

            if (entity == null)
                entity = new EducationOrganizationGoalPlanning();

            // Apply the values from the request to the entity
            Mapper.Map(request, entity);

            // Save the entity
            goalPlanningRepo.Save(entity, out created);

            // Return the model projection
            return GetModelProjection(entity.ToQueryable()).SingleOrDefault();
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ManageGoals)]
        public void Delete(PlannedGoalsItemRequest request)
        {
            goalPlanningRepo.Delete(x =>
                x.EducationOrganizationId == request.LocalEducationAgencyId
                && x.MetricId == request.MetricId);
        }

        // ------------------------
        // Queries and Projections
        // ------------------------
        private IQueryable<EducationOrganizationGoalPlanning> GetCollectionQuery(PlannedGoalsCollectionRequest request)
        {
            return from pg in goalPlanningRepo.GetAll()
                   where pg.EducationOrganizationId == request.LocalEducationAgencyId
                   select pg;
        }

        private IQueryable<EducationOrganizationGoalPlanning> GetItemQuery(PlannedGoalsItemRequest request)
        {
            return from pg in GetCollectionQuery(request)
                   where pg.MetricId == request.MetricId
                   select pg;
        }

        private IEnumerable<PlannedGoalModel> GetModelProjection(IQueryable<EducationOrganizationGoalPlanning> query)
        {
            return (from pg in query.ToList() // Executes a query that gets all columns in the table (due to with Subsonic projection 
                    select new PlannedGoalModel // ... capabilities related to invoking the URI generation as part of the projection)
                               {
                                   LocalEducationAgencyId = pg.EducationOrganizationId,
                                   MetricId = pg.MetricId,
                                   Goal = pg.Goal,
                                   ResourceUrl = leaLinks.ApiResource(pg.EducationOrganizationId, pg.MetricId, resourceName),
                               });
        }
    }
#endif
}
