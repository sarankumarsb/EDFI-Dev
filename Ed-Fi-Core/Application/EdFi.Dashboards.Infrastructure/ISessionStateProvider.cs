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
	/// Defines methods and properties around maintaining state about a user's session.
	/// </summary>
	public interface ISessionStateProvider
	{
		/// <summary>
		/// Gets the specified value from the user's session.
		/// </summary>
		/// <param name="name">The name of the value to be retrieved.</param>
		/// <returns>The value if it exists; otherwise null/</returns>
		object GetValue(string name);

		/// <summary>
		/// Sets the specified value for the user's session.
		/// </summary>
		/// <param name="name">The name of the value to be set.</param>
		/// <param name="value">The value to be stored in session state.</param>
		void SetValue(string name, object value);

		/// <summary>
		/// Gets or sets values in the uer
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		object this[string name] { get; set; }

	    void RemoveValue(string name);

	    void Clear();

	    int Count { get; }

        string SessionId { get; }
	}
}
