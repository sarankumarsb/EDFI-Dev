// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Security
{
	public interface IRoleAuthorization
	{
		/// <summary>
		/// Authorizes the current request for a given role.
		/// </summary>
		/// <param name="methodAttributes"></param>
		/// <param name="parameters"></param>
		void AuthorizeRequest(IEnumerable<Attribute> methodAttributes, IEnumerable<ParameterInstance> parameters);
	}
}
