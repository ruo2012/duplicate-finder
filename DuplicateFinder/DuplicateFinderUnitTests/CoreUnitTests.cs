using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Collections.Generic;
using DuplicateFinder;
using System.Linq;

namespace DuplicateFinderUnitTests
{
    [TestClass]
    public class CoreUnitTests
    {
        private List<string> directories = new List<string>
        {
            "root", "root\\dir1", "root\\dir2",
            "root\\dir1\\dir1"
        };

        private List<string> files = new List<string>
        {
            "file1", "file2", "file3", "file4"
        };

        private List<string> fileContents = new List<string>
        {
            "This is duplicate file.", "This is not a duplicate file."
        };

        [TestMethod]
        public void FindTests()
        {
            List<FileInfo> files = Core.Find(new List<DirectoryInfo> { new DirectoryInfo(directories[0]) }).ToList();

            // For now - Add more conditions
            Assert.IsTrue(files.Count == 16);
        }

        [TestInitialize]
        public void TestInitialize()
        {
            foreach(string directory in directories)
            {
                Directory.CreateDirectory(directory);

                foreach(string file in files)
                {
                    using (StreamWriter fs = File.CreateText(Path.Combine(directory, file)))
                    {
                        // file1-3 are duplicates
                        if(file.Equals(files[3]))
                        {
                            fs.WriteLine(fileContents[1]); // not a duplicate
                        }
                        else
                        {
                            fs.WriteLine(fileContents[0]); // duplicate
                        }
                    }
                }
            }
        }

        [TestCleanup]
        public void TestCleanUp()
        {
            Directory.Delete(directories[0], true);
        }
    }
}
