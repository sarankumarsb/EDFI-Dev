// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using EdFi.Dashboards.Metric.Data.Entities;
using EdFi.Dashboards.Metric.Resources.Models;
using MetricState = EdFi.Dashboards.Metric.Resources.Models.MetricState;
using MetricStateType = EdFi.Dashboards.Metric.Resources.Models.MetricStateType;

namespace EdFi.Dashboards.Metric.Resources.Services
{
    public interface IMetricStateProvider
    {
        State GetState(int metricId, string value, string valueType);
        State GetState(int metricId, int? stateTypeId);
        State GetState(MetricMetadataNode metricNode, MetricInstance metricInstance);
    }

    public class MetricStateProvider : IMetricStateProvider
    {
        private readonly IMetricMetadataTreeService metricMetadataTreeService;

        public MetricStateProvider(IMetricMetadataTreeService metricMetadataTreeService)
        {
            this.metricMetadataTreeService = metricMetadataTreeService;
        }

        #region MetricMetadataTree Property

        /// <summary>
        /// Holds the value for the <see cref="MetricMetadataTree"/> property.
        /// </summary>
        private MetricMetadataTree _metricMetadataTree;

        /// <summary>
        /// Gets the metric metadata tree.
        /// </summary>
        public MetricMetadataTree MetricMetadataTree
        {
            get
            {
                if (_metricMetadataTree == null)
                    _metricMetadataTree = metricMetadataTreeService.Get(MetricMetadataTreeRequest.Create());

                return _metricMetadataTree;
            }
        }

        #endregion

        public State GetState(int metricId, string value, string valueType)
        {
            // Make sure metadata is initialized
            var node = MetricMetadataTree.AllNodesByMetricId.ValuesByKey(metricId).FirstOrDefault();

            if (node == null)
                throw new InvalidOperationException(String.Format("No metric metadata was found for metric Id:{0}", metricId));

            return CalculateState(node, value, valueType);
        }

        public State GetState(int metricId, int? stateTypeId)
        {
            // Make sure metadata is initialized
            var node = MetricMetadataTree.AllNodesByMetricId.ValuesByKey(metricId).FirstOrDefault();
            
            if (node == null)
                throw new InvalidOperationException(String.Format("No metric metadata was found for metric Id: {0}", metricId));

            if (!stateTypeId.HasValue)
                return new State(MetricStateType.None, String.Empty);

            var metricStateType = (MetricStateType) stateTypeId.Value;
            var state = node.States.FirstOrDefault(x => x.StateType == metricStateType);

            if (state == null)
                return new State(MetricStateType.None, String.Empty);

            return new State(state.StateType, state.StateText);
        }

        public State GetState(MetricMetadataNode metricNode, MetricInstance metricInstance)
        {
            var defaultState = new State(MetricStateType.None, string.Empty);

            //If the instance is null then we cant calculate a state so we default to none.
            if (metricInstance == null)
                return defaultState;

            //Lets see if the State has been supplied to us.
            if (metricInstance.MetricStateTypeId.HasValue)
            {
                var suppliedState = (MetricStateType) metricInstance.MetricStateTypeId.Value;

                //We can have more than one state rule per state type so lets see what weve got.
                var metricStateRules = metricNode.States.Where(x => x.StateType == suppliedState);

                //We only need one rule to proceed.
                Models.MetricState singleRuleThatApplies = null;

                if (metricStateRules.Count() == 1)
                    singleRuleThatApplies = metricStateRules.Single();

                if (metricStateRules.Count() > 1)
                    singleRuleThatApplies = GetStateThatAppliesFromMultipleStates(metricNode, metricInstance, metricStateRules);

                //If we didnt find any then lets check if our metric is enabled.
                if (!metricStateRules.Any() || singleRuleThatApplies == null)
                {
                    //If there is no state then we should see if the metric is enabled, if so this is a problem in the metadata...
                    if (metricNode.Enabled)
                        throw new InvalidOperationException("Metric Id:" + metricNode.MetricId + " needs a Metric State value in its table to be able to calculate a State.");

                    //If we are still here it means that we have a disabled metric and we should not blow up.
                    return defaultState;
                }

                return new State(suppliedState, singleRuleThatApplies.StateText);
            }

            //If we are still here then we don't have a supplied state and now we need to go and calculate the state.
            return CalculateState(metricNode, metricInstance.Value, metricInstance.ValueTypeName);
        }

