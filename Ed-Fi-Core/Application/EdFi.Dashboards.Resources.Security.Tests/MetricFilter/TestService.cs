// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using EdFi.Dashboards.Core;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services;
using EdFi.Dashboards.Resources.Security.Common;

namespace EdFi.Dashboards.Resources.Security.Tests.MetricFilter
{
    public interface ITestService<in TRequest>
        where TRequest : MetricInstanceSetRequestBase, new()
    {
        [ApplyMetricSecurity]
        MetricTree Get(TRequest request);

        MetricTree GetMetrics(int metricVariantId, object hello);
    }

    public class TestService<TRequest> : ITestService<TRequest>
        where TRequest : MetricInstanceSetRequestBase, new()
    {
        public MetricTree GetMetrics(int metricVariantId, object hello)
        {
            if (metricVariantId == 1 || metricVariantId == (int)SchoolMetricEnum.ExperienceEducationCertifications)
                return BuildMetrics(metricVariantId, (int)SchoolMetricEnum.OperationsDashboard);

            return BuildMetrics(metricVariantId, (int)LocalEducationAgencyMetricEnum.OperationsDashboard);
        }

        protected MetricTree BuildMetrics(int metricVariantId, int dashboardMetricVariantId)
        {
            var result = new MetricTree(
                new ContainerMetric
                            {
                                MetricId = metricVariantId + 10000,
                                MetricVariantId = metricVariantId,
                                Children = new List<MetricBase>
                                                    {
                                                        new ContainerMetric
                                                            {
                                                                MetricId = 9997,
                                                                MetricVariantId = 999700
                                                            },
                                                        new ContainerMetric
                                                            {
                                                                MetricVariantId = dashboardMetricVariantId,
                                                                MetricId = dashboardMetricVariantId + 10000,
                                                                Children = new List<MetricBase>
                                                                                    {
                                                                                        new ContainerMetric
                                                                                            {
                                                                                                MetricId = 9999,
                                                                                                MetricVariantId = 999900
                                                                                            },
                                                                                        new ContainerMetric
                                                                                            {
                                                                                                MetricId = 9998,
                                                                                                MetricVariantId = 999800
                                                                                            }
                                                                                    }
                                                            },
                                                        new ContainerMetric
                                                            {
                                                                MetricId = 9996,
                                                                MetricVariantId = 999600
                                                            }
                                                    },
                                Actions = new List<MetricAction>
                                                {
                                                    //One to pass
                                                    new MetricAction{ MetricVariantId=1, Title="Historical", Tooltip="a", Url="~/aaa.aspx", ActionType=MetricActionType.DynamicContent },
                                                    //One to be filtered
                                                    new MetricAction{ MetricVariantId=1, Title="Detail", Tooltip="b", Url="~/bbb.aspx", ActionType=MetricActionType.DynamicContent },
                                                }

                            });

            return result;
        }

        public MetricTree Get(TRequest request)
        {
            return GetMetrics(request.MetricVariantId, new object());
        }
    }
}
