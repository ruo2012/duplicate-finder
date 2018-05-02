using System.Collections.Generic;
using System.IO;

namespace DuplicateFinder
{
    /// <summary>
    /// A class representing a group of duplicate files
    /// </summary>
    public class DuplicateGroup
    {
        /// <summary>
        /// MD5 hash of this duplicate group
        /// </summary>
        public string Hash { get; private set; }

        /// <summary>
        /// File size of this duplicate group
        /// </summary>
        public long FileSize { get; private set; }

        /// <summary>
        /// File paths of this duplicate group
        /// </summary>
        public IEnumerable<FileInfo> FilePaths
        {
            get { return Files.AsReadOnly(); }
        }

        /// <summary>
        /// Number of duplicate files in this group
        /// </summary>
        public int Count => Files.Count;

        private List<FileInfo> Files = new List<FileInfo>();

        public DuplicateGroup(string hash, FileInfo file)
        {
            Hash = hash;
            FileSize = file.Length;
            Files.Add(file);
        }

        /// <summary>
        /// Add a file path to this duplicate group
        /// </summary>
        /// <param name="filePath">Duplicate file path</param>
        public void Add(FileInfo file)
        {
            Files.Add(file);
        }
    }
}
