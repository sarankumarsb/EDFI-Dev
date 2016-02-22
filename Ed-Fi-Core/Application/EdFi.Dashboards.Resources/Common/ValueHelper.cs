using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Common
{
    public static class ValueHelper
    {
        /// <summary>
        /// Determines whether the value has a usable value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>A boolean indicating whether the value is usable.</returns>
        public static bool HasUsableValue(this int value)
        {
            return value > 0;
        }

        /// <summary>
        /// Determines whether the value has a usable value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>A boolean indicating whether the value is usable.</returns>
        public static bool HasUsableValue(this int? value)
        {
            return value.HasValue && value.Value > 0;
        }

        /// <summary>
        /// Determines whether the value has a usable value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>A boolean indicating whether the value is usable.</returns>
        public static bool HasUsableValue(this long value)
        {
            return value > 0;
        }

        /// <summary>
        /// Determines whether the value has a usable value.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>A boolean indicating whether the value is usable.</returns>
        public static bool HasUsableValue(this long? value)
        {
            return value.HasValue && value.Value > 0;
        }
    }
}
