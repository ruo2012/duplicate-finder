using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

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

        public string RootDirectoryPath { get; set; }

        public int TotalFileCount { get; private set; } = 0;

        public int ProcessedFileCount { get; private set; } = 0;

        public IEnumerable<DuplicateGroup> DuplicateGroups { get; private set; }

        public string ProcessingFilePath { get; private set; }

        public event Action OnProgressChanged;

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

        /// <summary>
        /// Computes MD5 Hash
        /// </summary>
        /// <param name="filePath">Target file path</param>
        /// <returns>MD5 hash string</returns>
        private static string ComputeMd5Hash(FileInfo file)
        {
            using (MD5 md5 = MD5.Create())
            using (FileStream fs = file.OpenRead())
            {
                return BitConverter.ToString(md5.ComputeHash(fs)).Replace("-", "").ToLowerInvariant();
            }
        }

        public void FindDuplicates()
        {
            if(RootDirectoryPath.IsNullOrEmpty())
            {
                throw new NullReferenceException("RootDirectoryPath cannot be null or empty.");
            }

            IEnumerable<FileInfo> files = Find(new List<DirectoryInfo> { new DirectoryInfo(RootDirectoryPath) });

            Dictionary<string, DuplicateGroup> dictionary = new Dictionary<string, DuplicateGroup>();

            TotalFileCount = files.Count();
            ProcessedFileCount = 0;

            foreach (FileInfo file in files)
            {
                Debug.WriteLine($"Computing hash of: {file.FullName}...");
                ProcessingFilePath = file.FullName;

                OnProgressChanged();

                string hash = ComputeMd5Hash(file);

                if (dictionary.ContainsKey(hash) && dictionary[hash].FileSize == file.Length)
                {
                    dictionary[hash].Add(file);
                }
                else
                {
                    dictionary[hash] = new DuplicateGroup(hash, file);
                }

                ProcessedFileCount++;

                OnProgressChanged();
            }

            // Return DuplicateGroup objects containing more than one files
            DuplicateGroups = dictionary.Values.Where(g => g.Count > 1);
        }
    }
}
