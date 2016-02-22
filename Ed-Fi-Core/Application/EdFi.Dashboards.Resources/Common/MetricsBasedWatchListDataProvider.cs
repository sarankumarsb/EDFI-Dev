using System.Globalization;
using EdFi.Dashboards.Application.Data.Entities;
using EdFi.Dashboards.Data.Entities;
using EdFi.Dashboards.Data.Repository;
using EdFi.Dashboards.Resources.Models.Common;
using EdFi.Dashboards.Resources.Models.CustomGrid;
using EdFi.Dashboards.Resources.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Resources.StudentMetrics;

namespace EdFi.Dashboards.Resources.Common
{
    /// <summary>
    /// The interface for retrieving metrics based watch list data.
    /// </summary>
    public interface IMetricsBasedWatchListDataProvider
    {
        /// <summary>
        /// Gets the watch list data for a particular watch list.
        /// </summary>
        /// <param name="watchListId">The watch list identifier.</param>
        /// <returns>The requested watch list record.</returns>
        MetricBasedWatchList GetWatchListData(long? watchListId);

        /// <summary>
        /// Gets the watch list selection data for a particular watch list.
        /// </summary>
        /// <param name="watchListId">The watch list identifier.</param>
        /// <returns>The requested watch list selections.</returns>
        List<NameValuesType> GetWatchListSelectionData(long? watchListId);

        /// <summary>
        /// Gets the EdFiGridWatchListModel; the structure will be determined
        /// by the logged in user.
        /// </summary>
        /// <param name="staffUSI">The staff usi.</param>
        /// <param name="schoolId">The school identifier.</param>
        /// <param name="localEducationAgencyId">The local education agency identifier.</param>
        /// <param name="sectionId">The section identifier this is really the watch list id.</param>
        /// <param name="watchListSelections">The watch list selections.</param>
        /// <returns>
        /// An <see cref="EdFiGridWatchListModel" /> loaded with data for the currently logged in user.
        /// </returns>
        EdFiGridWatchListModel GetEdFiGridWatchListModel(long staffUSI, int? schoolId, int? localEducationAgencyId = null, long? sectionId = null, List<NameValuesType> watchListSelections = null);
    }

    /// <summary>
    /// Retrieves the model used to build the metrics based watch list.
    /// </summary>
    public class MetricsBasedWatchListDataProvider : IMetricsBasedWatchListDataProvider
    {
        /// <summary>
        /// The section description format used to format the section name.
        /// </summary>
        protected const string SectionDescriptionFormat = "{0}({1}) - {2} ({3}) {4}";

        /// <summary>
        /// The teacher section repository used to get a teachers classes.
        /// </summary>
        protected IRepository<TeacherSection> TeacherSectionRepository;

        /// <summary>
        /// The watch list repository used to get watch list data.
        /// </summary>
        protected readonly IRepository<MetricBasedWatchList> WatchListRepository;

        /// <summary>
        /// The watch list selections repository used to get the selections made on a watch list.
        /// </summary>
        protected readonly IRepository<MetricBasedWatchListSelectedOption> WatchListSelectionsRepository;

        /// <summary>
        /// The student metrics provider.
        /// </summary>
        protected readonly IStudentMetricsProvider StudentMetricsProvider;

        /// <summary>
        /// The watch list link provider.
        /// </summary>
        protected readonly IWatchListLinkProvider WatchListLinkProvider;

        /// <summary>
        /// The general area links used to produce url's.
        /// </summary>
        protected readonly IGeneralLinks GeneralLinks;

        private readonly ITabFactory TabFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsBasedWatchListDataProvider" /> class.
        /// </summary>
        /// <param name="teacherSectionRepository">The teacher section repository is passed by the IOC container.</param>
        /// <param name="watchListRepository">The watch list repository is passed by the IOC container.</param>
        /// <param name="watchListSelectionsRepository">The watch list selections repository is passed by the IOC container.</param>
        /// <param name="studentMetricsProvider">The student metrics provider is passed by the IOC container.</param>
        /// <param name="watchListLinkProvider">The watch list search link provider.</param>
        /// <param name="generalLinks">The staff area links is passed by the IOC container.</param>
        public MetricsBasedWatchListDataProvider(
            IRepository<TeacherSection> teacherSectionRepository,
            IRepository<MetricBasedWatchList> watchListRepository,
            IRepository<MetricBasedWatchListSelectedOption> watchListSelectionsRepository,
            IStudentMetricsProvider studentMetricsProvider,
            IWatchListLinkProvider watchListLinkProvider,
            IGeneralLinks generalLinks,
            ITabFactory tabFactory)
        {
            TeacherSectionRepository = teacherSectionRepository;
            WatchListRepository = watchListRepository;
            WatchListSelectionsRepository = watchListSelectionsRepository;
            StudentMetricsProvider = studentMetricsProvider;
            WatchListLinkProvider = watchListLinkProvider;
            GeneralLinks = generalLinks;
            TabFactory = tabFactory;
        }


