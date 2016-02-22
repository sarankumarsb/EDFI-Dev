using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Testing;
using EdFi.Dashboards.Warehouse.Data.Entities;
using EdFi.Dashboards.Warehouse.Resource.Models.School.Detail;
using EdFi.Dashboards.Warehouse.Resources.Application;
using EdFi.Dashboards.Warehouse.Resources.School.Detail;
using NUnit.Framework;
using Rhino.Mocks;

namespace EdFi.Dashboards.Warehouse.Resources.Tests.School.Detail
{
    public abstract class StaffPriorYearMetricListServiceFixtureBase : TestFixtureBase
    {
        protected bool suppliedWarehouseAvailability = true;
        protected bool isStaffCountGreaterThanZero = true;
        protected int suppliedMetricId = 100;
        protected int suppliedMetricIdNotFound = 101;
        protected int suppliedMetricVariantId = 1000;
        protected int suppliedSchoolId = 100;
        protected int suppliedSchoolIdNotFound = 101;
        protected short suppliedMaxYear = 2012;
        protected short suppliedMaxYearNotFound = 2013;
        protected int suppliedStaffUSI1 = 1;
        protected int suppliedStaffUSI2 = 2;
        protected int suppliedStaffUSI3 = 3;
        protected int suppliedNumberOfFootnotes = 0;
        protected int suppliedMissingCount = 0;
        protected string suppliedUniqueId = "UniqueId";

        protected const string FirstName = "FirstName";
        protected const string LastName = "LastName";
        protected const string EmailAddress = "email@email.com";
        protected const string HighestLevelOfEducation = "Doctorate";
        protected const int YearsExperience = 1;
        protected const string NoLongerEnrolledFootnoteFormat = "{0} teachers excluded because they are no longer affiliated.";

        protected IRepository<SchoolMetricInstanceTeacherList> schoolMetricInstanceTeacherListRepository;
        protected IRepository<StaffInformation> staffInformationRepository;
        protected IRepository<StaffEducationOrgInformation> staffEdOrgRepository;
        protected IUniqueListIdProvider uniqueListProvider;
        protected IMetricNodeResolver metricNodeResolver;
        protected IStaffAreaLinks staffLinks;
        protected ICodeIdProvider codeIdProvider;
        protected ILocalEducationAgencyContextProvider localEducationAgencyContextProvider;
        protected IWarehouseAvailabilityProvider warehouseAvailabilityProvider;
        protected IMaxPriorYearProvider maxPriorYearProvider;

        protected readonly List<long> expectedStaffIds = new List<long>();
        protected StaffPriorYearMetricListModel actualModel;

        protected override void EstablishContext()
        {
            schoolMetricInstanceTeacherListRepository = mocks.StrictMock<IRepository<SchoolMetricInstanceTeacherList>>();
            staffInformationRepository = mocks.StrictMock<IRepository<StaffInformation>>();
            staffEdOrgRepository = mocks.StrictMock<IRepository<StaffEducationOrgInformation>>();
            uniqueListProvider = mocks.StrictMock<IUniqueListIdProvider>();
            metricNodeResolver = mocks.StrictMock<IMetricNodeResolver>();
            staffLinks = mocks.StrictMock<IStaffAreaLinks>();
            codeIdProvider = mocks.StrictMock<ICodeIdProvider>();
            localEducationAgencyContextProvider = mocks.StrictMock<ILocalEducationAgencyContextProvider>();
            warehouseAvailabilityProvider = mocks.StrictMock<IWarehouseAvailabilityProvider>();
            maxPriorYearProvider = mocks.StrictMock<IMaxPriorYearProvider>();

            Expect.Call(warehouseAvailabilityProvider.Get()).Return(suppliedWarehouseAvailability);

            if (suppliedWarehouseAvailability)
            {
                Expect.Call(metricNodeResolver.GetMetricNodeForSchoolFromMetricVariantId(-1, -1)).IgnoreArguments().Return(GetMetricMetadataNode());
                Expect.Call(localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode()).Return("Code");
                Expect.Call(codeIdProvider.Get("Code")).Return(1);
                Expect.Call(maxPriorYearProvider.Get(1)).Return(2012);
                Expect.Call(schoolMetricInstanceTeacherListRepository.GetAll()).Return(GetSuppliedSchoolMetricInstanceTeacherList());
                Expect.Call(uniqueListProvider.GetUniqueId(suppliedMetricVariantId)).Return(suppliedUniqueId);


                if (isStaffCountGreaterThanZero)
                {
                    Expect.Call(staffEdOrgRepository.GetAll()).Return(GetSuppliedStaffEdOrg());
                    Expect.Call(staffInformationRepository.GetAll()).Return(GetStaffInformation());
                    Expect.Call(staffLinks.Default(-1, -1, string.Empty, null, null, null)).IgnoreArguments().Return(string.Empty);
                }
            }

            base.EstablishContext();
        }

