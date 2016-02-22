using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.LocalEducationAgency;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.School.Detail;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;
using EdFi.Dashboards.Warehouse.Data.Entities;
using EdFi.Dashboards.Warehouse.Resource.Models.School.Detail;
using EdFi.Dashboards.Warehouse.Resources.Application;

namespace EdFi.Dashboards.Warehouse.Resources.School.Detail
{
    public class StaffPriorYearMetricListRequest
    {
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaffPriorYearMetricListRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="StaffPriorYearMetricListRequest"/> instance.</returns>
        public static StaffPriorYearMetricListRequest Create(int schoolId, int metricVariantId)
        {
            return new StaffPriorYearMetricListRequest { SchoolId = schoolId, MetricVariantId = metricVariantId };
        }
    }

    public interface IStaffPriorYearMetricListService : IService<StaffPriorYearMetricListRequest, StaffPriorYearMetricListModel> { }

    public class StaffPriorYearMetricListService : IStaffPriorYearMetricListService
    {
        private readonly IRepository<SchoolMetricInstanceTeacherList> _schoolMetricInstanceTeacherListRepository;
        private readonly IRepository<StaffInformation> _staffInformationRepository;
        private readonly IRepository<StaffEducationOrgInformation> _staffEdOrgRepository;
        private readonly IUniqueListIdProvider _uniqueListProvider;
        private readonly IMetricNodeResolver _metricNodeResolver;
        private readonly IStaffAreaLinks _staffLinks;
        private readonly ICodeIdProvider _codeIdProvider;
        private readonly ILocalEducationAgencyContextProvider _localEducationAgencyContextProvider;
        private readonly IWarehouseAvailabilityProvider _warehouseAvailabilityProvider;
        private readonly IMaxPriorYearProvider _maxPriorYearProvider;
        private const string NoLongerEnrolledFootnoteFormat = "{0} teachers excluded because they are no longer affiliated.";

        public StaffPriorYearMetricListService(IRepository<SchoolMetricInstanceTeacherList> schoolMetricInstanceTeacherListRepository, 
                                                IRepository<StaffInformation> staffInformationRepository,
                                                IRepository<StaffEducationOrgInformation> staffEdOrgRepository,
                                                IUniqueListIdProvider uniqueListProvider,
                                                IMetricNodeResolver metricNodeResolver, 
                                                IStaffAreaLinks staffLinks,
                                                ICodeIdProvider codeIdProvider,
                                                ILocalEducationAgencyContextProvider localEducationAgencyContextProvider,
                                                IWarehouseAvailabilityProvider warehouseAvailabilityProvider, 
                                                IMaxPriorYearProvider maxPriorYearProvider)
        {
            _schoolMetricInstanceTeacherListRepository = schoolMetricInstanceTeacherListRepository;
            _staffInformationRepository = staffInformationRepository;
            _staffEdOrgRepository = staffEdOrgRepository;
            _uniqueListProvider = uniqueListProvider;
            _metricNodeResolver = metricNodeResolver;
            _staffLinks = staffLinks;
            _codeIdProvider = codeIdProvider;
            _localEducationAgencyContextProvider = localEducationAgencyContextProvider;
            _warehouseAvailabilityProvider = warehouseAvailabilityProvider;
            _maxPriorYearProvider = maxPriorYearProvider;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllTeachers)]
        public StaffPriorYearMetricListModel Get(StaffPriorYearMetricListRequest request)
        {
            var model = new StaffPriorYearMetricListModel();
            if (!_warehouseAvailabilityProvider.Get())
            {
                return model;
            }

            var metricMetadataNode = _metricNodeResolver.GetMetricNodeForSchoolFromMetricVariantId(request.SchoolId, request.MetricVariantId);
            int metricId = metricMetadataNode.MetricId;

            //Get the LEA ID
            var localEducationAgencyId = _codeIdProvider.Get(_localEducationAgencyContextProvider.GetCurrentLocalEducationAgencyCode());
            var year = _maxPriorYearProvider.Get(localEducationAgencyId);

            var priorYearStaffListQuery = from teacherList in _schoolMetricInstanceTeacherListRepository.GetAll()
                          where teacherList.SchoolId == request.SchoolId
                                && teacherList.MetricId == metricId
                                && teacherList.SchoolYear == year
                          select teacherList;

            var priorYearStaffList = priorYearStaffListQuery.ToList();
            var priorYearStaffUSIs = priorYearStaffList.Select(x => x.StaffUSI).ToArray();

            List<StaffInformation> staffList;

            if (priorYearStaffUSIs.Length == 0)
                staffList = new List<StaffInformation>();
            else
                staffList = (from seo in _staffEdOrgRepository.GetAll()
                             join si in _staffInformationRepository.GetAll()
                                on seo.StaffUSI equals si.StaffUSI
                               where seo.EducationOrganizationId == request.SchoolId
                                     && priorYearStaffUSIs.Contains(seo.StaffUSI)
                               select si).ToList();

            // calculate footnote
            if (priorYearStaffList.Count != staffList.Count)
                model.MetricFootnotes.Add(new MetricFootnote
                                                {
                                                    FootnoteNumber = 0,
                                                    FootnoteText = String.Format(NoLongerEnrolledFootnoteFormat, priorYearStaffList.Count - staffList.Count),
                                                    FootnoteTypeId = MetricFootnoteType.DrillDownFootnote
                                                });

            var uniqueId = _uniqueListProvider.GetUniqueId(request.MetricVariantId);

            model.SchoolId = request.SchoolId;
            model.MetricValueLabel = metricMetadataNode.ListDataLabel;
            model.UniqueListId = uniqueId;

            foreach (var staff in staffList)
            {
                var priorYearStaffListData = priorYearStaffList.SingleOrDefault(x => x.StaffUSI == staff.StaffUSI);
                if (priorYearStaffListData == null)
                    continue;

                var staffMetric = new StaffMetricListModel.StaffMetric
                                        {
                                            StaffUSI = staff.StaffUSI,
                                            Name = Utilities.FormatPersonNameByLastName(staff.FirstName, null, staff.LastSurname),
                                            Education = staff.HighestLevelOfEducationCompleted,
                                            Experience = Convert.ToInt32(staff.YearsOfPriorProfessionalExperience),
                                            Email = staff.EmailAddress,
                                            Href = _staffLinks.Default(request.SchoolId, staff.StaffUSI, staff.FullName, null, null, new { uniqueId }),
                                        };

                staffMetric.Value = InstantiateValue.FromValueType(priorYearStaffListData.Value, priorYearStaffListData.ValueType);
                staffMetric.DisplayValue = String.Format(metricMetadataNode.ListFormat, staffMetric.Value);
                model.StaffMetrics.Add(staffMetric);
            }

            return model;
        }
    }
}