        /// <summary>
        /// Gets the watch list data for a particular watch list.
        /// </summary>
        /// <param name="watchListId">The watch list identifier.</param>
        /// <returns>
        /// The requested watch list record.
        /// </returns>
        public MetricBasedWatchList GetWatchListData(long? watchListId)
        {
            return WatchListRepository.GetAll()
                .Where(data => data.MetricBasedWatchListId == watchListId)
                .Select(data => new MetricBasedWatchList
                {
                    EducationOrganizationId = data.EducationOrganizationId,
                    MetricBasedWatchListId = data.MetricBasedWatchListId,
                    StaffUSI = data.StaffUSI,
                    WatchListName = data.WatchListName,
                    WatchListDescription = data.WatchListDescription,
                    IsWatchListShared = data.IsWatchListShared
                }).SingleOrDefault();
        }

        /// <summary>
        /// Gets the watch list selection data for a particular watch list.
        /// </summary>
        /// <param name="watchListId">The watch list identifier.</param>
        /// <returns>
        /// The requested watch list selections.
        /// </returns>
        public List<NameValuesType> GetWatchListSelectionData(long? watchListId)
        {
            return WatchListSelectionsRepository.GetAll()
                .Where(data => data.MetricBasedWatchListId == watchListId)
                .GroupBy(group => group.Name).ToList()
                .Select(data => new NameValuesType
                {
                    Name = data.Key,
                    Values = data.Select(select => select.Value).ToList()
                }).ToList();
        }

