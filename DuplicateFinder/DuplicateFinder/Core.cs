using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DuplicateFinder
{
    /// <summary>
    /// A class responsible for core functions
    /// </summary>
    public class Core
    {
        /// <summary>
        /// Extension for any file
        /// </summary>
        public const string FileExtensionAny = "*.*";

        /// <summary>
        /// Find all files in given directories
        /// </summary>
        /// <param name="directories">Target directories</param>
        /// <param name="extensions">Target file extensions. Searches for all extensions if this is null or empty. Default is null.</param>
        /// <param name="recursive">If true, searches recursively. Default is true.</param>
        /// <returns>FileInfo IEnumerable</returns>
        public static IEnumerable<FileInfo> Find(IEnumerable<DirectoryInfo> directories, IEnumerable<string> extensions = null, bool recursive = true)
        {
            if(directories.IsNullOrEmpty())
            {
                throw new ArgumentException("Directories cannot be null or empty.");
            }

            if(extensions.IsNullOrEmpty())
            {
                extensions = new List<string> { FileExtensionAny };
            }

            IEnumerable<FileInfo> fileInfos = null;
            SearchOption searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            foreach (DirectoryInfo directory in directories)
            {
                fileInfos = fileInfos.IsNullOrEmpty() ? 
                    FindHelper(directory, extensions, recursive) : 
                    fileInfos.Union(FindHelper(directory, extensions, recursive));
            }

            return fileInfos;
        }

        /// <summary>
        /// Helper method for the public method <see cref="Find(IEnumerable{string}, IEnumerable{string}, bool)"/>.
        /// </summary>
        /// <param name="directory">Target directory</param>
        /// <param name="extensions">Target extensions</param>
        /// <param name="recursive">If true, searches the directory recursively</param>
        /// <returns>FileInfo IEnumerable</returns>
        private static IEnumerable<FileInfo> FindHelper(DirectoryInfo directory, IEnumerable<string> extensions, bool recursive)
        {
            List<FileInfo> fileInfos = new List<FileInfo>();
            SearchOption searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

            foreach(string extension in extensions)
            {
                foreach (string filePath in Directory.EnumerateFiles(directory.ToString(), extension, searchOption))
                {
                    fileInfos.Add(new FileInfo(filePath));
                }
            }

            return fileInfos;
        }
    }
}
