// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Providers;
using EdFi.Dashboards.Resources.Common;
using EdFi.Dashboards.Resources.Metric.Requests;
using EdFi.Dashboards.Resources.Models.School.Detail;
using EdFi.Dashboards.Resources.Navigation;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Resources.Security.Common;
using EdFi.Dashboards.SecurityTokenService.Authentication;

namespace EdFi.Dashboards.Resources.School.Detail
{
    public class StaffMetricListRequest
    {
        public int SchoolId { get; set; }
        public int MetricVariantId { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="StaffMetricListRequest"/> using the specified parameters.
        /// </summary>
        /// <returns>A new <see cref="StaffMetricListRequest"/> instance.</returns>
        public static StaffMetricListRequest Create(int schoolId, int metricVariantId) 
        {
            return new StaffMetricListRequest { SchoolId = schoolId, MetricVariantId = metricVariantId };
        }
    }

    public interface IStaffMetricListService : IService<StaffMetricListRequest, StaffMetricListModel> { }

    public class StaffMetricListService : IStaffMetricListService
    {
        private readonly IRepository<SchoolMetricTeacherList> schoolMetricTeacherListRepository;
        private readonly IRepository<StaffInformation> staffInformationRepository;
        private readonly IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository; 
        private readonly IRepository<MetricInstanceFootnote> metricInstanceFootnoteRepository;
        private readonly IUniqueListIdProvider uniqueListProvider;
        private readonly IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver;
        private readonly IMetricNodeResolver metricNodeResolver;
        private readonly IStaffAreaLinks staffLinks;

        public StaffMetricListService(IRepository<SchoolMetricTeacherList> schoolMetricTeacherListRepository, 
                                        IRepository<StaffInformation> staffInformationRepository, 
                                        IRepository<StaffEducationOrgInformation> staffEducationOrgInformationRepository,
                                        IRepository<MetricInstanceFootnote> metricInstanceFootnoteRepository,
                                        IUniqueListIdProvider uniqueListProvider, 
                                        IMetricInstanceSetKeyResolver<SchoolMetricInstanceSetRequest> metricInstanceSetKeyResolver,
                                        IMetricNodeResolver metricNodeResolver,
                                        IStaffAreaLinks staffLinks)
        {
            this.schoolMetricTeacherListRepository = schoolMetricTeacherListRepository;
            this.staffInformationRepository = staffInformationRepository;
            this.staffEducationOrgInformationRepository = staffEducationOrgInformationRepository;
            this.metricInstanceFootnoteRepository = metricInstanceFootnoteRepository;
            this.uniqueListProvider = uniqueListProvider;
            this.metricInstanceSetKeyResolver = metricInstanceSetKeyResolver;
            this.metricNodeResolver = metricNodeResolver;
            this.staffLinks = staffLinks;
        }

        [CanBeAuthorizedBy(EdFiClaimTypes.ViewAllTeachers)]
        public StaffMetricListModel Get(StaffMetricListRequest request)
        {
            int schoolId = request.SchoolId;
            int metricVariantId = request.MetricVariantId;
            var metricMetadataNode = metricNodeResolver.GetMetricNodeForSchoolFromMetricVariantId(schoolId, metricVariantId);
            int metricId = metricMetadataNode.MetricId;

            if (String.IsNullOrEmpty(metricMetadataNode.ListFormat))
                throw new ArgumentNullException(string.Format("List format is null for metric variant Id:{0}", metricVariantId));

            var results = from teacherList in schoolMetricTeacherListRepository.GetAll()
                          join staffInformation in staffInformationRepository.GetAll() on teacherList.StaffUSI equals staffInformation.StaffUSI
                          where teacherList.SchoolId == schoolId 
                                && teacherList.MetricId == metricId
                          select new { teacherList, staffInformation };

            var schoolStaffUSIs = (from staffEducationOrg in staffEducationOrgInformationRepository.GetAll()
                                   where staffEducationOrg.EducationOrganizationId == schoolId
                                   select staffEducationOrg.StaffUSI).ToList();

            var metricInstanceSetKey = metricInstanceSetKeyResolver.GetMetricInstanceSetKey(SchoolMetricInstanceSetRequest.Create(schoolId, metricVariantId));
            var footnotes = (from data in metricInstanceFootnoteRepository.GetAll()
                             where data.MetricInstanceSetKey == metricInstanceSetKey 
                                    && data.MetricId == metricId 
                                    && data.FootnoteTypeId == (int)MetricFootnoteType.DrillDownFootnote
                             select data).ToList();


            var listContext = uniqueListProvider.GetUniqueId(metricVariantId);
            var model = new StaffMetricListModel
                            {
                                SchoolId = schoolId,
                                UniqueListId = listContext,
                                MetricValueLabel = metricMetadataNode.ListDataLabel,
                                MetricFootnotes = footnotes.Select(x => new MetricFootnote
                                                                             {
                                                                                 FootnoteTypeId = (MetricFootnoteType)x.FootnoteTypeId,
                                                                                 FootnoteText = x.FootnoteText
                                                                             }).ToList()
                            };


            foreach (var result in results)
            {
                bool generateLink = schoolStaffUSIs.Contains(result.teacherList.StaffUSI);
                var staffMetric = new StaffMetricListModel.StaffMetric
                                            {
                                                StaffUSI = result.teacherList.StaffUSI,
                                                Name = Utilities.FormatPersonNameByLastName(result.staffInformation.FirstName, null, result.staffInformation.LastSurname),
                                                Education = result.staffInformation.HighestLevelOfEducationCompleted,
                                                Experience = Convert.ToInt32(result.staffInformation.YearsOfPriorProfessionalExperience),
                                                Email = result.staffInformation.EmailAddress,
                                                Href = generateLink ? staffLinks.Default(schoolId, result.teacherList.StaffUSI, result.staffInformation.FullName, null, null, new { listContext }) : String.Empty,
                                            };

                staffMetric.Value = InstantiateValue.FromValueType(result.teacherList.Value, result.teacherList.ValueType);
                staffMetric.DisplayValue = String.Format(metricMetadataNode.ListFormat, staffMetric.Value);
                model.StaffMetrics.Add(staffMetric);
            }

            return model;
        }
    }
}