        /// <summary>
        /// Gets the EdFiGridWatchListModel; the structure will be determined
        /// by the logged in user.
        /// </summary>
        /// <param name="staffUSI">The staff USI.</param>
        /// <param name="schoolId">The school identifier.</param>
        /// <param name="localEducationAgencyId">The local education agency identifier.</param>
        /// <param name="sectionId">The section identifier.</param>
        /// <param name="watchListSelections">The watch list selections.</param>
        /// <returns>
        /// An <see cref="EdFiGridWatchListModel" /> loaded with data for the currently logged in user.
        /// </returns>
        public virtual EdFiGridWatchListModel GetEdFiGridWatchListModel(long staffUSI, int? schoolId, int? localEducationAgencyId = null, long? sectionId = null, List<NameValuesType> watchListSelections = null)
        {
            var watchListDataState = new WatchListDataState();

            var defaultStudentMetricsProviderOptions = new StudentMetricsProviderQueryOptions
            {
                StaffUSI = staffUSI,
                SchoolId = schoolId,
                LocalEducationAgencyId = localEducationAgencyId,
                GetAllMetrics = true
            };

            watchListDataState.Students = GetStudentList(defaultStudentMetricsProviderOptions);
            watchListDataState.Metrics = GetStudentMetrics(defaultStudentMetricsProviderOptions);

            var grades = watchListDataState.Students.Select(data => new { data.GradeLevel, data.GradeLevelSortOrder }).Distinct().OrderBy(order => order.GradeLevelSortOrder).ToList();
            watchListDataState.Grades = grades.Select(data => new EdFiGridWatchListSelectionItemModel { DisplayValue = data.GradeLevel, Name = data.GradeLevel }).ToList();

            watchListDataState.MetricIds = watchListDataState.Metrics.Select(data => data.MetricId).Distinct().ToList();

            var results = new List<TeacherSection>();

            if (schoolId.HasUsableValue())
            {
                results = (TeacherSectionRepository.GetAll()
                    .Where(data => data.StaffUSI == staffUSI && data.SchoolId == schoolId)
                    .OrderBy(data => data.SubjectArea).ThenBy(data => data.CourseTitle)
                    .ThenBy(data => data.ClassPeriod).ThenBy(data => data.LocalCourseCode)
                    .ThenBy(data => data.TeacherSectionId)).ToList();
            }

            if (sectionId.HasUsableValue())
            {
                watchListDataState.CurrentSectionId = sectionId.Value;
            }

            watchListDataState.Sections = results.ToDictionary(
                key => key.TeacherSectionId,
                value => String.Format(SectionDescriptionFormat, value.SubjectArea, value.LocalCourseCode, value.CourseTitle, value.ClassPeriod, value.TermType)
            );

            // determine if the section id has a value and if that value is
            // contained in the sections dictionary above; if not the section
            // id is really a watch list id
            var isWatchListSection = sectionId.HasUsableValue() && (!(sectionId.Value <= int.MaxValue ? (int?) sectionId.Value : null).HasValue || !watchListDataState.Sections.ContainsKey((int) sectionId.Value));

            MetricBasedWatchList watchList = null;
            List<NameValuesType> watchListSelectedOptions = null;

            if (isWatchListSection)
            {
                watchList = GetWatchListData(sectionId.Value);
                watchListSelectedOptions = GetWatchListSelectionData(sectionId.Value);
            }

            var isWatchListChanged = false;
            // if this parameter is set then we are coming back from a watch
            // list and need to reset to the last values selected
            if (watchListSelections != null)
            {
                isWatchListChanged = IsWatchListChanged(watchListSelections, watchListSelectedOptions);
                watchListSelectedOptions = watchListSelections;
            }

            var searchLinkRequest = new WatchListLinkRequest
            {
                LocalEducationAgencyId = localEducationAgencyId,
                SchoolId = schoolId,
                StaffUSI = staffUSI,
                MetricBasedWatchListId = (int?)sectionId,
                ResourceName = "MetricsBasedWatchListSearch"
            };

            // create the watch list model
            var watchListModel = new EdFiGridWatchListModel
            {
                WatchListName = (watchList != null && watchList.WatchListName != string.Empty ? watchList.WatchListName : "New Dynamic List"),
                WatchListDescription = (watchList != null ? watchList.WatchListDescription : string.Empty),
                WatchListUrl = GeneralLinks.MetricsBasedWatchList("MetricsBasedWatchList"),
                WatchListSearchUrl = WatchListLinkProvider.GenerateLink(searchLinkRequest),
                IsWatchListChanged = isWatchListChanged,
                IsWatchListShared = watchList != null && watchList.IsWatchListShared,
                Tabs = TabFactory.CreateAllTabs(watchListDataState)
            };

            // load any available selections into the model
            if (watchListSelectedOptions != null)
            {
                LoadWatchListSelections(watchListModel, watchListSelectedOptions);
            }

            return watchListModel;
        }

        /// <summary>
        /// Determines whether a watch list has changed based upon the
        /// specified current selections.
        /// </summary>
        /// <param name="currentSelections">The current selections.</param>
        /// <param name="databaseSelections">The database selections.</param>
        /// <returns>A boolean indicating if the current selections are different from those in the database.</returns>
        private static bool IsWatchListChanged(ICollection<NameValuesType> currentSelections, ICollection<NameValuesType> databaseSelections)
        {
            // the watch list has not been saved to the database so return
            // false
            if (databaseSelections == null)
                return false;

            // if the number of selected metrics are different then the list
            // has changed
            if (currentSelections.Count != databaseSelections.Count)
                return true;

            // if there is an item name in the current selections that is not
            // contained in the database selections then the list has changed
            if (currentSelections.Any(currentSelection => databaseSelections.Any(data => data.Name != currentSelection.Name)))
            {
                return true;
            }

            // now that we know the names in each collection are the same the
            // count of the values for each name needs to be checked
            if (currentSelections.Any(currentSelection => databaseSelections.Single(selection => selection.Name == currentSelection.Name).Values.Count != currentSelection.Values.Count))
            {
                return true;
            }

            // we will only get here if the number of selections are the same,
            // and the names in each list are the same, and the counts of the
            // selected values in each list are the same
            // when this is the case each value needs to be checked to
            // determine if any of the values are different
            return
                currentSelections.Any(
                    currentSelection =>
                        databaseSelections.Single(selection => selection.Name == currentSelection.Name)
                            .Values.Any(databaseValue => currentSelection.Values.All(value => value != databaseValue)));
        }

        #region Model Loading Methods

