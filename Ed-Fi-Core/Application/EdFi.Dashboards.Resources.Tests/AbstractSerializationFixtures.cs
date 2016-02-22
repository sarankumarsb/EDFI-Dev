// *************************************************************************
// Copyright (C) 2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Resources.Tests.Serialization;
using EdFi.Dashboards.Resources.Tests.Serialization.Builders;
using EdFi.Dashboards.Testing;
using NUnit.Framework;
using ServiceStack.Text;

namespace EdFi.Dashboards.Resources.Tests
{
    [TestFixture]
    public abstract class When_serializing_all_models_in_assembly<TMarker, TSerializer, TSerializationAttribute> : TestFixtureBase
        where TSerializer : ISerializer, new()
        where TSerializationAttribute : Attribute
    {
        private List<Type> modelTypes;
        private TestObjectFactory testObjectFactory;

        protected override void ExecuteTest()
        {
            // Find all models in the assembly
            modelTypes = (from t in typeof(TMarker).Assembly.GetTypes()
                          where !t.IsAbstract && t.IsClass && t.GetCustomAttributes(typeof(TSerializationAttribute), false).Length > 0
                          orderby t.FullName
                          select t)
                            .ToList();

            var concreteFixtureAssembly = Assembly.GetAssembly(this.GetType());
            var abstractFixtureAssembly = Assembly.GetExecutingAssembly();

            var builderTypes =
                from t in concreteFixtureAssembly.GetTypes() // Concrete test fixture assembly
                    .Concat(abstractFixtureAssembly.GetTypes()) // Abstract test fixture assembly
                where t.IsClass && !t.IsAbstract && typeof(IValueBuilder).IsAssignableFrom(t)
                    && t != typeof(CustomObjectBuilder)
                select t;

            // Create a list of builders, with the custom object builder at the very end of the list
            var builders =
                (from bt in builderTypes
                select (IValueBuilder) Activator.CreateInstance(bt))
                .Concat(new [] { new CustomObjectBuilder() });

            testObjectFactory = new TestObjectFactory(builders.ToArray(), modelTypes);
            StringValueBuilder.GenerateEmptyStrings = false;
            //StringValueBuilder.GenerateNulls = false;
        }

        [Test]
        public void Should_exactly_serialize_and_deserialize_all_serializable_models()
        {
            var sb = new StringBuilder();
            Console.WriteLine("Testing serialization for the following model types:");
            Console.WriteLine("----------------------------------------------------");

            var serializer = new TSerializer();
            Stopwatch sw = new Stopwatch();
            int dataLength = 0;

            string activity = "processing";

            foreach (var modelType in modelTypes)
            {
                string originalAsText = null;
                string deserializedText = null;

                try
                {
                    activity = "creating";
                    Console.WriteLine("\t" + modelType.FullName);

                    object model = testObjectFactory.Create(string.Empty, modelType);
                    Type modelRuntimeType = model.GetType();

                    sw.Start();

                    object deserializedModel = null;
                    
                    //for (int i = 0; i < 500; i++)
                    //{
                        activity = "serializing";
                        byte[] data = serializer.Serialize(model);
                        dataLength += data.Length;

                        activity = "deserializing";
                        deserializedModel = serializer.Deserialize(data, modelRuntimeType);
                    //}

                    sw.Stop();

                    activity = "dumping original model for";
                    originalAsText = AlphabetizeMembersForComparison(model.Dump());

                    activity = "dumping deserialized model for";
                    deserializedText = AlphabetizeMembersForComparison(deserializedModel.Dump());

                    activity = "comparing models for";
                    if (originalAsText != deserializedText)
                    {
                        //Console.WriteLine("\r\n======================================================");
                        Assert.That(deserializedText, Is.EqualTo(originalAsText), "Differences detected in serialization");
                        //Console.WriteLine("\r\n-----------------------------------------------");
                        //Console.WriteLine("\r\nOriginal Model:");
                        //Console.WriteLine(originalAsText);
                        //Console.WriteLine("\r\n-----------------------------------------------");
                        //Console.WriteLine("\r\nDeserialized Model:");
                        //Console.WriteLine(deserializedText);
                        //sb.AppendLine("\r\n======================================================");
                    }
                }
                catch (Exception ex)
                {
                    sb.AppendLine("\n===================================================================================");
                    sb.AppendFormat("Exception occurred while {0} type '{1}':\r\n", activity, modelType.FullName);
                    sb.AppendLine("-----------------------------------------------------------------------------------");
                    sb.AppendLine(ex.ToString());
                    
                    if (originalAsText != null)
                    {
                        sb.AppendLine("-----------------------------------------------------------------------------------");
                        sb.AppendLine("Original Model:");
                        sb.AppendLine(originalAsText);
                        sb.AppendLine("-----------------------------------------------------------------------------------");
                        sb.AppendLine("Deserialized Model:");
                        sb.AppendLine(deserializedText);
                    }

                    sb.AppendLine("===================================================================================");
                }
            }

            Console.WriteLine("Serialization/Deserialization Time (ms):    " + sw.ElapsedMilliseconds.ToString("n0"));
            Console.WriteLine("Serialization/Deserialization Size (bytes): " + dataLength.ToString("n0"));

            if (sb.Length > 0)
                throw new Exception(sb.ToString());
        }

        private string AlphabetizeMembersForComparison(string input)
        {
            string[] lines = input.Split('\n').Select(l => l.TrimEnd('\r')).ToArray();

            int i = 0;
            int objectBegin = -1;
            int objectEnd = -1;

            while (i < lines.Length)
            {
                string trimmedLine = lines[i].Trim();

                if (trimmedLine == "{")
                {
                    objectBegin = i + 1;
                }
                else if (trimmedLine == "}" || trimmedLine == "},")
                {
                    objectEnd = i;

                    if (objectBegin >= 0)
                        SortLines(ref lines, objectBegin, objectEnd);
                }
                else if (!Regex.IsMatch(trimmedLine, @"[\w]+:\s+[\w#]+,?"))
                {
                    objectBegin = -1;
                }

                i++;
            }

            return string.Join("\r\n", lines);
        }

        private void SortLines(ref string[] lines, int start, int end)
        {
            var sortedLines = new List<string>();

            for (int i = start; i < end; i++)
                sortedLines.Add(lines[i].TrimEnd(','));

            sortedLines.Sort();
            
            for (int i = 0; i < sortedLines.Count; i++)
            {
                lines[start + i] = sortedLines[i] + 
                    ((i == sortedLines.Count - 1) ? string.Empty : ",");
            }
        }
    }
}
