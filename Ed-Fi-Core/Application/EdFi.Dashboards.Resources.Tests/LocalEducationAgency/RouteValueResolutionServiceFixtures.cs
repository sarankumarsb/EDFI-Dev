// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.LocalEducationAgency
{
    public abstract class When_resolving_a_route_value_to_a_local_education_agency : TestFixtureBase
    {
        protected const int MATCHING_LEA_ID = 2;

        protected IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency> repository;
        protected RouteValueResolutionService resolutionService;

        protected int actualLocalEducationAgencyId;

        protected override void EstablishContext()
        {
            repository = mocks.StrictMock<IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency>>();
            Expect.Call(repository.GetAll()).Return(GetLocalEducationAgency());

            resolutionService = new RouteValueResolutionService(repository);
        }

        protected IQueryable<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency> GetLocalEducationAgency()
        {
            return (new List<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency>
                        {
                            new EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency { LocalEducationAgencyId = MATCHING_LEA_ID - 1, Code = "First Non-matching Route Value"},
                            new EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency { LocalEducationAgencyId = MATCHING_LEA_ID, Code = "Grand_Bend_ISD"},
                            new EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency { LocalEducationAgencyId = MATCHING_LEA_ID + 1, Code = "Second Non-matching Route Value"},
                        }).AsQueryable();
        }
    }

    [TestFixture]
    public class When_resolving_a_route_value_to_a_local_education_agency_where_there_is_a_matching_name : When_resolving_a_route_value_to_a_local_education_agency
    {
        protected override void ExecuteTest()
        {
            actualLocalEducationAgencyId = resolutionService.Get(RouteValueResolutionRequest.Create("Grand-Bend-ISD"));
        }

        [Test]
        public void Should_construct_a_SQL_Server_wildcard_pattern_using_a_mixed_case_word_breaking_strategy_and_resolve_the_Local_Education_Agency_id()
        {
            Assert.That(actualLocalEducationAgencyId, Is.EqualTo(MATCHING_LEA_ID));
        }
    }

    [TestFixture]
    public class When_resolving_a_route_value_to_a_local_education_agency_where_there_is_NOT_a_matching_name : When_resolving_a_route_value_to_a_local_education_agency
    {
        protected override void ExecuteTest()
        {
            actualLocalEducationAgencyId = resolutionService.Get(RouteValueResolutionRequest.Create("NoMatch"));
        }

        [Test]
        public void Should_fail_to_resolve_to_a_Local_Education_Agency_Id_and_return_a_0_value()
        {
            Assert.That(actualLocalEducationAgencyId, Is.EqualTo(0));
        }
    }
}
