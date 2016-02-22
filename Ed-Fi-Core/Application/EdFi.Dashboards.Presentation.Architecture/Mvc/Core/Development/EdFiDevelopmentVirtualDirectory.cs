using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;

#if DEBUG

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.Core.Development
{
    /// <summary>
    /// Retrieves information about a physical directory's contents, and combines it with another provided 
    /// <see cref="VirtualDirectory"/> to present to the consumer a combined logical view of the available files and directories.
    /// </summary>
    public class EdFiDevelopmentVirtualDirectory : VirtualDirectory
    {
        private readonly string physicalPath;
        private readonly VirtualDirectory virtualDirectory;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdFiDevelopmentVirtualDirectory"/> class.
        /// </summary>
        /// <param name="virtualPath">The virtual path for the directory.</param>
        /// <param name="physicalPath">The resolved physical path for the directory.</param>
        /// <param name="virtualDirectory">The <see cref="VirtualDirectory"/> instance provided by the originally configured <see cref="VirtualPathProvider"/>.
        /// This is used to combine results from two sources to provide a single logical view of all the available files.</param>
        public EdFiDevelopmentVirtualDirectory(string virtualPath, string physicalPath, VirtualDirectory virtualDirectory) 
            : base(virtualPath)
        {
            this.physicalPath = physicalPath;
            this.virtualDirectory = virtualDirectory;
        }

        /// <summary>
        /// Gets all the subdirectories in the directory (as an enumerable of <see cref="VirtualDirectory"/> instances).
        /// </summary>
        public override IEnumerable Directories
        {
            get
            {
                IEnumerable<EdFiDevelopmentVirtualDirectory> coreDirectories;

                // Make sure physical directory exists before trying to get additional directories
                if (Directory.Exists(physicalPath))
                {
                    // Get the directories under this directory's physical path
                    coreDirectories =
                        from d in Directory.GetDirectories(physicalPath)
                        select new EdFiDevelopmentVirtualDirectory("~", d, null); // Virtual path not critical, but must be non-null
                }
                else
                {
                    coreDirectories = new EdFiDevelopmentVirtualDirectory[0];
                }

                // Combine the current directory contents (in "Core") with the contents of the virtual directory provided 
                // by the original VirtualPathProvider.  This presents to the application a combined logical view of both locations.
                var result = virtualDirectory.Directories
                    .Cast<VirtualDirectory>()
                    .Concat(coreDirectories);

                return result;
            }
        }

        /// <summary>
        /// Gets all the files in the directory (as an enumerable of <see cref="VirtualFile"/> instances).
        /// </summary>
        public override IEnumerable Files
        {
            get
            {
                IEnumerable<EdFiDevelopmentVirtualFile> coreDirectoryFiles;

                // Make sure physical directory exists before trying to get the files
                if (Directory.Exists(physicalPath))
                {
                    // Get the files available under this directory's physical path
                    coreDirectoryFiles = from f in Directory.GetFiles(physicalPath)
                                         select new EdFiDevelopmentVirtualFile("~", f);
                }
                else
                {
                    coreDirectoryFiles = new EdFiDevelopmentVirtualFile[0];
                }

                // Combine the current directory contents (in "Core") with the contents of the virtual directory provided 
                // by the original VirtualPathProvider.  This presents to the application a combined logical view of both locations.
                var result = virtualDirectory.Files
                    .Cast<VirtualFile>()
                    .Concat(coreDirectoryFiles);

                return result;
            }
        }

        /// <summary>
        /// Gets all the <see cref="Files"/> and <see cref="Directories"/> under the current directory.
        /// </summary>
        public override IEnumerable Children
        {
            get 
            {
                // Spoon feed the two results to the lazy Microsoft programmers
                foreach (var d in Directories)
                    yield return d;

                foreach (var f in Files)
                    yield return f;
            }
        }
    }
}

#endif
