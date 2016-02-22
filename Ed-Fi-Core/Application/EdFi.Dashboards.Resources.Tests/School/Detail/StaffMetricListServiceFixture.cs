// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Metric;
using EdFi.Dashboards.Resources.Metric.MetricInstanceSetKeyResolvers;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Resources.Models.School.Detail;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.School.Detail;
using EdFi.Dashboards.Resources.Tests.Navigation.Fakes;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Resources.Tests.School.Detail
{
    [TestFixture]
    public class When_building_the_detail_staff_list_model_for_the_school : TestFixtureBase
    {
        private IRepository<SchoolMetricTeacherList> schoolMetricTeacherListRepository;
        private IRepository<StaffInformation> staffInformationRepository; 
        private IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository;
        private IRepository<MetricInstanceFootnote> footnoteRepository;
        private IUniqueListIdProvider uniqueListIdProvider;
        private IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private IMetricNodeResolver metricNodeResolver;

        private IQueryable<SchoolMetricTeacherList> suppliedSchoolMetricTeacherList;
        private IQueryable<StaffInformation> suppliedStaffInformationList;
        private IQueryable<StaffEducationOrgInformation> suppliedStaffEducationOrgInformationList;
        private IQueryable<MetricInstanceFootnote> suppliedFootnoteList;

        private const int suppliedSchoolId = 1;
        private const int suppliedMetricId = 2;
        private const int suppliedMetricVariantId = 2999;
        private const string suppliedUniqueListId = "myUniqueId2";
        private readonly Guid suppliedMetricInstanceSetKey = Guid.NewGuid();
        private const string suppliedStringValue = "this is a string value";
        private const string suppliedFootnoteText1 = "supplied footnote 1";
        private const string suppliedFootnoteText2 = "supplied footnote 2";
        private const string suppliedFootnoteText3 = "supplied footnote 3";
        private const string suppliedListFormat = "{0:P3}";
        private const string suppliedListDataLabel = "supplied list data label";

        private IStaffAreaLinks staffAreaLinksFake = new StaffAreaLinksFake();
        private StaffMetricListModel actualModel;

        protected override void EstablishContext()
        {
            schoolMetricTeacherListRepository = mocks.StrictMock<IRepository<SchoolMetricTeacherList>>();
            staffInformationRepository = mocks.StrictMock<IRepository<StaffInformation>>();
            staffEducationOrgInformationRepository = mocks.StrictMock<IRepository<StaffEducationOrgInformation>>();
            footnoteRepository = mocks.StrictMock<IRepository<MetricInstanceFootnote>>();
            uniqueListIdProvider = mocks.StrictMock<IUniqueListIdProvider>();
            metricInstanceSetKeyResolver = mocks.StrictMock<IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest>>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();

            suppliedSchoolMetricTeacherList = GetSuppliedSchoolMetricTeacherList();
            suppliedStaffInformationList = GetSuppliedStaffInformationList();
            suppliedStaffEducationOrgInformationList = GetSuppliedStaffEducationOrgInformationList();
            suppliedFootnoteList = GetSuppliedFootnoteList();

            Expect.Call(metricNodeResolver.GetMetricNodeForSchoolFromMetricVariantId(suppliedSchoolId, suppliedMetricVariantId)).Return(GetSuppliedMetricMetadataNode());
            Expect.Call(schoolMetricTeacherListRepository.GetAll()).Repeat.Any().Return(suppliedSchoolMetricTeacherList);
            Expect.Call(staffEducationOrgInformationRepository.GetAll()).Repeat.Any().Return(suppliedStaffEducationOrgInformationList);
            Expect.Call(staffInformationRepository.GetAll()).Repeat.Any().Return(suppliedStaffInformationList);
            Expect.Call(footnoteRepository.GetAll()).Repeat.Any().Return(suppliedFootnoteList);
            Expect.Call(uniqueListIdProvider.GetUniqueId(suppliedMetricVariantId)).Return(suppliedUniqueListId);
            Expect.Call(metricInstanceSetKeyResolver.GetMetricInstanceSetKey(null))
                .Constraints(
                    new ActionConstraint<SchoolMetricInstanceSetRequest>(x =>
                    {
                        Assert.That(x.SchoolId == suppliedSchoolId);
                        Assert.That(x.MetricVariantId == suppliedMetricVariantId);
                    })
                ).Return(suppliedMetricInstanceSetKey);

        }

        protected IQueryable<SchoolMetricTeacherList> GetSuppliedSchoolMetricTeacherList()
        {
            var list = new List<SchoolMetricTeacherList> { 
                new SchoolMetricTeacherList { MetricId = suppliedMetricId + 1, SchoolId = suppliedSchoolId, StaffUSI = 10, Value = "wrong data"},
                new SchoolMetricTeacherList { MetricId = suppliedMetricId, SchoolId = suppliedSchoolId + 1, StaffUSI = 10, Value = "wrong data"},
                new SchoolMetricTeacherList { MetricId = suppliedMetricId, SchoolId = suppliedSchoolId, StaffUSI = 10, Value = "1.3", ValueType = "System.Double"},
                new SchoolMetricTeacherList { MetricId = suppliedMetricId, SchoolId = suppliedSchoolId, StaffUSI = 30, Value = "1.3", ValueType = "System.Double"},
                new SchoolMetricTeacherList { MetricId = suppliedMetricId, SchoolId = suppliedSchoolId, StaffUSI = 35, Value = suppliedStringValue, ValueType = "System.String"},
            };
            return list.AsQueryable();
        }

        protected IQueryable<StaffInformation> GetSuppliedStaffInformationList()
        {
            var list = new List<StaffInformation> { 
                new StaffInformation { StaffUSI = 10, LastSurname = "Doe", FirstName = "John", FullName = "John Doe", EmailAddress="john.doe@email.com", HighestLevelOfEducationCompleted = "education1", YearsOfPriorProfessionalExperience = 1},
                new StaffInformation { StaffUSI = 30, LastSurname = "Din", FirstName = "Mark", FullName = "Mark Din", EmailAddress="mark.din@email.com", HighestLevelOfEducationCompleted = "education3", YearsOfPriorProfessionalExperience = 3},
                new StaffInformation { StaffUSI = 35, LastSurname = "Bob", FirstName = "Joe", FullName = "Joe Bob", EmailAddress="joe.bob@email.com", HighestLevelOfEducationCompleted = "education4", YearsOfPriorProfessionalExperience = 4},
                new StaffInformation { StaffUSI = 40, LastSurname = "Brown", FirstName = "Doug", FullName = "Doug Brown" },//This one will not be included...
            };
            return list.AsQueryable();
        }

        protected IQueryable<StaffEducationOrgInformation> GetSuppliedStaffEducationOrgInformationList()
        {
            var list = new List<StaffEducationOrgInformation>
                           {
                               new StaffEducationOrgInformation { StaffUSI = 10, EducationOrganizationId = suppliedSchoolId },
                               new StaffEducationOrgInformation { StaffUSI = 30, EducationOrganizationId = suppliedSchoolId },
                               new StaffEducationOrgInformation { StaffUSI = 35, EducationOrganizationId = suppliedSchoolId + 1}
                           };
            return list.AsQueryable();
        }

        protected IQueryable<MetricInstanceFootnote> GetSuppliedFootnoteList()
        {
            var list = new List<MetricInstanceFootnote>
                           {
                               new MetricInstanceFootnote { MetricId = suppliedMetricId + 1, MetricInstanceSetKey = suppliedMetricInstanceSetKey, FootnoteTypeId = 1, FootnoteText = "wrong data"},
                               new MetricInstanceFootnote { MetricId = suppliedMetricId, MetricInstanceSetKey = Guid.NewGuid(), FootnoteText = "wrong data"},
                               new MetricInstanceFootnote { MetricId = suppliedMetricId, MetricInstanceSetKey = suppliedMetricInstanceSetKey, FootnoteTypeId = 2, FootnoteText = suppliedFootnoteText3 },
                               new MetricInstanceFootnote { MetricId = suppliedMetricId, MetricInstanceSetKey = suppliedMetricInstanceSetKey, FootnoteTypeId = 1, FootnoteText = suppliedFootnoteText1 },
                               new MetricInstanceFootnote { MetricId = suppliedMetricId, MetricInstanceSetKey = suppliedMetricInstanceSetKey, FootnoteTypeId = 2, FootnoteText = suppliedFootnoteText2 },
                           };
            return list.AsQueryable();
        }

        protected MetricMetadataNode GetSuppliedMetricMetadataNode()
        {
            return new MetricMetadataNode(null) { MetricId = suppliedMetricId, MetricVariantId = suppliedMetricVariantId, ListFormat = suppliedListFormat, ListDataLabel = suppliedListDataLabel };
        }

        protected override void ExecuteTest()
        {
            var service = new StaffMetricListService(schoolMetricTeacherListRepository, staffInformationRepository, staffEducationOrgInformationRepository, footnoteRepository, uniqueListIdProvider, metricInstanceSetKeyResolver, metricNodeResolver, staffAreaLinksFake);
            actualModel = service.Get(new StaffMetricListRequest()
                                          {
                                              SchoolId = suppliedSchoolId,
                                              MetricVariantId = suppliedMetricVariantId
                                          });
        }

        [Test]
        public void Should_return_a_model_that_is_not_null()
        {
            Assert.That(actualModel, Is.Not.Null);
            Assert.That(actualModel.MetricValueLabel, Is.EqualTo(suppliedListDataLabel));
        }

        [Test]
        public void Should_return_correct_number_of_staff_in_the_list()
        {
            Assert.That(actualModel.StaffMetrics.Count, Is.EqualTo(suppliedSchoolMetricTeacherList.Count(x => x.MetricId == suppliedMetricId && x.SchoolId == suppliedSchoolId)));
        }
        [Test]
        public void Should_return_correct_name_and_information_for_staff()
        {
            var staff = (from cl in suppliedSchoolMetricTeacherList
                           join ci in suppliedStaffInformationList on cl.StaffUSI equals ci.StaffUSI
                           where cl.MetricId == suppliedMetricId && cl.SchoolId == suppliedSchoolId
                           orderby cl.SchoolId
                           select new { cl, ci });
            int i = 0;
            foreach (var suppliedStaff in staff)
            {
                Assert.That(actualModel.StaffMetrics[i].StaffUSI, Is.EqualTo(suppliedStaff.ci.StaffUSI));
                Assert.That(actualModel.StaffMetrics[i].Name, Is.EqualTo(String.Format("{0}, {1}", suppliedStaff.ci.LastSurname, suppliedStaff.ci.FirstName)));
                Assert.That(actualModel.StaffMetrics[i].Education, Is.EqualTo(suppliedStaff.ci.HighestLevelOfEducationCompleted));
                Assert.That(actualModel.StaffMetrics[i].Experience, Is.EqualTo(suppliedStaff.ci.YearsOfPriorProfessionalExperience));
                Assert.That(actualModel.StaffMetrics[i].Email, Is.EqualTo(suppliedStaff.ci.EmailAddress));
                if (suppliedStaff.ci.StaffUSI == 35)
                    Assert.That(actualModel.StaffMetrics[i].Href, Is.EqualTo(String.Empty));
                else
                    Assert.That(actualModel.StaffMetrics[i].Href, Is.EqualTo(staffAreaLinksFake.Default(suppliedStaff.cl.SchoolId, suppliedStaff.ci.StaffUSI, suppliedStaff.ci.FullName, null as int?, null, new { listContext = suppliedUniqueListId })));
                i++;
            }
        }

        [Test]
        public void Should_return_correct_metric_value()
        {
            var metricValues = suppliedSchoolMetricTeacherList.Where(x => x.MetricId == suppliedMetricId && x.ValueType == "System.Double").OrderBy(x => x.StaffUSI);

            int i = 0;
            foreach (var suppliedValues in metricValues)
            {
                Assert.That(actualModel.StaffMetrics[i].Value, Is.EqualTo(Convert.ToDouble(suppliedValues.Value)));
                Assert.That(actualModel.StaffMetrics[i].DisplayValue, Is.EqualTo(string.Format(suppliedListFormat, Convert.ToDouble(suppliedValues.Value))));
                i++;
            }
            Assert.That(actualModel.StaffMetrics[i].Value, Is.EqualTo(suppliedStringValue));
            Assert.That(actualModel.StaffMetrics[i].DisplayValue, Is.EqualTo(suppliedStringValue));
        }

        [Test]
        public void Should_return_correct_footnotes()
        {
            Assert.That(actualModel.MetricFootnotes.Count, Is.EqualTo(2));
            Assert.That(actualModel.MetricFootnotes[0].FootnoteText, Is.EqualTo(suppliedFootnoteText3));
            Assert.That(actualModel.MetricFootnotes[1].FootnoteText, Is.EqualTo(suppliedFootnoteText2));
        }

        [Test]
        public void Should_have_no_unassigned_values_on_presentation_model()
        {
            //In the case of Accommodations they could be null or false that is the default value. This is OK.
            //REfere to the supplied data.
            actualModel.EnsureNoDefaultValues(new [] { "StaffMetricListModel.MetricFootnotes[0].FootnoteNumber",
                                                       "StaffMetricListModel.MetricFootnotes[1].FootnoteNumber"});
        }

        [Test]
        public virtual void Should_have_serializable_model()
        {
            actualModel.EnsureSerializableModel();
        }
    }
}
