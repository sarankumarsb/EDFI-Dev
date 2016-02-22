using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Testing
{
    public class PairedResult<TActual, TExpected>
    {
        public PairedResult() { }

        public PairedResult(TActual actual, TExpected expected)
        {
            Actual = actual;
            Expected = expected;
        }

        /// <summary>
        /// Gets or sets the actual data for the pair.
        /// </summary>
        public TActual Actual { get; set; }

        /// <summary>
        /// Gets or sets the expected data for the pair.
        /// </summary>
        public TExpected Expected { get; set; }
    }
}