        private State CalculateState(MetricMetadataNode node, string value, string valueType)
        {
            if (string.IsNullOrEmpty(value))
                return new State(MetricStateType.None, String.Empty);

            if (string.IsNullOrEmpty(valueType))
                throw new ArgumentNullException("valueType", string.Format("Can't calculate State for metric Id({0}) because value and value type are null", node.MetricId));

            //Activate the value.
            var t = GetType(valueType);
            object valueInstance = Convert.ChangeType(value, t);
            var compValue = (IComparable)valueInstance;

            foreach (var stateRule in node.States)
            {
                bool? metMinValueConditionResult = null;
                bool? metMaxValueConditionResult = null;

                //Test to see what condition applies.
                Func<bool> metMinValueCondition = () =>
                {
                    if (metMinValueConditionResult != null)
                        return metMinValueConditionResult.Value;

                    metMinValueConditionResult = IsMinimumConditionMet(stateRule, compValue, t);

                    return metMinValueConditionResult.Value;
                };

                Func<bool> metMaxValueCondition = () =>
                {
                    if (metMaxValueConditionResult != null)
                        return metMaxValueConditionResult.Value;

                    metMaxValueConditionResult = IsMaximumConditionMet(stateRule, compValue, t);

                    return metMaxValueConditionResult.Value;
                }; 

                if (stateRule.MaxValue == null)
                    if (metMinValueCondition())
                        metMaxValueConditionResult = true;

                if (stateRule.MinValue == null && metMaxValueCondition())
                    metMinValueConditionResult = true;

                if (metMinValueCondition() && metMaxValueCondition())
                    return new State(stateRule.StateType, stateRule.StateText);
            }

            //After going through the foreach if there is nothing then there is no threshold rule defined.
            //throw new NullReferenceException("There is no threshold rule that applies for this metricId=" + studentMetric.MetricId);
            return new State(MetricStateType.None, String.Empty);
        }

        private Dictionary<string, Type> typesByName = new Dictionary<string, Type>(); 

        private Type GetType(string valueType)
        {
            Type t;

            if (!typesByName.TryGetValue(valueType, out t))
            {
                t = Type.GetType(valueType);
                typesByName[valueType] = t;
            }

            return t;
        }

        private Models.MetricState GetStateThatAppliesFromMultipleStates(MetricMetadataNode metricNode, MetricInstance metricInstance, IEnumerable<Models.MetricState> metricStateGoodRules)
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
            Type t = GetType(metricInstance.ValueTypeName);
            object value = Convert.ChangeType(metricInstance.Value, t);
            IComparable compValue = (IComparable)value;

            foreach (var stateRule in metricStateGoodRules)
            {
                bool? metMinValueConditionResult = null;
                bool? metMaxValueConditionResult = null;

                Func<bool> metMinValueCondition = () =>
                {
                    if (metMinValueConditionResult != null)
                        return metMinValueConditionResult.Value;

                    metMinValueConditionResult = IsMinimumConditionMet(stateRule, compValue, t);

                    return metMinValueConditionResult.Value;
                };

                Func<bool> metMaxValueCondition = () =>
                {
                    if (metMaxValueConditionResult != null)
                        return metMaxValueConditionResult.Value;

                    metMaxValueConditionResult = IsMaximumConditionMet(stateRule, compValue, t);

                    return metMaxValueConditionResult.Value;
                }; 

                if (stateRule.MaxValue == null)
                    if (metMinValueCondition())
                        metMaxValueConditionResult = true;

                if (stateRule.MinValue == null && metMaxValueCondition())
                    metMinValueConditionResult = true;

                if (metMinValueCondition() && metMaxValueCondition())
                    return stateRule;
            }

            return null;
        }

        private static bool IsMinimumConditionMet(MetricState stateRule, IComparable compValue, Type t)
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

        private static bool IsMaximumConditionMet(MetricState stateRule, IComparable compValue, Type t)
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
