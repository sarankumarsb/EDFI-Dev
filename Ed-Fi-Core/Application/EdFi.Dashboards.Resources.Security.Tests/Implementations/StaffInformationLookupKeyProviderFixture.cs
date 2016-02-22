using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Security.Implementations;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Security.Tests.Implementations
{
    public class When_Calling_StaffInformationLookupKeyProvider : TestFixtureBase
    {
        private const string ExpectedValue = "EmailAddress";
        protected ILocalEducationAgencyContextProvider LeaContextProvider;
        protected IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency> LocalEducationAgencyApplicationRepository;
        protected IRepository<Dashboards.Application.Data.Entities.LocalEducationAgencyAuthentication> LocalEducationAgencyApplicationAuthRepository;
        protected string result;

        protected override void EstablishContext()
        {
            LeaContextProvider = mocks.StrictMock<ILocalEducationAgencyContextProvider>();
            LocalEducationAgencyApplicationRepository = mocks.StrictMock<IRepository<EdFi.Dashboards.Application.Data.Entities.LocalEducationAgency>>();
            LocalEducationAgencyApplicationAuthRepository = mocks.StrictMock<IRepository<Dashboards.Application.Data.Entities.LocalEducationAgencyAuthentication>>();


            Expect.Call(LeaContextProvider.GetCurrentLocalEducationAgencyCode()).Repeat.Any().Return("1");

            Expect.Call(LocalEducationAgencyApplicationRepository.GetAll()).Repeat.Any().Return(new List<Dashboards.Application.Data.Entities.LocalEducationAgency>
                                                                                                                      {
                                                                                                                          new Dashboards.Application.Data.Entities.LocalEducationAgency
                                                                                                                              {
                                                                                                                                  LocalEducationAgencyId = 1,
                                                                                                                                  Name = "Test",
                                                                                                                                  Code = "1"
                                                                                                                              }
                                                                                                                      }.AsQueryable());

            Expect.Call(LocalEducationAgencyApplicationAuthRepository.GetAll()).Repeat.Any().Return(new List<Dashboards.Application.Data.Entities.LocalEducationAgencyAuthentication>
                                                                                                        {
                                                                                                            new LocalEducationAgencyAuthentication
                                                                                                                {
                                                                                                                    LocalEducationAgencyId = 1,
                                                                                                                    StaffInformationLookupKey = ExpectedValue,
                                                                                                                    LdapLookupKey = string.Empty
                                                                                                                }
                                                                                                        }.AsQueryable());

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var provider = new StaffInformationLookupKeyProvider(LeaContextProvider, LocalEducationAgencyApplicationRepository,
                                                  LocalEducationAgencyApplicationAuthRepository);

            result = provider.GetStaffInformationLookupKey();
        }

        [Test]
        public void Should_return_the_value_from_the_repo()
        {
            Assert.That(result, Is.EqualTo(ExpectedValue));
        }
    }
}
