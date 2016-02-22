using System;
using EdFi.Dashboards.Metric.Resources.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace EdFi.Dashboards.Presentation.Core.UITests.Support.RestSharp
{
    public abstract class JsonCreationConverter<T> : JsonConverter
    {
        protected abstract T Create(Type objectType, JObject jsonObject);

        public override bool CanConvert(Type objectType)
        {
            return typeof(T).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            var target = Create(objectType, jsonObject);
            serializer.Populate(jsonObject.CreateReader(), target);
            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public class JsonMetricBaseConverter : JsonCreationConverter<MetricBase>
    {
        protected override MetricBase Create(Type objectType, JObject jsonObject)
        {
            if (jsonObject["Children"] != null)
            {
                if (jsonObject["IsFlagged"] != null)
                {
                    return new AggregateMetric();
                }

                return new ContainerMetric();
            }

            return new GranularMetric<dynamic>();
        }
    }
}
