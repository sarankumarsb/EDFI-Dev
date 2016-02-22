// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Reflection;

namespace EdFi.Dashboards.Resources.Security
{
	public class ParameterInstance
	{
		public string Name { get; set;}
		public ParameterInfo ParameterInfo { get; set; }
		public object Value { get; set; }

        public override string ToString()
        {
            return (string.Format("{0}={1}", Name, Value));
        }
	}
}