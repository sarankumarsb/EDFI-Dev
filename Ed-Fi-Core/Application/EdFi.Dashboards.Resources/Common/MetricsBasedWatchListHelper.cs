using EdFi.Dashboards.Resources.Models.CustomGrid;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Common
{
    public static class MetricsBasedWatchListHelper
    {
        /// <summary>
        /// Gets the view models from the templates.
        /// </summary>
        /// <param name="templates">The templates.</param>
        /// <returns></returns>
        public static List<object> GetViewModelsFromTemplates(this IEnumerable<EdFiGridWatchListTemplateModel> templates)
        {
            var viewModels = new List<object>();

            foreach (var template in templates)
            {
                var childTemplates = template.ViewModel as IEnumerable<EdFiGridWatchListTemplateModel>;
                if (childTemplates != null)
                {
                    viewModels.AddRange(GetViewModelsFromTemplates(childTemplates));
                }
                else
                {
                    switch (template.TemplateName)
                    {
                        case "metricRadioButtonTemplate":
                        case "metricCheckboxTemplate":
                        case "metricCheckboxInlineTemplate":
                            var singleSelectionModel = template.ViewModel as EdFiGridWatchListSingleSelectionModel;

                            if (singleSelectionModel != null)
                            {
                                viewModels.Add(singleSelectionModel);
                            }
                            break;
                        case "metricDropDownTemplate":
                            var dropDownSelectionModel = template.ViewModel as EdFiGridWatchListDoubleSelectionModel;

                            if (dropDownSelectionModel != null)
                            {
                                viewModels.Add(dropDownSelectionModel);
                            }
                            break;
                        case "metricDropDownTextboxTemplate":
                            var dropDownTextBoxSelectionModel = template.ViewModel as EdFiGridWatchListDoubleSelectionTextboxModel;

                            if (dropDownTextBoxSelectionModel != null)
                            {
                                viewModels.Add(dropDownTextBoxSelectionModel);
                            }
                            break;
                    }
                }
            }

            return viewModels;
        }
    }
}