        /// <summary>
        /// Loads the watch list with the saved selections.
        /// </summary>
        /// <param name="model">The watch list model.</param>
        /// <param name="selectedOptions">The selected options.</param>
        /// <returns>An <see cref="EdFiGridWatchListModel"/> with the selections loaded.</returns>
        protected virtual EdFiGridWatchListModel LoadWatchListSelections(EdFiGridWatchListModel model, List<NameValuesType> selectedOptions)
        {
            // this code will get the view models for all of the templates in
            // all of the columns in all of the tabs
            var viewModels = new List<object>();
            foreach (var column in model.Tabs.SelectMany(tab => tab.Columns))
            {
                viewModels.AddRange(column.Templates.GetViewModelsFromTemplates());
            }

            // loop through all of the view models and set the IsSelected
            // property to true where selections were made
            foreach (var viewModel in viewModels)
            {
                switch (viewModel.GetType().Name)
                {
                    case "EdFiGridWatchListSingleSelectionModel":
                        var singleSelectionModel = viewModel as EdFiGridWatchListSingleSelectionModel;

                        if (singleSelectionModel != null)
                        {
                            if (selectedOptions.Any(data => data.Name == singleSelectionModel.Name))
                            {
                                var selectedOption = selectedOptions.SingleOrDefault(data => data.Name == singleSelectionModel.Name);

                                if (selectedOption != null)
                                {
                                    foreach (var singleSelectionModelValue in selectedOption.Values.Select(value => singleSelectionModel.Values.SingleOrDefault(data => data.Name == value)).Where(singleSelectionModelValue => singleSelectionModelValue != null))
                                    {
                                        singleSelectionModelValue.IsSelected = true;
                                    }
                                }
                            }
                        }
                        break;
                    case "EdFiGridWatchListDoubleSelectionModel":
                        var doubleDropDownModel = viewModel as EdFiGridWatchListDoubleSelectionModel;

                        if (doubleDropDownModel != null)
                        {
                            if (selectedOptions.Any(data => data.Name == doubleDropDownModel.Name))
                            {
                                var selectedOption = selectedOptions.SingleOrDefault(data => data.Name == doubleDropDownModel.Name);

                                if (selectedOption != null)
                                {
                                    foreach (var value in selectedOption.Values)
                                    {
                                        var comparison = doubleDropDownModel.Comparisons.SingleOrDefault(data => data.Name == value);
                                        if (comparison != null)
                                        {
                                            comparison.IsSelected = true;
                                            continue;
                                        }

                                        var selectedValue = doubleDropDownModel.Values.SingleOrDefault(data => data.Name == value);
                                        if (selectedValue != null)
                                        {
                                            selectedValue.IsSelected = true;
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case "EdFiGridWatchListDoubleSelectionTextboxModel":
                        var dropDownTextBoxModel = viewModel as EdFiGridWatchListDoubleSelectionTextboxModel;

                        if (dropDownTextBoxModel != null)
                        {
                            if (selectedOptions.Any(data => data.Name == dropDownTextBoxModel.Name))
                            {
                                var selectionOption = selectedOptions.SingleOrDefault(data => data.Name == dropDownTextBoxModel.Name);

                                if (selectionOption != null)
                                {
                                    foreach (var value in selectionOption.Values)
                                    {
                                        var comparison = dropDownTextBoxModel.Comparisons.SingleOrDefault(data => data.Name == value);
                                        if (comparison != null)
                                        {
                                            comparison.IsSelected = true;
                                            continue;
                                        }

                                        dropDownTextBoxModel.SelectionValue = value;
                                    }
                                }
                            }
                        }
                        break;
                }
            }

            return model;
        }

        #endregion

        #region Student Metrics Provider Calls

        /// <summary>
        /// Gets the student list. This is broken out into its own method so
        /// other projects can change how this data is retrieved.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <param name="sortColumn">The sort column.</param>
        /// <param name="sortDirection">The sort direction.</param>
        /// <returns></returns>
        protected virtual IQueryable<EnhancedStudentInformation> GetStudentList(
            StudentMetricsProviderQueryOptions queryOptions, MetadataColumn sortColumn = null, string sortDirection = "")
        {
            return StudentMetricsProvider.GetOrderedStudentList(queryOptions, sortColumn, sortDirection);
        }

        /// <summary>
        /// Gets the student metrics. This is broken out into its own method so
        /// other projects can change how this data is retrieved.
        /// </summary>
        /// <param name="queryOptions">The query options.</param>
        /// <returns></returns>
        protected virtual IQueryable<StudentMetric> GetStudentMetrics(StudentMetricsProviderQueryOptions queryOptions)
        {
            return StudentMetricsProvider.GetStudentsWithMetrics(queryOptions);
        }

        #endregion
    }
}
