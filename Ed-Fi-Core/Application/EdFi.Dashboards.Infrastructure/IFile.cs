// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EdFi.Dashboards.Infrastructure
{
	public interface IFile
	{
		bool Exists(string path);

		string[] ReadAllLines(string path);
	}
}
