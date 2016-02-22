// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Common;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using EdFi.Dashboards.Metric.Resources.Services.Data;
using MetricStateType = EdFi.Dashboards.Metric.Resources.Models.MetricStateType;

namespace EdFi.Dashboards.Metric.Resources.Services
{
    public interface IMetricGoalProvider
    {
        Goal GetMetricGoal(Guid metricInstanceSetKey, int metricId);
        Goal GetMetricGoal(MetricMetadataNode metricMetadataNode, MetricData metricData);
    }

    public class MetricGoalProvider : IMetricGoalProvider
    {
        private readonly IMetricMetadataTreeService metricMetadataNodeService;
        private readonly IMetricGoalsService metricGoalsService;
        private readonly IMetricInstancesService metricInstancesService;

        public MetricGoalProvider(IMetricMetadataTreeService metricMetadataNodeService, IMetricGoalsService metricGoalsService, 
            IMetricInstancesService metricInstancesService)
        {
            this.metricMetadataNodeService = metricMetadataNodeService;
            this.metricGoalsService = metricGoalsService;
            this.metricInstancesService = metricInstancesService;
        }

        public Goal GetMetricGoal(Guid metricInstanceSetKey, int metricId)
        {
            var metricNode = MetricMetadataTree.AllNodesByMetricId.ValuesByKey(metricId).FirstOrDefault();

            if (metricNode == null)
				throw new InvalidOperationException(string.Format("MetricId:{0} was not found.", metricId));

            //Getting required data...
            var metricDataRequest = MetricDataRequest.Create(metricInstanceSetKey);
            var goalData = metricGoalsService.Get(metricDataRequest).SingleOrDefault(x => x.MetricId == metricId);
            var metricInstance = metricInstancesService.Get(metricDataRequest).SingleOrDefault(x => x.MetricId == metricId);

            return CalculateGoal(metricNode, goalData, metricInstance);
        }

        private MetricMetadataTree metricMetadataTree;

        private MetricMetadataTree MetricMetadataTree
        {
            get { return metricMetadataTree ?? (metricMetadataTree = metricMetadataNodeService.Get(MetricMetadataTreeRequest.Create())); }
        }

        public Goal GetMetricGoal(MetricMetadataNode metricMetadataNode, MetricData metricData)
        {
            if (metricData.MetricGoalsByMetricId == null)
                return null;

            var goalData = metricData.MetricGoalsByMetricId.GetValueOrDefault(metricMetadataNode.MetricId);
            var metricInstance = metricData.MetricInstancesByMetricId.GetValueOrDefault(metricMetadataNode.MetricId);

            return CalculateGoal(metricMetadataNode, goalData, metricInstance);
        }

        private static Goal CalculateGoal(MetricMetadataNode metricNode, MetricGoal goalData, MetricInstance metricInstance)
        {
            //If we have a disabled metric we should not blow up and return a default Goal.
            if(!metricNode.Enabled)
                return new Goal
                    {
                        Interpretation = TrendInterpretation.None,
                        Value = null
                    };

            if (metricNode.TrendInterpretation == null)
                throw new InvalidOperationException(string.Format("Trend Interpretation is needed to be able to calculate Goal. Metric Id({0})", metricNode.MetricId));

            //Lets try to get goal from data.
            var goalFromData = GetGoalFromGoalData(metricNode, goalData);
            if (goalFromData != null)
                return goalFromData;

            //If we are still here lets get the Goal value from the MetricState.
            var goalFromMetricState = GetGoalFromMetricState(metricNode, goalData, metricInstance);
            if (goalFromMetricState != null)
                return goalFromMetricState;

            //If we are still here something went wrong.
            throw new InvalidOperationException("Something went wrong and I could not calculate a goal for metric Id:" + metricNode.MetricId);
        }

        private static Goal GetGoalFromGoalData(MetricMetadataNode metricNode, MetricGoal goalData)
        {
            var model = new Goal { Interpretation = (TrendInterpretation)metricNode.TrendInterpretation.GetValueOrDefault() };

            //Lets see if we have a goal.
            if (goalData != null)
            {
                model.Value = goalData.Value;
                return model;
            }

            return null;
        }

