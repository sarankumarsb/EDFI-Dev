using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web.Caching;
using System.Web.Hosting;

#if DEBUG

namespace EdFi.Dashboards.Presentation.Architecture.Mvc.Core.Development
{
    /// <summary>
    /// Decorates the provided <see cref="VirtualPathProvider"/> to provide a combined logical view of the combined contents of
    /// both locations (Core and locations served by the original provider). 
    /// </summary>
    public class EdFiDevelopmentVirtualPathsProviderDecorator : VirtualPathProvider
    {
        private readonly VirtualPathProvider wrappedProvider;
        private string webBasePath;
        private string coreBasePath;

        private const string RelativePathToCoreArtifacts = @"Application\EdFi.Dashboards.Presentation.Core\";

        /// <summary>
        /// Initializes a new instance of the <see cref="EdFiDevelopmentVirtualPathsProviderDecorator"/> class.
        /// </summary>
        /// <param name="wrappedProvider">The original <see cref="VirtualPathProvider"/> implementation to use as the primary
        /// source for files and directories.  The Core project will be used as a fallback location, as is the 
        /// convention in the EdFi project for extensibility purposes.</param>
        public EdFiDevelopmentVirtualPathsProviderDecorator(VirtualPathProvider wrappedProvider)
        {
            this.wrappedProvider = wrappedProvider;

            InitializePaths();
        }

