// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using EdFi.Dashboards.Resource.Models.Common;

namespace EdFi.Dashboards.Resources.Models.LocalEducationAgency
{
	[Serializable]
	public class BriefModel : ResourceModelBase
	{
		public int LocalEducationAgencyId { get; set; }
		public string Name { get; set; }
		public string ProfileThumbnail { get; set; }
	}	
}
