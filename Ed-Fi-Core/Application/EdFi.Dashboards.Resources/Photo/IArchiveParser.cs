// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System;
using System.Collections.Generic;

namespace EdFi.Dashboards.Resources.Photo
{
    public interface IArchiveParser
    {
        /// <summary>
        /// Convert the zip file into a flat collection of files.
        /// </summary>
        /// <param name="archive">The file bytes that represent the archive file (e.g. zip, 7z, etc).</param>
        /// <returns>The dictionary that represents the flat collection of files.</returns>
        Dictionary<string, byte[]> Parse(byte[] archive);
    }

    public class NullArchiveParser : IArchiveParser
    {
        public Dictionary<string, byte[]> Parse(byte[] archive)
        {
			throw new NotImplementedException(string.Format("Unhandled request. Could not open the archive."));
        }
    }
}