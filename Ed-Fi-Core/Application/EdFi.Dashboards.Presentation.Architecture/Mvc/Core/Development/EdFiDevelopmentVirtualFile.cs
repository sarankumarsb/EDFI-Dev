using System.IO;
using System.Web.Hosting;

#if DEBUG

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.Core.Development
{
    /// <summary>
    /// Provides functionality to open a virtual file from a physical one.
    /// </summary>
    public class EdFiDevelopmentVirtualFile : VirtualFile
    {
        private readonly string corePhysicalPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="EdFiDevelopmentVirtualFile"/> class.
        /// </summary>
        /// <param name="virtualPath">The virtual path for the file.</param>
        /// <param name="corePhysicalPath">The resolved physical path for the file.</param>
        public EdFiDevelopmentVirtualFile(string virtualPath, string corePhysicalPath)
            : base(virtualPath)
        {
            this.corePhysicalPath = corePhysicalPath;
        }

        /// <summary>
        /// Gets the physical path to the file.
        /// </summary>
        public string PhysicalPath
        {
            get { return corePhysicalPath; }
        }

        /// <summary>
        /// Opens a stream containing the file contents.
        /// </summary>
        /// <returns></returns>
        public override Stream Open()
        {
            return new FileStream(PhysicalPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        }
    }
}

#endif