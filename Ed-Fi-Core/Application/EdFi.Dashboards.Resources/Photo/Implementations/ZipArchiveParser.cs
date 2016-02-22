// *************************************************************************
// ©2013 Ed-Fi Alliance, LLC. All Rights Reserved.
// *************************************************************************

using System.Collections.Generic;
using System.IO;
using System.Linq;
using EdFi.Dashboards.Common;
using ICSharpCode.SharpZipLib.Zip;

namespace EdFi.Dashboards.Resources.Photo.Implementations
{
    public class ZipArchiveParser : ChainOfResponsibilityBase<IArchiveParser, byte[], Dictionary<string, byte[]>>, IArchiveParser
    {
        public ZipArchiveParser(IArchiveParser next)
            : base(next)
        {

        }

        public Dictionary<string, byte[]> Parse(byte[] archive)
        {
            return ProcessRequest(archive);
        }

        protected override bool CanHandleRequest(byte[] request)
        {
            try
            {
                var files = HandleRequest(request);

                return files.Any();
            }
            catch
            {
                return false;
            }
        }

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", 
			Justification= @"Nested usings, such as the one below, will cause the the Dispose method of the MemoryStream to be called when the 
							 Dispose method of the ZipInputStream is called when the code exits out of the using block, however IDisposable ensures that this behavior is safe.

							 http://msdn.microsoft.com/en-us/library/system.idisposable.dispose.aspx
							 If an object's Dispose method is called more than once, the object must ignore all calls after the first one. 
							 The object must not throw an exception if its Dispose method is called multiple times.

							 When you use an object that accesses unmanaged resources, such as a StreamWriter, a good practice is to create the instance with a using statement. 
							 The using statement automatically closes the stream and calls Dispose on the object when the code that is using it has completed.")]
		protected override Dictionary<string, byte[]> HandleRequest(byte[] request)
        {
            var unzippedPackage = new Dictionary<string, byte[]>();

            using (var memoryStream = new MemoryStream(request))
            {
                using (var zipInputStream = new ZipInputStream(memoryStream))
                {
                    for (var zipEntry = zipInputStream.GetNextEntry(); zipEntry != null; zipEntry = zipInputStream.GetNextEntry())
                    {
                        if (string.IsNullOrEmpty(zipEntry.Name)) continue;

                        using (var fileEntry = new MemoryStream())
                        {
                            var data = new byte[2048];

                            for (var size = zipInputStream.Read(data, 0, data.Length); size > 0; size = zipInputStream.Read(data, 0, data.Length))
                            {
                                fileEntry.Write(data, 0, size);
                            }

                            var zipEntryContents = fileEntry.ToArray();

                            unzippedPackage.Add(zipEntry.Name, zipEntryContents);
                        }
                    }
                }
            }

            return unzippedPackage;
        }
    }
}
