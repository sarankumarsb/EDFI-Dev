using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Resources.Models.Common
{
    public class StudentListIdentity
    {
        /// <summary>
        /// Gets or sets the basis for the student list (a teacher's section, cohort, or custom list).
        /// </summary>
        public StudentListType StudentListType { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the list, interpreted based on the <see cref="StudentListType"/> property.
        /// </summary>
        public long Id { get; set; }
    }
}