        private static Goal GetGoalFromMetricState(MetricMetadataNode metricNode, MetricGoal goalData, MetricInstance metricInstance)
        {

            var model = new Goal { Interpretation = (TrendInterpretation)metricNode.TrendInterpretation.GetValueOrDefault() };

            //We can have more than one good state so lets see what we've got.
            var metricStateGoodRules = metricNode.States.Where(x => x.StateType == MetricStateType.Good).ToList();

            //We only one rule to proceed.
            Models.MetricState singleRuleThatApplies = null;

            if (metricStateGoodRules.Count() == 1)
                singleRuleThatApplies = metricStateGoodRules.Single();

            if (metricStateGoodRules.Count() > 1)
            {
                //To get a metric state from a case like this we need an instance if no instance then we cant proceed.
                if (metricInstance == null)
                {
                    model.Value = null;
                    return model;
                }

                singleRuleThatApplies = GetStateThatAppliesFromMultipleStates(metricNode, metricInstance, metricStateGoodRules);
            }


            //If we didn't find any then lets check if our metric is enabled.
            if (!metricStateGoodRules.Any() || singleRuleThatApplies == null)
            {
                //If there's no state, see if the metric is enabled, if it's not enabled, ignore the problem.
                if (!metricNode.Enabled)
                {
                    //If we are here it means that we have a disabled metric and we should not blow up.
                    return new Goal
                    {
                        Interpretation = TrendInterpretation.None,
                        Value = null
                    };
                }

                //If the metric is configured not to use metric states, then it's fine for there not to be a goal.
                if (metricNode.States.Any(x => x.StateType == MetricStateType.Na || x.StateType == MetricStateType.None))
                {
                    return new Goal
                    {
                        Interpretation = TrendInterpretation.None,
                        Value = null
                    };
                }

                //Otherwise, this is a problem with the metadata.
                throw new InvalidOperationException("Metric Id:" + metricNode.MetricId + " needs a Metric State value in its table to be able to calculate a goal.");
            }

            if (singleRuleThatApplies.MinValue != null && singleRuleThatApplies.MinValue.Value != 0)
            {
                model.Value = singleRuleThatApplies.MinValue.Value;
                return model;
            }

            if (singleRuleThatApplies.MaxValue != null)
            {
                model.Value = singleRuleThatApplies.MaxValue.Value;
                return model;
            }

            //This metric has a state rule but we cant calculate the value of the goal so its null.
            //This metric does not have goal.
            if (singleRuleThatApplies.MinValue == null && singleRuleThatApplies.MaxValue == null)
            {
                model.Value = null;
                return model;
            }

            return null;
        }

        private static Models.MetricState GetStateThatAppliesFromMultipleStates(MetricMetadataNode metricNode, MetricInstance metricInstance, IEnumerable<Models.MetricState> metricStateGoodRules)
        {
            //Logic to pick the right one when we have many.

            if (metricInstance == null)
				throw new ArgumentNullException("metricInstance", string.Format("Can not calculate a Goal for metric Id:({0}) A Metric Instance is needed in the DB.", metricNode.MetricId));

            //We need to have the value to proceed.
            if (string.IsNullOrEmpty(metricInstance.Value))
				throw new ArgumentOutOfRangeException("metricInstance", "The value has to be supplied in order to be able to calculate the state. metric Id:(" + metricNode.MetricId + ")");

            if (string.IsNullOrEmpty(metricInstance.ValueTypeName))  // In case of bad or missing metadata	
				throw new ArgumentOutOfRangeException("metricInstance", "The value type has to be supplied in order to be able to calculate the state. metric Id:(" + metricNode.MetricId + ")");

            if (!metricInstance.ValueTypeName.StartsWith("System."))
                return null; //new Goal { Interpretation = TrendInterpretation.None, Value = null};

            //Activate the value.
            Type t = Type.GetType(metricInstance.ValueTypeName);
            object value = Convert.ChangeType(metricInstance.Value, t);
            var compValue = (IComparable)value;

            foreach (var stateRule in metricStateGoodRules)
            {
                //Test to see what condition applies!
                bool metMinValueCondition = IsMinimumConditionMet(stateRule, compValue, t);
                bool metMaxValueCondition = IsMaximumConditionMet(stateRule, compValue, t);

                if (stateRule.MaxValue == null)
                    if (metMinValueCondition)
                        metMaxValueCondition = true;

                if (stateRule.MinValue == null && metMaxValueCondition)
                    metMinValueCondition = true;

                if (metMinValueCondition && metMaxValueCondition)
                    return stateRule;
            }

            return null;
        }

        private static bool IsMinimumConditionMet(Models.MetricState stateRule, IComparable compValue, Type t)
        {
            bool metMinValueCondition = false;

            if (stateRule.MinValue != null)
            {
                if (stateRule.IsMinValueInclusive != null)
                {
                    int comparedValue = compValue.CompareTo(Convert.ChangeType(stateRule.MinValue.Value, t));
                    //Is inclusive
                    if (stateRule.IsMinValueInclusive.Value != 0)
                    {
                        if (comparedValue >= 0)
                            metMinValueCondition = true;
                    }
                    else//Is not
                    {
                        if (comparedValue > 0)
                            metMinValueCondition = true;
                    }
                }
            }

            return metMinValueCondition;
        }

        private static bool IsMaximumConditionMet(Models.MetricState stateRule, IComparable compValue, Type t)
        {
            bool metMaxValueCondition = false;

            if (stateRule.MaxValue != null)
            {
                if (stateRule.IsMaxValueInclusive != null)
                {
                    int comparedValue = compValue.CompareTo(Convert.ChangeType(stateRule.MaxValue.Value, t));

                    if (stateRule.IsMaxValueInclusive.Value != 0)//Is inclusive
                    {
                        if (comparedValue <= 0)
                            metMaxValueCondition = true;
                    }
                    else//Is not
                    {
                        if (comparedValue < 0)
                            metMaxValueCondition = true;
                    }
                }
            }

            return metMaxValueCondition;
        }
    }
}
