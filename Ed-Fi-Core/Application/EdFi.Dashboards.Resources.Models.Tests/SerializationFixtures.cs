// *************************************************************************
// Copyright (C) 2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using EdFi.Dashboards.Infrastructure;
using EdFi.Dashboards.Infrastructure.Implementations;
using EdFi.Dashboards.Resources.Tests;
using NUnit.Framework;

namespace EdFi.Dashboards.Resources.Models.Tests
{
    /// <summary>
    /// Provides a base class for testing serialization of models in the model assembly under test.
    /// </summary>
    /// <typeparam name="TSerializer">The type of ISerializer to use for data serialization.</typeparam>
    /// <typeparam name="TSerializationAttribute">The attribute to be used as the basis for determining which model classes to test.</typeparam>
    public abstract class When_serializing_models_in_this_assembly<TSerializer, TSerializationAttribute>
        : When_serializing_all_models_in_assembly<Marker_EdFi_Dashboards_Resources_Models, TSerializer, TSerializationAttribute>
        where TSerializer : ISerializer, new()
        where TSerializationAttribute : Attribute { }

    [TestFixture]
    public class When_serializing_all_models_in_this_assembly_using_native_dotnet_binary_serializer
        : When_serializing_models_in_this_assembly
            <BinaryFormatterSerializer, SerializableAttribute> { }

    [TestFixture]
    public class When_serializing_all_models_in_this_assembly_using_ServiceStacks_type_serializer
        : When_serializing_models_in_this_assembly
            <ServiceStackTypeSerializer, SerializableAttribute> { }
}