        private void InitializePaths()
        {
            // Physical path to main web application
            webBasePath = HostingEnvironment.MapPath("~");

            // Morph path to point to Core location for development web files
            string projectParentFolder = Path.Combine(webBasePath, @"..\..\..\");
            var directories = Directory.GetDirectories(projectParentFolder, "*-Core");

            if (directories.Length > 0)
                coreBasePath = Path.Combine(directories[0], RelativePathToCoreArtifacts);
        }

        private string GetPhysicalPath(string virtualPath)
        {
            string physicalPath = HostingEnvironment.MapPath(virtualPath);
            string corePhysicalPath = physicalPath.Replace(webBasePath, coreBasePath);
            return corePhysicalPath;
        }

        /// <summary>
        /// Indicates whether or not the specified virtual directory exists in either the wrapped VirtualPathProvider
        /// or the Core project folders.
        /// </summary>
        /// <param name="virtualDir">The virtual path to evaluate.</param>
        /// <returns><b>true</b> if the directory exists in either project; otherwise <b>false</b>.</returns>
        public override bool DirectoryExists(string virtualDir)
        {
            try
            {
                return wrappedProvider.DirectoryExists(virtualDir)
                                   || Directory.Exists(GetPhysicalPath(virtualDir));
            }
            catch (PathTooLongException)
            {
                // This is only OK because this is a development provider. When resolving the physical path,
                // the physical path was too long and the exception was being thrown. We'll just ignore the 
                // exception since this thrown on long URI's that are not physical anyway.
            }

            return false;
        }

        private static Dictionary<string, SupportedLocation> filesByVirtualPath = new Dictionary<string, SupportedLocation>();

        /// <summary>
        /// Indicates whether or not the specified virtual file exists in either the wrapped VirtualPathProvider
        /// or the Core project folders.
        /// </summary>
        /// <param name="virtualPath">The virtual path to provide.</param>
        /// <returns><b>true</b> if the file exists in either project; otherwise <b>false</b>.</returns>
        public override bool FileExists(string virtualPath)
        {
            SupportedLocation supportedLocation;

            if (filesByVirtualPath.TryGetValue(virtualPath, out supportedLocation))
                return (supportedLocation != SupportedLocation.None);

            if (wrappedProvider.FileExists(virtualPath))
            {
                filesByVirtualPath[virtualPath] = SupportedLocation.Web;
                return true;
            }

            try
            {
                var physicalPath = GetPhysicalPath(virtualPath);

                if (File.Exists(physicalPath))
                {
                    filesByVirtualPath[virtualPath] = SupportedLocation.Core;
                    return true;
                }
            }
            catch (PathTooLongException)
            {
                // This is only OK because this is a development provider. When resolving the physical path,
                // the physical path was too long and the exception was being thrown. We'll just ignore the 
                // exception since this thrown on long URI's that are not physical anyway.
            }

            filesByVirtualPath[virtualPath] = SupportedLocation.None;
            return false;
        }

        /// <summary>
        /// Gets a <see cref="CacheDependency"/> for the physical file represented by the provided virtual path, using the 
        /// wrapped <see cref="VirtualPathProvider"/>'s file (if it exists), otherwise using the Core projects' physical file.
        /// </summary>
        /// <param name="virtualPath">The virtual path to provide.</param>
        /// <param name="virtualPathDependencies">An enumerable of strings containing virtual paths for which to create 
        /// physical file <see cref="CacheDependency"/> instances.</param>
        /// <param name="utcStart">The <see cref="DateTime"/> to use when creating the <see cref="CacheDependency"/>.</param>
        /// <returns>The <see cref="CacheDependency"/>.</returns>
        public override CacheDependency GetCacheDependency(string virtualPath, IEnumerable virtualPathDependencies, DateTime utcStart)
        {
            if (virtualPathDependencies == null)
                return null;

            StringCollection physicalFilenames = null;
            
            foreach (string str in virtualPathDependencies)
            {
                string physicalFilename;

                if (physicalFilenames == null)
                    physicalFilenames = new StringCollection();

                var coreFile = this.GetFile(str) as EdFiDevelopmentVirtualFile;

                if (coreFile != null)
                    physicalFilename = coreFile.PhysicalPath;
                else
                    physicalFilename = HostingEnvironment.MapPath(str);

                physicalFilenames.Add(physicalFilename);
            }

            if (physicalFilenames == null)
                return null;

            string[] array = new string[physicalFilenames.Count];
            physicalFilenames.CopyTo(array, 0);
            return new CacheDependency(array, utcStart);
        }

        #region Passthroughs to wrapped provider (commented out)

        //public override string CombineVirtualPaths(string basePath, string relativePath)
        //{
        //    return wrappedProvider.CombineVirtualPaths(basePath, relativePath);
        //}

        //public override string GetCacheKey(string virtualPath)
        //{
        //    return wrappedProvider.GetCacheKey(virtualPath);
        //}

        //public override string GetFileHash(string virtualPath, IEnumerable virtualPathDependencies)
        //{
        //    return wrappedProvider.GetFileHash(virtualPath, virtualPathDependencies);
        //}

        //public override object InitializeLifetimeService()
        //{
        //    return wrappedProvider.InitializeLifetimeService();
        //}

        #endregion

        /// <summary>
        /// Gets the <see cref="EdFiDevelopmentVirtualDirectory"/> representing the combined view of the physical directories from the
        /// Core project, and the original <see cref="VirtualPathProvider"/> for the specified virtual path.
        /// </summary>
        /// <param name="virtualDir">The virtual path to evaluate.</param>
        /// <returns>The <see cref="EdFiDevelopmentVirtualDirectory"/> instance.</returns>
        public override VirtualDirectory GetDirectory(string virtualDir)
        {
            var directory = wrappedProvider.GetDirectory(virtualDir);

            return new EdFiDevelopmentVirtualDirectory(virtualDir, GetPhysicalPath(virtualDir), directory);
        }

        /// <summary>
        /// Get the <see cref="EdFiDevelopmentVirtualFile"/> representing the combined logical view of the files in the 
        /// physical directories based on the virtual path specified.
        /// </summary>
        /// <param name="virtualPath">The virtual path to evaluate.</param>
        /// <returns>The <see cref="EdFiDevelopmentVirtualFile"/> instance.</returns>
        public override VirtualFile GetFile(string virtualPath)
        {
            SupportedLocation supportedLocation;

            if (!filesByVirtualPath.TryGetValue(virtualPath, out supportedLocation))
            {
                if (!FileExists(virtualPath))
                    return null;

                supportedLocation = filesByVirtualPath[virtualPath];
            }

            // This should never happen
            if (supportedLocation == SupportedLocation.None)
                throw new FileNotFoundException(string.Format("Could not find file for virtual path '{0}', even though it seems to have been found before.", virtualPath), virtualPath);
            
            // Standard web project file
            if (supportedLocation == SupportedLocation.Web)
                return wrappedProvider.GetFile(virtualPath);

            // Core file
            return new EdFiDevelopmentVirtualFile(virtualPath, GetPhysicalPath(virtualPath));
        }
    }

    /// <summary>
    /// Indicates where a virtual path's physical file is to be served from.
    /// </summary>
    internal enum SupportedLocation
    {
        /// <summary>
        /// Indicates that a physical file was not found for the paired virtual path.
        /// </summary>
        None,
        /// <summary>
        /// Indicates that a physical file was found in the main web project.
        /// </summary>
        Web,
        /// <summary>
        /// Indicates that a physical file was found in the core project.
        /// </summary>
        Core,
    }
}

#endif