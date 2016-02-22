using System.Linq;
using EdFi.Dashboards.Common.Utilities;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;

namespace EdFi.Dashboards.Resources.StudentMetrics
{
    public class DemographicsFilter : IStudentMetricFilter
    {
        protected readonly IRepository<StudentIndicator> StudentIndicatorRepository;

        public DemographicsFilter(IRepository<StudentIndicator> studentIndicatorRepository)
        {
            StudentIndicatorRepository = studentIndicatorRepository;
        }

        public IQueryable<EnhancedStudentInformation> ApplyFilter(IQueryable<EnhancedStudentInformation> query, StudentMetricsProviderQueryOptions providerQueryOptions)
        {
            if (providerQueryOptions.DemographicOptionGroups == null || !providerQueryOptions.DemographicOptionGroups.Any())
                return query;

            const string yes = "YES";

            foreach (var demographicOptionGroup in providerQueryOptions.DemographicOptionGroups)
            {
                if (demographicOptionGroup.SelectedOptions == null || !demographicOptionGroup.SelectedOptions.Any())
                {
                    continue;
                }

                var studentsWithinDemographics = ExpressionExtensions.False<EnhancedStudentInformation>();
                foreach (var selectedOption in demographicOptionGroup.SelectedOptions)
                {
                    var filterValue = selectedOption;
                    switch (filterValue.ToLowerInvariant().Trim())
                    {
                        case "female":
                        case "male":
                            studentsWithinDemographics =
                                studentsWithinDemographics.Or(information => information.Gender == filterValue);
                            break;
                        case "hispanic/latino":
                            studentsWithinDemographics = studentsWithinDemographics.Or(information => information.HispanicLatinoEthnicity == yes);
                            break;
                        case "late enrollment":
                            studentsWithinDemographics = studentsWithinDemographics.Or(information => information.LateEnrollment == yes);
                            break;
                        case "two or more":
                            studentsWithinDemographics = studentsWithinDemographics.Or(
                                information => information.Race.Contains(",") && information.HispanicLatinoEthnicity != yes);
                            break;
                        default:
                            var raceOrStudentIndicatorValue = filterValue;

                            //See if it's a indicator value
                            studentsWithinDemographics =
                                studentsWithinDemographics.Or(
                                    information => StudentIndicatorRepository.GetAll()
                                        .Any(data => data.StudentUSI == information.StudentUSI
                                                     && (data.EducationOrganizationId == information.SchoolId || data.EducationOrganizationId == information.LocalEducationAgencyId)
                                                     && data.Status
                                                     && data.Name == raceOrStudentIndicatorValue));

                            //Or it might be a race value
                            studentsWithinDemographics =
                                studentsWithinDemographics.Or(
                                    information =>
                                        information.Race == raceOrStudentIndicatorValue &&
                                        information.HispanicLatinoEthnicity != yes);
                            break;
                    }
                }

                query = query.Where(studentsWithinDemographics);
            }

            return query;
        }
    }
}