        protected override void ExecuteTest()
        {
            var service = new StaffPriorYearMetricListService(schoolMetricInstanceTeacherListRepository, staffInformationRepository, staffEdOrgRepository, uniqueListProvider, metricNodeResolver, staffLinks, codeIdProvider, localEducationAgencyContextProvider, warehouseAvailabilityProvider, maxPriorYearProvider);

            actualModel = service.Get(new StaffPriorYearMetricListRequest { MetricVariantId = suppliedMetricVariantId, SchoolId = suppliedSchoolId });
        }

        [Test]
        public void Should_load_correct_staff()
        {
            Assert.That(actualModel.StaffMetrics.Count, Is.EqualTo(expectedStaffIds.Count));

            foreach (var staff in actualModel.StaffMetrics)
                Assert.That(expectedStaffIds.Contains(staff.StaffUSI), Is.True);
        }

        [Test]
        public void Should_have_unique_list_id_populated()
        {
            Assert.That(actualModel.UniqueListId, Is.EqualTo(suppliedUniqueId));
        }

        [Test]
        public void Should_load_correct_footnotes()
        {
            Assert.That(actualModel.MetricFootnotes.Count, Is.EqualTo(suppliedNumberOfFootnotes));

            if (suppliedNumberOfFootnotes > 0)
                Assert.That(actualModel.MetricFootnotes[0].FootnoteText, Is.EqualTo(string.Format(NoLongerEnrolledFootnoteFormat, suppliedMissingCount)));
        }

        protected virtual IQueryable<SchoolMetricInstanceTeacherList> GetSuppliedSchoolMetricInstanceTeacherList()
        {
            return new List<SchoolMetricInstanceTeacherList>
                       {
                           new SchoolMetricInstanceTeacherList
                               {
                                   MetricId = suppliedMetricIdNotFound,
                                   SchoolId = suppliedSchoolId,
                                   SchoolYear = suppliedMaxYear
                               },

                           new SchoolMetricInstanceTeacherList
                               {
                                   MetricId = suppliedMetricId,
                                   SchoolId = suppliedSchoolIdNotFound,
                                   SchoolYear = suppliedMaxYear
                               },

                               new SchoolMetricInstanceTeacherList
                                   {
                                       MetricId = suppliedMetricId,
                                       SchoolId = suppliedSchoolId,
                                       SchoolYear = suppliedMaxYearNotFound                                       
                                   },

                            new SchoolMetricInstanceTeacherList
                                {
                                    MetricId = suppliedMetricId,
                                    SchoolId = suppliedSchoolId,
                                    SchoolYear = suppliedMaxYear,
                                    StaffUSI = suppliedStaffUSI1,
                                    Value = "0.750",
                                    ValueType = "System.Double"
                                }

                       }.AsQueryable();
        }

