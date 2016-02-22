// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.School;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.School
{
    public abstract class When_resolving_a_route_value_to_a_school : TestFixtureBase
    {
        protected int actualSchoolId;
        protected RouteValueResolutionService resolutionService;
        protected const int LOCAL_EDUCATION_AGENCY_ID = 1000;
        protected const int SCHOOL_ID = 1;

        protected override void EstablishContext()
        {
            IQueryable<SchoolInformation> schoolData =
                (new List<SchoolInformation>
                    {
                       // new SchoolInformation { LocalEducationAgencyId = LOCAL_EDUCATION_AGENCY_ID,     SchoolId = SCHOOL_ID,     Name = "-010--LISD-TV--thru-06-07-" }, // This is a decoy value matching supplied route value
                        new SchoolInformation { LocalEducationAgencyId = LOCAL_EDUCATION_AGENCY_ID,     SchoolId = SCHOOL_ID,     Name = " 010  LISD TV  thru 06 07 " }, // This is the targeted school
                        new SchoolInformation { LocalEducationAgencyId = LOCAL_EDUCATION_AGENCY_ID,     SchoolId = SCHOOL_ID + 1,     Name = "Beginning Text _010__LISD_TV__thru_06_07_" },
                        new SchoolInformation { LocalEducationAgencyId = LOCAL_EDUCATION_AGENCY_ID,     SchoolId = SCHOOL_ID + 2,     Name = "_010__LISD_TV__thru_06_07_ Ending Text" },
                        new SchoolInformation { LocalEducationAgencyId = LOCAL_EDUCATION_AGENCY_ID,     SchoolId = SCHOOL_ID + 3,     Name = "Beginning Text _010__LISD_TV__thru_06_07_ Ending Text" },
                        new SchoolInformation { LocalEducationAgencyId = LOCAL_EDUCATION_AGENCY_ID + 1, SchoolId = SCHOOL_ID + 1, Name = "_010__LISD_TV__thru_06_07_" }, // Make sure that we are filtering on LEA Id
                    })
                .AsQueryable();

            var schoolInfoRepo = mocks.StrictMock<IRepository<SchoolInformation>>();
            Expect.Call(schoolInfoRepo.GetAll())
                .Return(schoolData);

            resolutionService = new RouteValueResolutionService(schoolInfoRepo);
        }
    }

    public class When_resolving_a_route_value_to_a_school_when_there_is_a_matching_school_name_in_the_local_education_agency
        : When_resolving_a_route_value_to_a_school
    {
        protected override void ExecuteTest()
        {
            actualSchoolId = resolutionService.Get(RouteValueResolutionRequest.Create(LOCAL_EDUCATION_AGENCY_ID, "-010--LISD-TV--thru-06-07-"));
        }

        [Test]
        public void Should_construct_a_SQL_Server_wildcard_pattern_using_an_underscore_replacement_of_the_hyphens_and_resolve_the_School_id()
        {
            Assert.That(actualSchoolId, Is.EqualTo(SCHOOL_ID));
        }
    }

    public class When_resolving_a_route_value_to_a_school_when_there_is_NOT_a_matching_school_name_in_the_local_education_agency
        : When_resolving_a_route_value_to_a_school
    {
        protected override void ExecuteTest()
        {
            actualSchoolId = resolutionService.Get(RouteValueResolutionRequest.Create(LOCAL_EDUCATION_AGENCY_ID, "NonMatchingRouteValue"));
        }

        [Test]
        public void Should_construct_a_SQL_Server_wildcard_pattern_using_an_underscore_replacement_of_the_hyphens_and_resolve_the_School_id()
        {
            Assert.That(actualSchoolId, Is.EqualTo(0));
        }
    }
}
