// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Infrastructure
{
    /// <summary>
    /// Defines a "shim" method to enable exportable models to provide a generic interface to the 
    /// <see cref="ICsvSerializer"/> implementation.
    /// </summary>
    /// <remarks>This interface was added due to problems serializing an object for caching purposes (i.e. an export model) that directly implements <see cref="IEnumerable"/>.</remarks>
    public interface ICsvSerializable
    {
        /// <summary>
        /// Gets the data in a generic format that can be serialized by a <see cref="ICsvSerializer"/> implementation.
        /// </summary>
        /// <returns>The data to be serialized as an enumerable of key value pair list that describes the key as the "column name" and the value as the "value for the column".</returns>
        IEnumerable<IEnumerable<KeyValuePair<string, object>>> ToSerializableEnumerable();
    }

    /// <summary>
    /// Defines a method for serializing data to CSV format.
    /// </summary>
    public interface ICsvSerializer
    {
        /// <summary>
        /// Converts a list of key value pairs to CSV format.
        /// </summary>
        /// <param name="serializable">An object exposing its data in a format that can be serialized to CSV format.</param>
        /// <returns>A string containing the CSV format.</returns>
        string Serialize(ICsvSerializable serializable);
    }    
}