        protected virtual IQueryable<StaffEducationOrgInformation> GetSuppliedStaffEdOrg()
        {
            return new List<StaffEducationOrgInformation>
                       {
                           new StaffEducationOrgInformation
                               {
                                   StaffUSI = suppliedStaffUSI2,
                                   EducationOrganizationId = suppliedSchoolId
                               },

                           new StaffEducationOrgInformation
                               {
                                   StaffUSI = suppliedStaffUSI1,
                                   EducationOrganizationId = suppliedSchoolIdNotFound
                               },

                           new StaffEducationOrgInformation
                               {
                                   StaffUSI = suppliedStaffUSI1,
                                   EducationOrganizationId = suppliedSchoolId
                               }
                       }.AsQueryable();
        }

        protected virtual IQueryable<StaffInformation> GetStaffInformation()
        {
            return new List<StaffInformation>
                       {
                           new StaffInformation
                               {
                                   StaffUSI = suppliedStaffUSI2
                               },

                           new StaffInformation
                               {
                                   StaffUSI = suppliedStaffUSI1,
                                   FirstName = FirstName,
                                   LastSurname = LastName,
                                   FullName = string.Format("{0} {1}", FirstName, LastName),
                                   HighestLevelOfEducationCompleted = HighestLevelOfEducation,
                                   YearsOfPriorProfessionalExperience = YearsExperience,
                                   EmailAddress = EmailAddress,
                               }
                       }.AsQueryable();
        }

        private MetricMetadataNode GetMetricMetadataNode()
        {
            return new MetricMetadataNode(null)
                       {
                           MetricId = 100,
                           ListDataLabel = "Prior Year",
                           ListFormat = "{0:P0}"
                       };
        }
    }

    public class When_loading_prior_year_staff_metric_list : StaffPriorYearMetricListServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            expectedStaffIds.Clear();
            expectedStaffIds.Add(suppliedStaffUSI1);

            base.EstablishContext();
        }

        [Test]
        public void Dummy()
        {
            
        }
    }

    public class When_loading_prior_year_staff_metric_list_with_some_teachers_not_in_school : StaffPriorYearMetricListServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedNumberOfFootnotes = 1;
            suppliedMissingCount = 1;

            expectedStaffIds.Clear();
            expectedStaffIds.Add(suppliedStaffUSI1);

            base.EstablishContext();
        }

        protected override IQueryable<SchoolMetricInstanceTeacherList> GetSuppliedSchoolMetricInstanceTeacherList()
        {
            var teacherList = base.GetSuppliedSchoolMetricInstanceTeacherList().ToList();
            teacherList.Add(new SchoolMetricInstanceTeacherList
                                {
                                    MetricId = suppliedMetricId,
                                    SchoolId = suppliedSchoolId,
                                    SchoolYear = suppliedMaxYear,
                                    StaffUSI = suppliedStaffUSI3,
                                    Value = "0.800",
                                    ValueType = "System.Double"
                                });
            return (teacherList as IList<SchoolMetricInstanceTeacherList>).AsQueryable();
        }

        [Test]
        public void Dummy()
        {

        }
    }

    public class When_loading_prior_year_staff_metric_list_with_no_staff : StaffPriorYearMetricListServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            isStaffCountGreaterThanZero = false;
            expectedStaffIds.Clear();
            suppliedUniqueId = string.Empty;

            base.EstablishContext();
        }

        protected override IQueryable<SchoolMetricInstanceTeacherList> GetSuppliedSchoolMetricInstanceTeacherList()
        {
            return new List<SchoolMetricInstanceTeacherList>().AsQueryable();   
        }

        [Test]
        public void Dummy()
        {

        }
    }

    public class When_loading_prior_year_staff_metric_list_but_warehouse_is_unavailable : StaffPriorYearMetricListServiceFixtureBase
    {
        protected override void EstablishContext()
        {
            suppliedWarehouseAvailability = false;
            expectedStaffIds.Clear();
            suppliedUniqueId = null;

            base.EstablishContext();
        }

        [Test]
        public void Dummy()
        {

        }
    }
   }
