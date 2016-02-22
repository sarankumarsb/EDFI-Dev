// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Models.School;
using EdFi.Dashboards.Resources.Models.School.Overview;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.School
{
    [TestFixture]
    public abstract class When_invoking_school_overview_service_get_method<TRequest, TResponse, TService, TSchoolAccountability, TAccountabilityRating> : TestFixtureBase
        where TRequest : OverviewRequest, new()
        where TResponse : OverviewModel, new()
        where TSchoolAccountability : Accountability, new()
        where TAccountabilityRating : AccountabilityRating, new()
        where TService : OverviewServiceBase<TRequest, TResponse, TSchoolAccountability, TAccountabilityRating>, new()
    {
        private TResponse actualModel;

        protected int suppliedSchoolId1 = 1;
        protected BriefModel suppliedSchoolBriefModel;
        protected IQueryable<SchoolAccountabilityInformation> suppliedSchoolAccountabilityInformation;

        private IService<BriefRequest, BriefModel> schoolBriefService;
        private IRepository<SchoolAccountabilityInformation> schoolAccountabilityInformationRepository;
        private ISchoolAreaLinks schoolAreaLinks;

        protected override void EstablishContext()
        {
            schoolBriefService = mocks.StrictMock<IService<BriefRequest, BriefModel>>();
            schoolAccountabilityInformationRepository = mocks.StrictMock<IRepository<SchoolAccountabilityInformation>>();
            schoolAreaLinks = new SchoolAreaLinksFake();

            suppliedSchoolBriefModel = GetSuppliedSchoolBrief();
            suppliedSchoolAccountabilityInformation = GetSuppliedSchoolAccountabilityInformation();



            Expect.Call(schoolBriefService.Get(null))
                .Constraints(
                    new ActionConstraint<BriefRequest>(x => Assert.That(x.SchoolId == suppliedSchoolId1))
                ).Return(suppliedSchoolBriefModel);
            Expect.Call(schoolAccountabilityInformationRepository.GetAll()).Return(suppliedSchoolAccountabilityInformation);

            //Tested Elsewhere.
            //Expect.Call(MetricInstanceSetKeyResolver.GetSchoolMetricInstanceSet(suppliedSchoolId1)).Return(suppliedMetricInstanceSetKey);
            //Expect.Call(metricService.Get(suppliedMetricInstanceSetKey, 1)).Return(new ContainerMetric());
        }

        protected MetricMetadataNode GetSuppliedRootNode()
        {
            return new MetricMetadataNode(new TestMetricMetadataTree())
                       {
                           MetricNodeId = 1
                       };
        }

        protected BriefModel GetSuppliedSchoolBrief()
        {
            return new BriefModel
                       {
                           SchoolId = suppliedSchoolId1,
                           Name = "My School",
                           ProfileThumbnail = "SuppliedImage.Jpg"
                       };
        }

        protected IQueryable<SchoolAccountabilityInformation> GetSuppliedSchoolAccountabilityInformation()
        {
            return (new List<SchoolAccountabilityInformation>
                        {
                            new SchoolAccountabilityInformation{ Attribute = "A", Value = "1", DisplayOrder = 1, SchoolId = suppliedSchoolId1},
                            new SchoolAccountabilityInformation{ Attribute = "C", Value = "3", DisplayOrder = 3, SchoolId = suppliedSchoolId1},
                            new SchoolAccountabilityInformation{ Attribute = "B", Value = "2", DisplayOrder = 2, SchoolId = suppliedSchoolId1},
                            new SchoolAccountabilityInformation{ Attribute = "Should not appear", Value = "99999", DisplayOrder = 1, SchoolId = 999999},
                        }).AsQueryable();
        }

        protected override void ExecuteTest()
        {
            var service = new TService
                              {
                                  SchoolBriefService = schoolBriefService,
                                  SchoolAccountabilityInformationRepository = schoolAccountabilityInformationRepository,
                                  SchoolAreaLinks = schoolAreaLinks
                              };

            var request = new TRequest
                              {
                                  SchoolId = suppliedSchoolId1
                              };

            actualModel = service.Get(request);
        }

        [Test]
        public void Should_return_model_that_is_not_null()
        {
            Assert.That(actualModel, Is.Not.Null);
        }

        [Test]
        public void Should_return_model_with_school_accountability_basic_information()
        {
            Assert.That(actualModel.Accountability.SchoolId, Is.EqualTo(suppliedSchoolBriefModel.SchoolId));
            Assert.That(actualModel.Accountability.Name, Is.EqualTo(suppliedSchoolBriefModel.Name));
            Assert.That(actualModel.Accountability.ProfileThumbnail, Is.EqualTo(schoolAreaLinks.Image(suppliedSchoolId1)));
            Assert.That(actualModel.Accountability.Url, Is.EqualTo(schoolAreaLinks.Overview(suppliedSchoolId1)));
        }

        [Test]
        public void Should_return_model_with_school_accountability_in_order_and_mapped()
        {
            var suppliedAccountabilityInOrder = suppliedSchoolAccountabilityInformation
                                                    .Where(x => x.SchoolId == suppliedSchoolId1)
                                                    .OrderBy(x => x.DisplayOrder).ToList();

            Assert.That(actualModel.Accountability.AccountabilityRatings.Count, Is.EqualTo(suppliedAccountabilityInOrder.Count), "Count does not match.");

            for(var i=0; i<suppliedAccountabilityInOrder.Count(); i++)
            {
                Assert.That(actualModel.Accountability.AccountabilityRatings[i].Attribute, Is.EqualTo(suppliedAccountabilityInOrder[i].Attribute));
                Assert.That(actualModel.Accountability.AccountabilityRatings[i].Value, Is.EqualTo(suppliedAccountabilityInOrder[i].Value));
            }
            
        }

        /* Come back to this when we can test metrics....*/
        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            actualModel.EnsureNoDefaultValues("OverviewModel.Accountability.ResourceUrl", "OverviewModel.Accountability.Links"); //Not needed in this model 
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
        
        [Test]
        public virtual void Should_have_all_autoMapper_mappings_valid()
        {
            AutoMapper.Mapper.AssertConfigurationIsValid();
        }
    }

    public class When_invoking_school_overview_information : When_invoking_school_overview_service_get_method<OverviewRequest, OverviewModel, OverviewService, Accountability, AccountabilityRating>
    { }
}
