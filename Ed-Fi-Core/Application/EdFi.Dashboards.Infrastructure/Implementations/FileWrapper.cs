// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************
using System.IO;
using EdFi.Dashboards.Common;

namespace EdFi.Dashboards.Infrastructure.Implementations
{
	/// <summary>
	/// Wraps the <see cref="File"/> class.
	/// </summary>
	public class FileWrapper : IFile
	{
		public bool Exists(string path)
		{
			return path.FileExists();
		}

		public string[] ReadAllLines(string path)
		{
			return File.ReadAllLines(path);
		}
	}
}
