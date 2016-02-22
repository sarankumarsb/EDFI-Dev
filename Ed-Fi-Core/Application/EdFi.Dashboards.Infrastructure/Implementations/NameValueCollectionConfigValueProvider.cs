// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.Collections.Specialized;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
	public class NameValueCollectionConfigValueProvider : IConfigValueProvider
	{
		public string GetValue(string name)
		{
			return values[name];
		}

		/// <summary>
		/// Holds the value for the <see cref="Values"/> property.
		/// </summary>
		private NameValueCollection values = new NameValueCollection();

		/// <summary>
		/// Gets or sets the underlying <see cref="NameValueCollection"/> containing the configuration values.
		/// </summary>
		public NameValueCollection Values
		{
			get { return values; }
			set { values = value; }
		}

	}
}
