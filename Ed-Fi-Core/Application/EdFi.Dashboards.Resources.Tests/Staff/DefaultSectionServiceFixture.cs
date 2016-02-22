using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.Staff;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Staff;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.Staff
{
    public abstract class DefaultSectionServiceFixture : TestFixtureBase
    {
        protected IStaffAreaLinks staffAreaLinks;
        protected DefaultSectionRequest suppliedDefaultSectionRequest;
        protected DefaultSectionModel actualModel;

        protected override void EstablishContext()
        {
            staffAreaLinks = mocks.StrictMock<IStaffAreaLinks>();

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var defaultSectionService = new DefaultSectionService(staffAreaLinks);
            actualModel = defaultSectionService.Get(suppliedDefaultSectionRequest);
        }
    }

    [TestFixture]
    public class When_view_type_is_not_in_switch_statement : DefaultSectionServiceFixture
    {
        protected override void ExecuteTest()
        {
            suppliedDefaultSectionRequest = new DefaultSectionRequest
                                                {
                                                    StudentListType = StudentListType.Section.ToString(),
                                                    ViewType = StaffModel.ViewType.AssessmentDetails
                                                };
            base.ExecuteTest();
        }

        [Test]
        public void VerifyModelLink()
        {
            Assert.AreEqual(actualModel.Link, string.Empty);
        }
    }

    [TestFixture]
    public class When_view_type_is_in_switch_statement : DefaultSectionServiceFixture
    {
        protected const string VerifiedUrl = "www.google.com";

        protected override void EstablishContext()
        {
            base.EstablishContext();

            Expect.Call(staffAreaLinks.GeneralOverview(-1, -1, null, null, null, null)).IgnoreArguments().Return(VerifiedUrl);
        }

        protected override void ExecuteTest()
        {
            suppliedDefaultSectionRequest = new DefaultSectionRequest
                                            {
                                                StudentListType = StudentListType.Section.ToString(),
                                                ViewType = StaffModel.ViewType.GeneralOverview
                                            };

            base.ExecuteTest();
        }

        [Test]
        public void VerifyModelLink()
        {
            Assert.AreEqual(actualModel.Link, VerifiedUrl);
        }
    }
}
